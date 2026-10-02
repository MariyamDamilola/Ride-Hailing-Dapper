using RideHailingApi_Dapper.DTO.Request.Ride;
using RideHailingApi_Dapper.DTO.Request.User;
using RideHailingApi_Dapper.Services.Interfaces;

namespace RideHailingApi_Dapper.Endpoints;

public static class DriverEndpoints
{
    public static void MapDriverEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/driver")
            .WithTags("Driver");

        group.MapGet("/{driverProfileId:int}/profile", async (
            int driverProfileId,
            IDriverService driverService) =>
        {
            return await driverService.GetProfileAsync(
                driverProfileId);
        });

        group.MapGet("/{driverProfileId:int}/rides", async (
            int driverProfileId,
            IDriverService driverService) =>
        {
            return await driverService.GetAssignedRidesAsync(
                driverProfileId);
        });

        group.MapGet("/{driverProfileId:int}/current-ride", async (
            int driverProfileId,
            IDriverService driverService) =>
        {
            return await driverService.GetCurrentRideAsync(
                driverProfileId);
        });

        group.MapPut("/{driverProfileId:int}/availability", async (
            int driverProfileId,
            SetAvailabilityRequestDto request,
            IDriverService driverService) =>
        {
            return await driverService.SetAvailabilityAsync(
                driverProfileId,
                request.IsAvailable);
        });

        group.MapPost("/{driverProfileId:int}/vehicle", async (
            int driverProfileId,
            CreateVehicleRequestDto request,
            IDriverService driverService) =>
        {
            return await driverService.AddVehicleAsync(
                driverProfileId,
                request);
        });
    }
}