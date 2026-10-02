using RideHailingApi_Dapper.Domain.Entities;

namespace RideHailingApi_Dapper.Repository.Interfaces;

public interface IAdminRepository
{
    Task<IEnumerable<User>> GetUsersAsync();

    Task<User?> GetUserByIdAsync(int userId);

    Task<IEnumerable<DriverProfile>> GetPendingDriversAsync();

    Task<DriverProfile?> GetDriverProfileByIdAsync(int driverProfileId);

    Task<IEnumerable<Ride>> GetRidesAsync();

    Task<IEnumerable<AuditLog>> GetAuditLogsAsync();

    Task UpdateDriverProfileAsync(DriverProfile driverProfile);
}