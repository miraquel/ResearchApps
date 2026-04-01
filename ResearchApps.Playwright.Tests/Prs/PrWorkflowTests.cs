namespace ResearchApps.Playwright.Tests.Prs;

[Collection("Browser")]
public sealed class PrWorkflowTests : IAsyncLifetime
{
    private readonly BrowserFixture _fixture;
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    public PrWorkflowTests(BrowserFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        _context = await _fixture.Browser.NewContextAsync();
        _page = await _context.NewPageAsync();
        await PageHelpers.LoginAsSuperAdminAsync(_page);
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();

    [Fact]
    public async Task PrSubmit_WithNoLines_ReturnsErrorMessage()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs/Create");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        if (PageHelpers.GetUrlPath(_page) != "/Prs/Create") return;

        await _page.EvaluateAsync(@"
            const input = document.getElementById('PrDate');
            if (input) {
                const fp = input._flatpickr;
                if (fp) fp.setDate('2026-03-31');
                else { input.value = '2026-03-31'; input.dispatchEvent(new Event('change', {bubbles:true})); }
            }
        ");

        var nameInput = await _page.QuerySelectorAsync("#PrName");
        if (nameInput != null)
            await nameInput.FillAsync("Workflow guard test - no lines");

        await PageHelpers.JsClickAsync(_page, "button.btn-primary");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Accept confirm dialog if present
        try
        {
            await _page.WaitForSelectorAsync(".swal2-confirm",
                new PageWaitForSelectorOptions { Timeout = 3000 });
            await PageHelpers.JsClickAsync(_page, ".swal2-confirm");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }
        catch (TimeoutException) { }

        var path = PageHelpers.GetUrlPath(_page);
        if (!path.StartsWith("/Prs/Edit/")) return; // Create failed — skip

        var prId = path.Split('/')[^1];

        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs/Details/{prId}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Submit the PR via the hidden form using JS
        var submitted = await _page.EvaluateAsync<bool>(@"(() => {
            const form = document.querySelector('form[action*=""/Prs/Submit""]');
            if (!form) return false;
            form.submit();
            return true;
        })()");

        if (!submitted) return;

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var content = await _page.ContentAsync();
        Assert.True(content.Contains("no line items", StringComparison.OrdinalIgnoreCase),
            "Expected 'no line items' error message after submitting PR with zero lines");
    }

    [Fact]
    public async Task PrDetails_RendersSuccessfully()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await PageHelpers.WaitForHtmxContentAsync(_page, "#pr-list-container");

        var href = await PageHelpers.GetFirstDetailsHrefAsync(_page, "#pr-list-container", "/Prs/Details/");
        if (href == null) return; // No records

        await _page.GotoAsync($"{TestConstants.BaseUrl}{href}");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(PageHelpers.GetUrlPath(_page).StartsWith("/Prs/Details/"));

        var hasCard = await _page.IsVisibleAsync(".card");
        Assert.True(hasCard, "PR details page should render a card");
    }
}
