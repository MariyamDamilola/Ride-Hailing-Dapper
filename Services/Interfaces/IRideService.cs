using RideHailingApi_Dapper.DTO.Request.Ride;
using RideHailingApi_Dapper.DTO.Response;

namespace RideHailingApi_Dapper.Services.Interfaces;

public interface IRideService
{
    Task<ApiResponse> RequestRideAsync(int passengerId, CreateRideRequestDto request);

    Task<ApiResponse> GetMyRidesAsync(int passengerId);

    Task<ApiResponse> GetMyCurrentRideAsync(int passengerId);

    Task<ApiResponse> CancelRideAsync(int passengerId, int rideId, CancelRideRequestDto request);

    Task<ApiResponse> AcceptRideAsync(int driverProfileId, int rideId);

    Task<ApiResponse> UpdateRideStatusAsync(int driverProfileId, int rideId, UpdateRideStatusRequestDto request);
}