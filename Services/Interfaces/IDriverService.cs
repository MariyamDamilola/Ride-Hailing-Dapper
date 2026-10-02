using RideHailingApi_Dapper.DTO.Request.Ride;
using RideHailingApi_Dapper.DTO.Response;

namespace RideHailingApi_Dapper.Services.Interfaces;

public interface IDriverService
{
    Task<ApiResponse> GetProfileAsync(int driverProfileId);
    
    Task<int?> GetDriverProfileIdByUserIdAsync(int userId);

    Task<ApiResponse> GetAssignedRidesAsync(int driverProfileId);

    Task<ApiResponse> GetCurrentRideAsync(int driverProfileId);

    Task<ApiResponse> SetAvailabilityAsync(int driverProfileId, bool isAvailable);
    
    Task<ApiResponse> AddVehicleAsync(int driverProfileId, CreateVehicleRequestDto request);
}