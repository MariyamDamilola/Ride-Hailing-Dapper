using RideHailingApi_Dapper.Domain.Entities;

namespace RideHailingApi_Dapper.Repository.Interfaces;

public interface IRideRepository
{
    Task<Ride> CreateRideAsync(Ride ride);
    Task<Ride?> GetRideByIdAsync(int rideId);
    Task<Ride?> GetRideByReferenceAsync(string rideReference);

    Task<IEnumerable<Ride>> GetPassengerRidesAsync(int passengerId);

    Task<IEnumerable<Ride>> GetDriverRidesAsync(int driverProfileId);

    Task<IEnumerable<DriverProfile>> GetAvailableDriversAsync();

    Task UpdateRideAsync(Ride ride);

    Task<RideStatusHistory> CreateStatusHistoryAsync(RideStatusHistory history);
    Task<IEnumerable<RideStatusHistory>> GetRideStatusHistoryAsync(int rideId);
}