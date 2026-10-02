using RideHailingApi_Dapper.DTO.Request.User;
using RideHailingApi_Dapper.DTO.Response;

namespace RideHailingApi_Dapper.Services.Interfaces;

public interface IAdminService
{
    Task<ApiResponse> GetUsersAsync();

    Task<ApiResponse> GetUserByIdAsync(int userId);

    Task<ApiResponse> GetPendingDriversAsync();

    Task<ApiResponse> ApproveDriverAsync(int driverProfileId);

    Task<ApiResponse> RejectDriverAsync(int driverProfileId, RejectDriverRequestDto request);

    Task<ApiResponse> GetRidesAsync();

    Task<ApiResponse> GetAuditLogsAsync();
}