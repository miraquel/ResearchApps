# Playwright Tests — Setup & Run

## Prerequisites

1. Install Playwright browsers (one-time):
   ```bash
   dotnet build ResearchApps.Playwright.Tests
   pwsh ResearchApps.Playwright.Tests/bin/Debug/net10.0/playwright.ps1 install chromium
   ```
   On Windows without PowerShell Core:
   ```bash
   dotnet tool install --global Microsoft.Playwright.CLI
   playwright install chromium
   ```

2. Start the web app on `http://localhost:5112`:
   ```bash
   dotnet run --project ResearchApps.Web --launch-profile http
   ```

## Running Tests

```bash
# All Playwright tests
dotnet test ResearchApps.Playwright.Tests

# Specific category
dotnet test ResearchApps.Playwright.Tests --filter "FullyQualifiedName~Auth"
dotnet test ResearchApps.Playwright.Tests --filter "FullyQualifiedName~Prs"
dotnet test ResearchApps.Playwright.Tests --filter "FullyQualifiedName~Pss"
dotnet test ResearchApps.Playwright.Tests --filter "FullyQualifiedName~Navigation"
```

## Test Coverage

| Suite | What it guards |
|---|---|
| `Auth/LoginTests` | Valid login → dashboard; invalid → error; unauth → redirect to login |
| `Auth/AccessControlTests` | SuperAdmin can reach Tenants admin; logout gates protected routes |
| `Prs/PrCrudTests` | PR list renders; Create form loads; create redirects to Edit; details page renders |
| `Prs/PrWorkflowTests` | Submit with zero lines shows error; workflow buttons section renders |
| `Pss/PssCrudTests` | PSS list renders; Create form loads; no 500 on create; details shows line table |
| `Navigation/NavigationConsistencyTests` | `.page-title-box` present on list pages; breadcrumb on details; sidebar/topbar present; no 500s on critical routes |

## Credentials Used

- SuperAdmin: `superadmin` / `SuperAdmin@123!` (main site, no tenant subdomain)
