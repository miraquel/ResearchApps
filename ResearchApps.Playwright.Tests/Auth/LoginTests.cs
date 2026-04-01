namespace ResearchApps.Playwright.Tests.Auth;

[Collection("Browser")]
public sealed class LoginTests : IAsyncLifetime
{
    private readonly BrowserFixture _fixture;
    private IBrowserContext _context = null!;
    private IPage _page = null!;

    public LoginTests(BrowserFixture fixture) => _fixture = fixture;

    public async Task InitializeAsync()
    {
        _context = await _fixture.Browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    public async Task DisposeAsync() => await _context.DisposeAsync();

    [Fact]
    public async Task ValidLogin_RedirectsToDashboard()
    {
        await PageHelpers.LoginAsSuperAdminAsync(_page);

        Assert.False(PageHelpers.IsOnLoginPage(_page));
        Assert.DoesNotContain("/Identity/Account/Login", _page.Url);
    }

    [Fact]
    public async Task InvalidLogin_ShowsErrorMessage()
    {
        await PageHelpers.LoginAsync(_page, "superadmin", "WrongPassword!");

        Assert.True(PageHelpers.IsOnLoginPage(_page));

        var errorVisible = await _page.IsVisibleAsync(".validation-summary-errors, [class*='alert-danger'], .text-danger");
        Assert.True(errorVisible, "Expected an error message to be visible after invalid login");
    }

    [Fact]
    public async Task UnauthenticatedAccess_RedirectsToLogin()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Prs");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(PageHelpers.IsOnLoginPage(_page),
            $"Expected redirect to login but got: {_page.Url}");
    }

    [Fact]
    public async Task UnauthenticatedAccess_ToAdminArea_RedirectsToLogin()
    {
        await _page.GotoAsync($"{TestConstants.BaseUrl}/Admin/Tenants");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(PageHelpers.IsOnLoginPage(_page),
            $"Expected redirect to login but got: {_page.Url}");
    }
}
