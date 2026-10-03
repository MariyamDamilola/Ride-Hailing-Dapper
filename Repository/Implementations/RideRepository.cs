using Dapper;
using RideHailingApi_Dapper.Data;
using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Repository.Interfaces;

public class RideRepository : IRideRepository
{
    private readonly DbConnectionFactory _dbConnectionFactory;

    public RideRepository(DbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<Ride> CreateRideAsync(Ride ride)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Rides
            (
                RideReference,
                PassengerId,
                DriverProfileId,
                PickupAddress,
                DestinationAddress,
                Status,
                RequestedAt
            )
            VALUES
            (
                @RideReference,
                @PassengerId,
                @DriverProfileId,
                @PickupAddress,
                @DestinationAddress,
                @Status,
                @RequestedAt
            );

            SELECT *
            FROM Rides
            WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<Ride>(
            sql,
            ride);
    }

    public async Task<Ride?> GetRideByIdAsync(int rideId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE Id = @RideId
            """;

        return await connection.QuerySingleOrDefaultAsync<Ride>(
            sql,
            new { RideId = rideId });
    }

    public async Task<Ride?> GetRideByReferenceAsync(string rideReference)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE RideReference = @RideReference
            """;

        return await connection.QuerySingleOrDefaultAsync<Ride>(
            sql,
            new { RideReference = rideReference });
    }

    public async Task<IEnumerable<Ride>> GetPassengerRidesAsync(int passengerId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE PassengerId = @PassengerId
            ORDER BY RequestedAt DESC
            """;

        return await connection.QueryAsync<Ride>(
            sql,
            new { PassengerId = passengerId });
    }

    public async Task<IEnumerable<Ride>> GetDriverRidesAsync(int driverProfileId)
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

    public async Task<IEnumerable<DriverProfile>> GetAvailableDriversAsync()
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM DriverProfiles
            WHERE IsAvailable = 1
            """;

        return await connection.QueryAsync<DriverProfile>(sql);
    }

    public async Task UpdateRideAsync(Ride ride)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            UPDATE Rides
            SET
                DriverProfileId = @DriverProfileId,
                Status = @Status,
                CompletedAt = @CompletedAt,
                CancelledAt = @CancelledAt,
                CancellationReason = @CancellationReason
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, ride);
    }

    public async Task<RideStatusHistory> CreateStatusHistoryAsync(
        RideStatusHistory history)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO RideStatusHistories
            (
                RideId,
                Status,
                ChangedByUserId,
                CreatedAt
            )
            VALUES
            (
                @RideId,
                @Status,
                @ChangedByUserId,
                @CreatedAt
            );

            SELECT *
            FROM RideStatusHistories
            WHERE Id = CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await connection.QuerySingleAsync<RideStatusHistory>(
            sql,
            history);
    }

    public async Task<IEnumerable<RideStatusHistory>> GetRideStatusHistoryAsync(
        int rideId)
    {
        using var connection = _dbConnectionFactory.CreateConnection();

        const string sql = """
            SELECT *
            FROM RideStatusHistories
            WHERE RideId = @RideId
            ORDER BY CreatedAt ASC
            """;

        return await connection.QueryAsync<RideStatusHistory>(
            sql,
            new { RideId = rideId });
    }
}