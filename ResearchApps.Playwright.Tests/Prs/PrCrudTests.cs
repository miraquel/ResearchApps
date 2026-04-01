namespace ResearchApps.Playwright.Tests.Prs;

[Collection("Browser")]
public sealed class PrCrudTests : IAsyncLifetime
{
    private readonly BrowserFixture _fixture;
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    public PrCrudTests(BrowserFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        _context = await _fixture.Browser.NewContextAsync();
        _page = await _context.NewPageAsync();
        await PageHelpers.LoginAsSuperAdminAsync(_page);
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();

    [Fact]
    public async Task PrList_LoadsSuccessfully()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(PageHelpers.GetUrlPath(_page).StartsWith("/Prs", StringComparison.OrdinalIgnoreCase));

        // Wait for HTMX to inject the list content
        await PageHelpers.WaitForHtmxContentAsync(_page, "#pr-list-container");

        var hasContent = await _page.IsVisibleAsync("table, .table") ||
                         await _page.IsVisibleAsync(".text-muted");
        Assert.True(hasContent, "PR list should render a table or empty state after HTMX load");
    }

    [Fact]
    public async Task PrCreate_FormRendersCorrectly()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs/Create");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check we're actually on the Create page, not redirected to access denied
        Assert.Equal("/Prs/Create", PageHelpers.GetUrlPath(_page));

        var formExists = await _page.QuerySelectorAsync("form#pr-form, form[action*='/Prs/Create']");
        Assert.NotNull(formExists);
    }

    [Fact]
    public async Task PrCreate_Submit_DoesNotCauseServerError()
    {
        // Verifies that submitting the PR create form never produces a 500 error.
        // Validation failures (missing required fields) are acceptable — server errors are not.
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs/Create");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        if (PageHelpers.GetUrlPath(_page) != "/Prs/Create") return;

        // Attempt a partial form fill and submit
        await _page.EvaluateAsync(@"
            const input = document.getElementById('PrDate');
            if (input) {
                const fp = input._flatpickr;
                if (fp) fp.setDate('2026-03-31');
                else { input.value = '2026-03-31'; input.dispatchEvent(new Event('change', {bubbles:true})); }
            }
        ");

        var nameInput = await _page.QuerySelectorAsync("#PrName");
        if (nameInput != null) await nameInput.FillAsync("Playwright Regression Test PR");

        await PageHelpers.JsClickAsync(_page, "button.btn-primary");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Accept any confirm dialog if shown
        try
        {
            await _page.WaitForSelectorAsync(".swal2-confirm",
                new PageWaitForSelectorOptions { Timeout = 2000 });
            await PageHelpers.JsClickAsync(_page, ".swal2-confirm");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        catch (TimeoutException) { }

        // Result: either redirected to Edit (success) or back on Create (validation failed).
        // Both are acceptable — a 500 error page is not.
        var title = await _page.TitleAsync();
        Assert.DoesNotContain("500", title);
        Assert.DoesNotContain("Error", title, StringComparison.OrdinalIgnoreCase);
        var path = PageHelpers.GetUrlPath(_page);
        Assert.True(path.StartsWith("/Prs/"), $"Unexpected redirect to {_page.Url}");
    }

    [Fact]
    public async Task PrDetails_ShowsCardContent()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await PageHelpers.WaitForHtmxContentAsync(_page, "#pr-list-container");

        // Read the details href from the hidden dropdown — don't click it
        var href = await PageHelpers.GetFirstDetailsHrefAsync(_page, "#pr-list-container", "/Prs/Details/");
        if (href == null) return; // No records

        await _page.GotoAsync($"{TestConstants.BaseUrl}{href}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(PageHelpers.GetUrlPath(_page).StartsWith("/Prs/Details/"),
            $"Expected /Prs/Details/{{id}} but got {_page.Url}");

        var hasContent = await _page.IsVisibleAsync(".card");
        Assert.True(hasContent, "PR details page should render a card");
    }
}
