using System.Security.Claims;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.EntityFrameworkCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using QuestPDF.Infrastructure;
using ResearchApps.Common.Constants;
using ResearchApps.Common.Tenant;
using ResearchApps.Domain;
using ResearchApps.Repo;
using ResearchApps.Service;
using ResearchApps.Service.Vm.Common;
using ResearchApps.Web.Context;
using ResearchApps.Web.Data;
using ResearchApps.Web.Exceptions;
using ResearchApps.Web.Hubs;
using ResearchApps.Web.Logging;
using ResearchApps.Web.Middlewares;
using ResearchApps.Web.Services;
using ResearchApps.Web.Swagger;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

// Configure QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

// Configure EPPlus License
ExcelPackage.License.SetNonCommercialOrganization("Research Apps"); //This will also set the Company property to the organization name provided in the argument.

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting ResearchApps application...");

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging with tenant enrichment
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        // Finbuckle logs expected "No tenant found in authentication properties" warnings for main-site
        // (non-tenant) accounts on every authenticated request — suppress to Error only.
        .MinimumLevel.Override("Finbuckle", Serilog.Events.LogEventLevel.Error)
        .Enrich.FromLogContext()
        .Enrich.WithTenantInfo(services)
        .WriteTo.Console()
        .WriteTo.File(
            path: "Logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] [Tenant:{TenantIdentifier}] {Message:lj}{NewLine}{Exception}"));

    // Add services to the container.
    builder.Services.AddProblemDetails(configure =>
    {
        configure.CustomizeProblemDetails = options =>
        {
            options.ProblemDetails.Extensions.TryAdd("traceId", options.HttpContext.TraceIdentifier);
        };
    });
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    builder.Services.AddDbContext<ResearchAppsDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()));
    builder.Services.AddDbContext<TenantStoreDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddDefaultIdentity<AppIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<AppIdentityRole>()
        .AddEntityFrameworkStores<ResearchAppsDbContext>()
        .AddApiEndpoints();

    builder.Services.AddAuthentication()
        .AddBearerToken(IdentityConstants.BearerScheme);

    builder.Services.AddAuthorization(options =>
    {
        var permissions = PermissionConstants.GetAllPermissions();

        if (permissions.Count == 0) return;
        
        foreach (var permission in permissions)
        {
            options.AddPolicy(permission, policy => policy.RequireClaim("permission", permission));
        }
    });

    var mvcBuilder = builder.Services.AddControllersWithViews();
    if (builder.Environment.IsDevelopment())
    {
        mvcBuilder.AddRazorRuntimeCompilation();
    }
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

    builder.Services.AddScoped(serviceProvider =>
    {
        var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        var username = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? "";
        _ = Guid.TryParse(httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out var userId);

        // Derive tenant info from Finbuckle context (not user claims)
        var tenantInfo = httpContextAccessor.HttpContext?.GetMultiTenantContext<AppTenantInfo>()?.TenantInfo;
        var userClaimDto = new UserClaimDto
        {
            UserId = userId,
            Username = username,
            TenantId = tenantInfo?.Id ?? "",
            TenantIdentifier = tenantInfo?.Identifier ?? ""
        };

        return userClaimDto;
    });

    // Register multi-tenant services with per-tenant authentication
    builder.Services.AddMultiTenant<AppTenantInfo>()
        .WithHostStrategy("__tenant__.*")
        .WithEFCoreStore<TenantStoreDbContext, AppTenantInfo>()
        .WithPerTenantAuthentication();

    // Per-tenant cookie names: each tenant gets its own cookie so sessions don't collide
    builder.Services.ConfigurePerTenant<CookieAuthenticationOptions, AppTenantInfo>(
        IdentityConstants.ApplicationScheme, (options, tenantInfo) =>
        {
            options.Cookie.Name = $".AspNetCore.Identity.App.{tenantInfo.Identifier}";
        });

    // Register custom services
    builder.Services.AddRepositories();
    builder.Services.AddServices();

    // Register report generator service
    builder.Services.AddScoped<IReportGeneratorService, ReportGeneratorService>();

    // Register SignalR
    builder.Services.AddSignalR();

    // Register unified workflow notification service as Singleton (stateless, IHubContext<T> is Singleton)
    builder.Services.AddSingleton<IWorkflowNotificationService, WorkflowNotificationService>();

    // Register tenant provisioning service
    builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
    // Register tenant user management service
    builder.Services.AddScoped<ITenantUserManagementService, TenantUserManagementService>();
    // Register tenant feature flag service
    builder.Services.AddScoped<ITenantFeatureService, TenantFeatureService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseStaticFiles();

    app.UseMultiTenant();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapAreaControllerRoute(
        name: "Admin",
        areaName: "Admin",
        pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.MapRazorPages()
        .WithStaticAssets();

    // Map SignalR hub with stateful reconnect for reliability during brief disconnections
    app.MapHub<WorkflowHub>("/hubs/workflow", options =>
    {
        options.AllowStatefulReconnects = true;
    });

    // Seed super-admin role and user on startup
    await SuperAdminSeeder.SeedAsync(app.Services);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
