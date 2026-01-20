namespace ResearchApps.Service.Tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepo> _customerRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly CustomerService _sut;

    public CustomerServiceTests()
    {
        _customerRepoMock = new Mock<ICustomerRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<CustomerService>>();
        _userClaimDto = new UserClaimDto { Username = "testuser" };
        
        _sut = new CustomerService(
            _customerRepoMock.Object,
            _dbTransactionMock.Object,
            _userClaimDto,
            loggerMock.Object);
    }

    [Fact]
    public async Task CustomerSelect_WithValidRequest_ReturnsPagedList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
        var customers = new PagedList<Customer>( new List<Customer> { new() { CustomerId = 1, CustomerName = "Customer 1" }, new() { CustomerId = 2, CustomerName = "Customer 2" } }, 1, 10, 2 );

        _customerRepoMock
            .Setup(x => x.CustomerSelect(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(customers);

        // Act
        var result = await _sut.CustomerSelect(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customers retrieved successfully.", result.Message);
        _customerRepoMock.Verify(x => x.CustomerSelect(It.IsAny<PagedListRequest>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CustomerSelectById_WithValidId_ReturnsCustomer()
    {
        // Arrange
        var customerId = 1;
        var cancellationToken = CancellationToken.None;
        var customer = new Customer { CustomerId = customerId, CustomerName = "Test Customer" };

        _customerRepoMock
            .Setup(x => x.CustomerSelectById(customerId, cancellationToken))
            .ReturnsAsync(customer);

        // Act
        var result = await _sut.CustomerSelectById(customerId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer retrieved successfully.", result.Message);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.Equal(customerId, data.CustomerId);
    }

    [Fact]
    public async Task CustomerInsert_WithValidCustomer_ReturnsInsertedId()
    {
        // Arrange
        var customerVm = new CustomerVm { CustomerName = "New Customer", Npwp = "123456789" };
        var cancellationToken = CancellationToken.None;
        var insertedId = 10;

        _customerRepoMock
            .Setup(x => x.CustomerInsert(It.IsAny<Customer>(), cancellationToken))
            .ReturnsAsync(insertedId);

        // Act
        var result = await _sut.CustomerInsert(customerVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertedId, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CustomerInsert_SetsCreatedByFromUserClaim()
    {
        // Arrange
        var customerVm = new CustomerVm { CustomerName = "New Customer" };
        var cancellationToken = CancellationToken.None;
        Customer? capturedCustomer = null;

        _customerRepoMock
            .Setup(x => x.CustomerInsert(It.IsAny<Customer>(), cancellationToken))
            .Callback<Customer, CancellationToken>((c, _) => capturedCustomer = c)
            .ReturnsAsync(1);

        // Act
        await _sut.CustomerInsert(customerVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedCustomer);
        Assert.Equal(_userClaimDto.Username, capturedCustomer.CreatedBy);
    }

    [Fact]
    public async Task CustomerUpdate_WithValidCustomer_CommitsTransaction()
    {
        // Arrange
        var customerVm = new CustomerVm { CustomerId = 1, CustomerName = "Updated Customer" };
        var cancellationToken = CancellationToken.None;

        _customerRepoMock
            .Setup(x => x.CustomerUpdate(It.IsAny<Customer>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CustomerUpdate(customerVm, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CustomerUpdate_SetsModifiedByFromUserClaim()
    {
        // Arrange
        var customerVm = new CustomerVm { CustomerId = 1, CustomerName = "Updated Customer" };
        var cancellationToken = CancellationToken.None;
        Customer? capturedCustomer = null;

        _customerRepoMock
            .Setup(x => x.CustomerUpdate(It.IsAny<Customer>(), cancellationToken))
            .Callback<Customer, CancellationToken>((c, _) => capturedCustomer = c)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.CustomerUpdate(customerVm, cancellationToken);

        // Assert
        Assert.NotNull(capturedCustomer);
        Assert.Equal(_userClaimDto.Username, capturedCustomer.ModifiedBy);
    }

    [Fact]
    public async Task CustomerDelete_WithValidId_CommitsTransaction()
    {
        // Arrange
        var customerId = 1;
        var cancellationToken = CancellationToken.None;

        _customerRepoMock
            .Setup(x => x.CustomerDelete(customerId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CustomerDelete(customerId, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customer deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CustomerCbo_WithValidRequest_ReturnsCustomerList()
    {
        // Arrange
        var request = new CboRequestVm { Term = "Test" };
        var cancellationToken = CancellationToken.None;
        var customers = new List<Customer>
        {
            new() { CustomerId = 1, CustomerName = "Test Customer 1" },
            new() { CustomerId = 2, CustomerName = "Test Customer 2" }
        };

        _customerRepoMock
            .Setup(x => x.CustomerCbo(It.IsAny<CboRequest>(), cancellationToken))
            .ReturnsAsync(customers);

        // Act
        var result = await _sut.CustomerCbo(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Customers retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CustomerInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        // Arrange
        var customerVm = new CustomerVm { CustomerName = "New Customer" };
        var cancellationToken = CancellationToken.None;

        _customerRepoMock
            .Setup(x => x.CustomerInsert(It.IsAny<Customer>(), cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.CustomerInsert(customerVm, cancellationToken));
        
        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CustomerSelect_WithEmptyResults_ReturnsSuccessWithEmptyList()
    {
        // Arrange
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var cancellationToken = CancellationToken.None;
        var customers = new PagedList<Customer>(
            new List<Customer>(),
            1,
            10,
            0
        );

        _customerRepoMock
            .Setup(x => x.CustomerSelect(It.IsAny<PagedListRequest>(), cancellationToken))
            .ReturnsAsync(customers);

        // Act
        var result = await _sut.CustomerSelect(request, cancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.TotalCount);
    }
}




