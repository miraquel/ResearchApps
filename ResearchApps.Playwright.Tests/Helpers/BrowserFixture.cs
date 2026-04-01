namespace ResearchApps.Playwright.Tests.Helpers;

/// <summary>
/// xUnit collection fixture that owns the Playwright browser instance for the entire test run.
/// Tests request a new BrowserContext (isolated session) per test class.
/// </summary>
public sealed class BrowserFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;
    public IBrowser Browser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.DisposeAsync();
        _playwright?.Dispose();
    }
}

[CollectionDefinition("Browser")]
public class BrowserCollection : ICollectionFixture<BrowserFixture> { }
