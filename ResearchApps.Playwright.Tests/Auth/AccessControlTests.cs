namespace ResearchApps.Playwright.Tests.Auth;

[Collection("Browser")]
public sealed class AccessControlTests : IAsyncLifetime
{
    private readonly BrowserFixture _fixture;
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    public AccessControlTests(BrowserFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        _context = await _fixture.Browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();

    [Fact]
    public async Task SuperAdmin_CanSeeTenantsMenu()
    {
        await PageHelpers.LoginAsSuperAdminAsync(_page);

        // Navigate to admin area
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Admin/Tenants");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Should NOT be redirected to login or a 403 page
        Assert.False(PageHelpers.IsOnLoginPage(_page),
            "SuperAdmin should have access to Tenants admin area");
        Assert.DoesNotContain("Access Denied", await _page.TitleAsync());
    }

    [Fact]
    public async Task SuperAdmin_CanAccessAllTopLevelRoutes()
    {
        await PageHelpers.LoginAsSuperAdminAsync(_page);

        string[] routes = ["/Prs", "/Pss", "/Items", "/Admin/Tenants"];

        foreach (var route in routes)
        {
            await _page.GotoAsync($"{TestConstants.BaseUrl}{route}");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            Assert.False(PageHelpers.IsOnLoginPage(_page),
                $"SuperAdmin should have access to {route}");
        }
    }

    [Fact]
    public async Task AfterLogout_ProtectedRoutesRedirectToLogin()
    {
        await PageHelpers.LoginAsSuperAdminAsync(_page);

        // Logout
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Identity/Account/Logout");
        // Some apps use POST for logout; fall back to navigating the logout page
        var logoutForm = await _page.QuerySelectorAsync("form[action*='Logout']");
        if (logoutForm != null)
        {
            await logoutForm.EvaluateAsync("f => f.submit()");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(PageHelpers.IsOnLoginPage(_page),
            "After logout, protected routes should redirect to login");
    }
}
