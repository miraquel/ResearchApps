namespace ResearchApps.Playwright.Tests.Navigation;

/// <summary>
/// Ensures consistent layout elements (breadcrumb, sidebar, topbar) are present
/// across the app after the design refactor.
/// </summary>
[Collection("Browser")]
public sealed class NavigationConsistencyTests : IAsyncLifetime
{
    private readonly BrowserFixture _fixture;
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    public NavigationConsistencyTests(BrowserFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        _context = await _fixture.Browser.NewContextAsync();
        _page = await _context.NewPageAsync();
        await PageHelpers.LoginAsSuperAdminAsync(_page);
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();

    [Theory]
    [InlineData("/Prs")]
    [InlineData("/Pss")]
    [InlineData("/Items")]
    public async Task PageTitleBox_IsPresentOnListPages(string route)
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}{route}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var titleBox = await _page.IsVisibleAsync(".page-title-box");
        Assert.True(titleBox, $"Expected .page-title-box on {route}");
    }

    [Fact]
    public async Task Breadcrumb_IsPresentOnDetailsPage()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await PageHelpers.WaitForHtmxContentAsync(_page, "#pr-list-container");

        var href = await PageHelpers.GetFirstDetailsHrefAsync(_page, "#pr-list-container", "/Prs/Details/");
        if (href == null) return; // No records

        await _page.GotoAsync($"{TestConstants.BaseUrl}{href}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var breadcrumb = await _page.IsVisibleAsync(".breadcrumb");
        Assert.True(breadcrumb, "Breadcrumb should be present on PR details page");
    }

    [Fact]
    public async Task Sidebar_IsPresent_OnAuthenticatedPages()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var sidebar = await _page.IsVisibleAsync("#sidebar, .app-menu, .navbar-menu");
        Assert.True(sidebar, "Sidebar/navbar-menu should be visible on authenticated pages");
    }

    [Fact]
    public async Task Topbar_IsPresent_OnAuthenticatedPages()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var topbar = await _page.IsVisibleAsync("#page-topbar, .navbar-header");
        Assert.True(topbar, "Topbar should be visible on authenticated pages");
    }

    [Fact]
    public async Task No500Errors_OnCriticalRoutes()
    {
        string[] routes = ["/Prs", "/Pss", "/Items", "/Prs/Create", "/Pss/Create"];
        var errors = new List<string>();

        foreach (var route in routes)
        {
            var response = await _page.GotoAsync($"{TestConstants.BaseUrl}{route}");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            if (response?.Status >= 500)
                errors.Add($"{route} returned {response.Status}");
        }

        Assert.Empty(errors);
    }
}
