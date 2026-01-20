# Testing Guide

## Overview

- **Framework**: xUnit v2.9.3
- **Mocking**: Moq v4.20.72
- **Project**: `ResearchApps.Service.Tests`
- **Coverage**: All public service methods

## Test Structure

```
ResearchApps.Service.Tests/
├── GlobalUsings.cs           # Shared imports
├── BudgetServiceTests.cs
├── CustomerOrderServiceTests.cs
├── CustomerServiceTests.cs
├── ItemServiceTests.cs
├── PrServiceTests.cs
├── PrLineServiceTests.cs
└── ... (17 test classes total)
```

## Naming Conventions

- **Test class**: `{ServiceName}Tests`
- **Test method**: `{MethodName}_{Scenario}_{ExpectedBehavior}`

```csharp
public class ItemServiceTests
{
    [Fact]
    public async Task InsertAsync_WithValidItem_ReturnsCreatedWithId()
    
    [Fact]
    public async Task SelectByIdAsync_WithExistingId_ReturnsItem()
    
    [Fact]
    public async Task DeleteAsync_WithValidId_CommitsTransaction()
}
```

## Test Pattern (AAA)

```csharp
public class PrLineServiceTests
{
    private readonly Mock<IPrLineRepo> _prLineRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly Mock<ILogger<PrLineService>> _loggerMock;
    private readonly PrLineService _sut;  // System Under Test

    public PrLineServiceTests()
    {
        _prLineRepoMock = new Mock<IPrLineRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        _loggerMock = new Mock<ILogger<PrLineService>>();

        _sut = new PrLineService(
            _prLineRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            _loggerMock.Object);
    }

    [Fact]
    public async Task PrLineInsert_WithValidData_ReturnsSuccessAndCommits()
    {
        // Arrange
        var prLineVm = new PrLineVm { PrId = "PR001", ItemId = 1, Qty = 10 };
        var expectedResult = "PR001-1";

        _prLineRepoMock
            .Setup(x => x.PrLineInsert(It.IsAny<PrLine>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.PrLineInsert(prLineVm, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult, result.Data);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task PrLineSelectByPr_WithValidPrId_ReturnsLines()
    {
        // Arrange
        var prId = "PR001";
        var prLines = new List<PrLine>
        {
            new() { PrLineId = 1, PrId = prId },
            new() { PrLineId = 2, PrId = prId }
        };

        _prLineRepoMock
            .Setup(x => x.PrLineSelectByPr(prId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(prLines);

        // Act
        var result = await _sut.PrLineSelectByPr(prId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());
    }
}
```

## Key Assertions

```csharp
// ServiceResponse success
Assert.True(result.IsSuccess);
Assert.Equal("Expected message", result.Message);

// ServiceResponse<T> data access
Assert.NotNull(result.Data);
Assert.Equal(expectedValue, result.Data.PropertyName);

// Status codes
Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

// Transaction verification
_dbTransactionMock.Verify(x => x.Commit(), Times.Once);
_dbTransactionMock.Verify(x => x.Commit(), Times.Never);  // For read-only ops

// Repository calls
_repoMock.Verify(x => x.InsertAsync(It.Is<Item>(i => 
    i.ItemName == "Expected" && i.CreatedBy == "testuser"), 
    It.IsAny<CancellationToken>()), Times.Once);
```

## Running Tests

```powershell
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~ItemServiceTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

## GlobalUsings.cs

```csharp
global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Logging;
global using Moq;
global using ResearchApps.Domain;
global using ResearchApps.Domain.Common;
global using ResearchApps.Repo.Interface;
global using ResearchApps.Service.Interface;
global using ResearchApps.Service.Vm;
global using ResearchApps.Service.Vm.Common;
global using Xunit;
```

## Testing Checklist for New Services

1. ✅ Test all public methods
2. ✅ Verify `ServiceResponse.IsSuccess` 
3. ✅ Access `result.Data` for generic responses
4. ✅ Verify `_dbTransaction.Commit()` for mutations
5. ✅ Verify correct status codes (201 for create, 200 for others)
6. ✅ Test that `CreatedBy`/`ModifiedBy` comes from `UserClaimDto`
