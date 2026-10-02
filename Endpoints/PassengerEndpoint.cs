using RideHailingApi_Dapper.DTO.Request.Ride;
using RideHailingApi_Dapper.Services.Interfaces;

namespace RideHailingApi_Dapper.Endpoints;

public static class PassengerEndpoints
{
    public static void MapPassengerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/passenger")
            .WithTags("Passenger");

        group.MapPost("/{passengerId:int}/rides", async (
            int passengerId,
            CreateRideRequestDto request,
            IRideService rideService) =>
        {
            return await rideService.RequestRideAsync(
                passengerId,
                request);
        });

        group.MapGet("/{passengerId:int}/rides", async (
            int passengerId,
            IRideService rideService) =>
        {
            return await rideService.GetMyRidesAsync(
                passengerId);
        });

        group.MapGet("/{passengerId:int}/rides/current", async (
            int passengerId,
            IRideService rideService) =>
        {
            return await rideService.GetMyCurrentRideAsync(
                passengerId);
        });

        group.MapPut("/{passengerId:int}/rides/{rideId:int}/cancel", async (
            int passengerId,
            int rideId,
            CancelRideRequestDto request,
            IRideService rideService) =>
        {
            return await rideService.CancelRideAsync(
                passengerId,
                rideId,
                request);
        });
    }
}