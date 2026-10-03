using Dapper;
using RideHailingApi_Dapper.Data;
using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Repository.Interfaces;

namespace RideHailingApi_Dapper.Repository.Implementations;

public class DriverRepository : IDriverRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public DriverRepository(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<DriverProfile?> GetDriverProfileByUserIdAsync(int userId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM DriverProfiles
            WHERE UserId = @UserId
            """;

        return await connection.QuerySingleOrDefaultAsync<DriverProfile>(
            sql,
            new { UserId = userId });
    }

    public async Task<DriverProfile?> GetDriverProfileByIdAsync(int driverProfileId)
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

    public async Task<IEnumerable<Ride>> GetAssignedRidesAsync(
        int driverProfileId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE DriverProfileId = @DriverProfileId
            ORDER BY RequestedAt DESC
            """;

        return await connection.QueryAsync<Ride>(
            sql,
            new { DriverProfileId = driverProfileId });
    }

    public async Task<Ride?> GetCurrentRideAsync(int driverProfileId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE DriverProfileId = @DriverProfileId
              AND Status IN
              (
                  'Accepted',
                  'DriverArriving',
                  'DriverArrived',
                  'InProgress'
              )
            ORDER BY RequestedAt DESC
            """;

        return await connection.QuerySingleOrDefaultAsync<Ride>(
            sql,
            new { DriverProfileId = driverProfileId });
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

    public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Vehicles
            (
                DriverProfileId,
                Make,
                Model,
                Color,
                Year,
                LicensePlate
            )
            VALUES
            (
                @DriverProfileId,
                @Make,
                @Model,
                @Color,
                @Year,
                @LicensePlate
            );

            SELECT *
            FROM Vehicles
            WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<Vehicle>(
            sql,
            vehicle);
    }
    
}