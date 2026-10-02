using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.DTO.Request.User;
using RideHailingApi_Dapper.DTO.Response;

namespace RideHailingApi_Dapper.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse> GetProfileAsync(int userId);

    Task<ApiResponse> DeactivateUserAsync(int userId, DeactivateUserRequestDto request);
}