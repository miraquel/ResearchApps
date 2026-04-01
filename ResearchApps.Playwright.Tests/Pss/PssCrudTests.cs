namespace ResearchApps.Playwright.Tests.Pss;

[Collection("Browser")]
public sealed class PssCrudTests : IAsyncLifetime
{
    private readonly BrowserFixture _fixture;
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    public PssCrudTests(BrowserFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        _context = await _fixture.Browser.NewContextAsync();
        _page = await _context.NewPageAsync();
        await PageHelpers.LoginAsSuperAdminAsync(_page);
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();

    [Fact]
    public async Task PssList_LoadsSuccessfully()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Pss");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(PageHelpers.GetUrlPath(_page).StartsWith("/Pss", StringComparison.OrdinalIgnoreCase));

        await PageHelpers.WaitForHtmxContentAsync(_page, "#ps-list-container");

        var hasContent = await _page.IsVisibleAsync("table, .table") ||
                         await _page.IsVisibleAsync(".text-muted");
        Assert.True(hasContent, "PSS list should render a table or empty state after HTMX load");
    }

    [Fact]
    public async Task PssCreate_FormRendersCorrectly()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Pss/Create");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Equal("/Pss/Create", PageHelpers.GetUrlPath(_page));

        var formExists = await _page.QuerySelectorAsync("form");
        Assert.NotNull(formExists);
    }

    [Fact]
    public async Task PssCreate_NoServerError()
    {
        var response = await _page.GotoAsync($"{TestConstants.BaseUrl}/Pss/Create");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.NotNull(response);
        Assert.True(response.Status < 500, $"Expected non-500 status but got {response.Status}");
    }

    [Fact]
    public async Task PssDetails_RendersSuccessfully()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Pss");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await PageHelpers.WaitForHtmxContentAsync(_page, "#ps-list-container");

        var href = await PageHelpers.GetFirstDetailsHrefAsync(_page, "#ps-list-container", "/Pss/Details/");
        if (href == null) return; // No records

        await _page.GotoAsync($"{TestConstants.BaseUrl}{href}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(PageHelpers.GetUrlPath(_page).StartsWith("/Pss/"),
            $"Expected /Pss/Details/{{id}} but got {_page.Url}");

        var hasCard = await _page.IsVisibleAsync(".card, .page-title-box");
        Assert.True(hasCard, "PSS details should show a card/title area");
    }

    [Fact]
    public async Task PssDetails_LineTable_IsPresent()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Pss");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await PageHelpers.WaitForHtmxContentAsync(_page, "#ps-list-container");

        var href = await PageHelpers.GetFirstDetailsHrefAsync(_page, "#ps-list-container", "/Pss/Details/");
        if (href == null) return;

        await _page.GotoAsync($"{TestConstants.BaseUrl}{href}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var hasTable = await _page.IsVisibleAsync("table, .table") ||
                       await _page.IsVisibleAsync(".text-muted");
        Assert.True(hasTable, "PSS details should show a line table or empty state");
    }
}
