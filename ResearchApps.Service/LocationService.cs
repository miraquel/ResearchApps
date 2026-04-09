using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ResearchApps.Mapper;
using ResearchApps.Repo.Interface;
using ResearchApps.Service.Interface;
using ResearchApps.Service.Vm;
using ResearchApps.Service.Vm.Common;

namespace ResearchApps.Service;

public partial class LocationService : ILocationService
{
    private readonly ILocationRepo _locationRepo;
    private readonly IDbTransaction _dbTransaction;
    private readonly UserClaimDto _userClaimDto;
    private readonly ILogger<LocationService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public LocationService(ILocationRepo locationRepo, IDbTransaction dbTransaction, UserClaimDto userClaimDto, ILogger<LocationService> logger)
    {
        _locationRepo = locationRepo;
        _dbTransaction = dbTransaction;
        _userClaimDto = userClaimDto;
        _logger = logger;
    }

    public async Task<ServiceResponse<IEnumerable<LocationVm>>> CboAsync(CboRequestVm listRequest,
        CancellationToken cancellationToken)
    {
        var locations = await _locationRepo.LocationCboAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<IEnumerable<LocationVm>>.Success(_mapper.MapToVm(locations));
    }

    public async Task<ServiceResponse> DeleteAsync(int locationId, CancellationToken cancellationToken)
    {
        LogDeletingLocation(locationId, _userClaimDto.Username);
        await _locationRepo.LocationDeleteAsync(locationId, cancellationToken);
        _dbTransaction.Commit();
        LogLocationDeleted(locationId);
        return ServiceResponse.Success("Location deleted successfully.");
    }

    public async Task<ServiceResponse<LocationVm>> InsertAsync(LocationVm locationVm, CancellationToken cancellationToken)
    {
        LogCreatingLocation(locationVm.LocationName, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(locationVm);
        entity.CreatedBy = _userClaimDto.Username;
        var insertedLocation = await _locationRepo.LocationInsertAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogLocationCreated(insertedLocation.LocationId);
        return ServiceResponse<LocationVm>.Success(_mapper.MapToVm(insertedLocation), "Location inserted successfully.", StatusCodes.Status201Created);
    }

    public async Task<ServiceResponse<PagedListVm<LocationVm>>> SelectAsync(PagedListRequestVm listRequest, CancellationToken cancellationToken)
    {
        var locations = await _locationRepo.LocationSelectAsync(_mapper.MapToEntity(listRequest), cancellationToken);
        return ServiceResponse<PagedListVm<LocationVm>>.Success(_mapper.MapToVm(locations), "Locations retrieved successfully.");
    }

    public async Task<ServiceResponse<LocationVm>> SelectByIdAsync(int locationId, CancellationToken cancellationToken)
    {
        var location = await _locationRepo.LocationSelectByIdAsync(locationId, cancellationToken);
        return ServiceResponse<LocationVm>.Success(_mapper.MapToVm(location), "Location retrieved successfully.");
    }

    public async Task<ServiceResponse<LocationVm>> UpdateAsync(LocationVm locationVm, CancellationToken cancellationToken)
    {
        LogUpdatingLocation(locationVm.LocationId, _userClaimDto.Username);
        var entity = _mapper.MapToEntity(locationVm);
        entity.ModifiedBy = _userClaimDto.Username;
        var updatedLocation = await _locationRepo.LocationUpdateAsync(entity, cancellationToken);
        _dbTransaction.Commit();
        LogLocationUpdated(locationVm.LocationId);
        return ServiceResponse<LocationVm>.Success(_mapper.MapToVm(updatedLocation), "Location updated successfully.");
    }

    [LoggerMessage(LogLevel.Information, "Creating new Location: {locationName} by user: {username}")]
    partial void LogCreatingLocation(string locationName, string username);

    [LoggerMessage(LogLevel.Information, "Location created successfully with Id: {locationId}")]
    partial void LogLocationCreated(int locationId);

    [LoggerMessage(LogLevel.Information, "Updating Location {locationId} by user: {username}")]
    partial void LogUpdatingLocation(int locationId, string username);

    [LoggerMessage(LogLevel.Information, "Location {locationId} updated successfully")]
    partial void LogLocationUpdated(int locationId);

    [LoggerMessage(LogLevel.Information, "Deleting Location {locationId} by user: {username}")]
    partial void LogDeletingLocation(int locationId, string username);

    [LoggerMessage(LogLevel.Information, "Location {locationId} deleted successfully")]
    partial void LogLocationDeleted(int locationId);
}
