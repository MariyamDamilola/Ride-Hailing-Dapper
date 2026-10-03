using Dapper;
using RideHailingApi_Dapper.Data;
using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Repository.Interfaces;

namespace RideHailingApi_Dapper.Repository.Implementations;

public class AdminRepository : IAdminRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public AdminRepository(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryAsync<User>(sql);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            WHERE Id = @UserId
            """;

        return await connection.QuerySingleOrDefaultAsync<User>(
            sql,
            new { UserId = userId });
    }

    public async Task<IEnumerable<DriverProfile>> GetPendingDriversAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM DriverProfiles
            WHERE Status = 'Pending'
            ORDER BY Id DESC
            """;

        return await connection.QueryAsync<DriverProfile>(sql);
    }

    public async Task<DriverProfile?> GetDriverProfileByIdAsync(
        int driverProfileId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM DriverProfiles
            WHERE Id = @DriverProfileId
            """;

        return await connection.QuerySingleOrDefaultAsync<DriverProfile>(
            sql,
            new { DriverProfileId = driverProfileId });
    }

    public async Task<IEnumerable<Ride>> GetRidesAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            ORDER BY RequestedAt DESC
            """;

        return await connection.QueryAsync<Ride>(sql);
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM AuditLogs
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryAsync<AuditLog>(sql);
    }

    public async Task UpdateDriverProfileAsync(
        DriverProfile driverProfile)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE DriverProfiles
            SET
                IsAvailable = @IsAvailable,
                Status = @Status,
                ApprovalReason = @ApprovalReason,
                ApprovedAt = @ApprovedAt
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, driverProfile);
    }
    
}