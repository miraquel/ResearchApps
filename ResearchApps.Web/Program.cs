using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ResearchApps.Repo;
using ResearchApps.Service;
using ResearchApps.Web.Context;
using ResearchApps.Web.Exceptions;
using ResearchApps.Web.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ResearchAppsDbContext>()
    .AddApiEndpoints();

builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Register custom services
builder.Services.AddRepositories();
builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler("/Home/Error");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.MapIdentityApi<IdentityUser>();

app.Run();