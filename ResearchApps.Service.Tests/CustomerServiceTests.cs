namespace ResearchApps.Service.Tests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepo> _customerRepoMock;
    private readonly Mock<IDbTransaction> _dbTransactionMock;
    private readonly UserClaimDto _userClaimDto;
    private readonly CustomerService _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public CustomerServiceTests()
    {
        _customerRepoMock = new Mock<ICustomerRepo>();
        _dbTransactionMock = new Mock<IDbTransaction>();
        var loggerMock = new Mock<ILogger<CustomerService>>();
        loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
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
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var customers = new PagedList<Customer>( new List<Customer> { new() { CustomerId = 1, CustomerName = "Customer 1" }, new() { CustomerId = 2, CustomerName = "Customer 2" } }, 1, 10, 2 );

        _customerRepoMock
            .Setup(x => x.CustomerSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(customers);

        var result = await _sut.CustomerSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customers retrieved successfully.", result.Message);
        _customerRepoMock.Verify(x => x.CustomerSelect(It.IsAny<PagedListRequest>(), _ct), Times.Once);
    }

    [Fact]
    public async Task CustomerSelectById_WithValidId_ReturnsCustomer()
    {
        var customerId = 1;
        var customer = new Customer { CustomerId = customerId, CustomerName = "Test Customer" };

        _customerRepoMock
            .Setup(x => x.CustomerSelectById(customerId, _ct))
            .ReturnsAsync(customer);

        var result = await _sut.CustomerSelectById(customerId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer retrieved successfully.", result.Message);
        var data = result.Data;
        Assert.NotNull(data);
        Assert.Equal(customerId, data.CustomerId);
    }

    [Fact]
    public async Task CustomerInsert_WithValidCustomer_ReturnsInsertedId()
    {
        var customerVm = new CustomerVm { CustomerName = "New Customer", Npwp = "123456789" };
        var insertedId = 10;

        _customerRepoMock
            .Setup(x => x.CustomerInsert(It.IsAny<Customer>(), _ct))
            .ReturnsAsync(insertedId);

        var result = await _sut.CustomerInsert(customerVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer created successfully.", result.Message);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(insertedId, result.Data);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CustomerInsert_SetsCreatedByFromUserClaim()
    {
        var customerVm = new CustomerVm { CustomerName = "New Customer" };
        Customer? capturedCustomer = null;

        _customerRepoMock
            .Setup(x => x.CustomerInsert(It.IsAny<Customer>(), _ct))
            .Callback<Customer, CancellationToken>((c, _) => capturedCustomer = c)
            .ReturnsAsync(1);

        await _sut.CustomerInsert(customerVm, _ct);

        Assert.NotNull(capturedCustomer);
        Assert.Equal(_userClaimDto.Username, capturedCustomer.CreatedBy);
    }

    [Fact]
    public async Task CustomerUpdate_WithValidCustomer_CommitsTransaction()
    {
        var customerVm = new CustomerVm { CustomerId = 1, CustomerName = "Updated Customer" };

        _customerRepoMock
            .Setup(x => x.CustomerUpdate(It.IsAny<Customer>(), _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CustomerUpdate(customerVm, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer updated successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CustomerUpdate_SetsModifiedByFromUserClaim()
    {
        var customerVm = new CustomerVm { CustomerId = 1, CustomerName = "Updated Customer" };
        Customer? capturedCustomer = null;

        _customerRepoMock
            .Setup(x => x.CustomerUpdate(It.IsAny<Customer>(), _ct))
            .Callback<Customer, CancellationToken>((c, _) => capturedCustomer = c)
            .Returns(Task.CompletedTask);

        await _sut.CustomerUpdate(customerVm, _ct);

        Assert.NotNull(capturedCustomer);
        Assert.Equal(_userClaimDto.Username, capturedCustomer.ModifiedBy);
    }

    [Fact]
    public async Task CustomerDelete_WithValidId_CommitsTransaction()
    {
        var customerId = 1;

        _customerRepoMock
            .Setup(x => x.CustomerDelete(customerId, _ct))
            .Returns(Task.CompletedTask);

        var result = await _sut.CustomerDelete(customerId, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customer deleted successfully.", result.Message);
        _dbTransactionMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CustomerCbo_WithValidRequest_ReturnsCustomerList()
    {
        var request = new CboRequestVm { Term = "Test" };
        var customers = new List<Customer>
        {
            new() { CustomerId = 1, CustomerName = "Test Customer 1" },
            new() { CustomerId = 2, CustomerName = "Test Customer 2" }
        };

        _customerRepoMock
            .Setup(x => x.CustomerCbo(It.IsAny<CboRequest>(), _ct))
            .ReturnsAsync(customers);

        var result = await _sut.CustomerCbo(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.Equal("Customers retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task CustomerInsert_WhenRepoThrowsException_DoesNotCommitTransaction()
    {
        var customerVm = new CustomerVm { CustomerName = "New Customer" };

        _customerRepoMock
            .Setup(x => x.CustomerInsert(It.IsAny<Customer>(), _ct))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _sut.CustomerInsert(customerVm, _ct));

        _dbTransactionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Fact]
    public async Task CustomerSelect_WithEmptyResults_ReturnsSuccessWithEmptyList()
    {
        var request = new PagedListRequestVm { PageNumber = 1, PageSize = 10 };
        var customers = new PagedList<Customer>(
            new List<Customer>(),
            1,
            10,
            0
        );

        _customerRepoMock
            .Setup(x => x.CustomerSelect(It.IsAny<PagedListRequest>(), _ct))
            .ReturnsAsync(customers);

        var result = await _sut.CustomerSelect(request, _ct);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.TotalCount);
    }
}
