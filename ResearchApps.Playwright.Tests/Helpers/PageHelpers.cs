namespace ResearchApps.Playwright.Tests.Helpers;

public static class TestConstants
{
    public const string BaseUrl = "http://localhost:5112";
    public const string SuperAdminUser = "superadmin";
    public const string SuperAdminPassword = "SuperAdmin@123!";
}

public static class PageHelpers
{
    public static async Task LoginAsync(IPage page, string username, string password)
    {
        await page.GotoAsync($"{TestConstants.BaseUrl}/Identity/Account/Login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.FillAsync("#Input_UserName", username);
        await page.FillAsync("#password-input", password);
        await page.ClickAsync("#login-submit");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public static async Task LoginAsSuperAdminAsync(IPage page)
        => await LoginAsync(page, TestConstants.SuperAdminUser, TestConstants.SuperAdminPassword);

    public static bool IsOnLoginPage(IPage page)
        => page.Url.Contains("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Waits for an HTMX-driven container to finish loading by waiting for the
    /// spinner inside it to be replaced by real content.
    /// Falls back gracefully after the timeout.
    /// </summary>
    public static async Task WaitForHtmxContentAsync(IPage page, string containerId, int timeoutMs = 10_000)
    {
        try
        {
            // Wait for the spinner / placeholder to disappear inside the container
            await page.WaitForSelectorAsync(
                $"{containerId} table, {containerId} .text-muted:not(.spinner-border), {containerId} [class*='empty']",
                new PageWaitForSelectorOptions { Timeout = timeoutMs });
        }
        catch (TimeoutException)
        {
            // Content might still be loading or genuinely empty — let the test assertion decide
        }
    }

    /// <summary>
    /// Clicks an element via JavaScript, bypassing Playwright's visibility check.
    /// Useful for elements that are technically in the DOM but Playwright considers not visible
    /// (e.g., off-screen, inside collapsed panels, or with zero-dimension parent).
    /// </summary>
    public static async Task JsClickAsync(IPage page, string selector)
    {
        await page.EvaluateAsync($@"
            const el = document.querySelector('{selector.Replace("'", "\\'")}');
            if (el) el.click();
        ");
    }

    /// <summary>
    /// Returns the href of the first Details/View link within an HTMX list container.
    /// Links may be inside hidden dropdown menus, so we read the href via JS instead of clicking.
    /// </summary>
    public static async Task<string?> GetFirstDetailsHrefAsync(IPage page, string listContainerId, string routePrefix)
    {
        return await page.EvaluateAsync<string?>($@"(() => {{
            const container = document.querySelector('{listContainerId}');
            if (!container) return null;
            const links = container.querySelectorAll('a[href*=""{routePrefix}""]');
            return links.length > 0 ? links[0].getAttribute('href') : null;
        }})()");
    }

    public static string GetUrlPath(IPage page)
    {
        var uri = new Uri(page.Url);
        return uri.AbsolutePath;
    }
}
