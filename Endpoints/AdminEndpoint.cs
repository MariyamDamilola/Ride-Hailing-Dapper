using RideHailingApi_Dapper.DTO.Request.User;
using RideHailingApi_Dapper.Services.Interfaces;

namespace RideHailingApi_Dapper.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/admin")
            .WithTags("Admin");

        group.MapGet("/users", async (
            IAdminService adminService) =>
        {
            return await adminService.GetUsersAsync();
        });

        group.MapGet("/users/{userId:int}", async (
            int userId,
            IAdminService adminService) =>
        {
            return await adminService.GetUserByIdAsync(userId);
        });

        group.MapGet("/drivers/pending", async (
            IAdminService adminService) =>
        {
            return await adminService.GetPendingDriversAsync();
        });

        group.MapPut("/drivers/{driverProfileId:int}/approve", async (
            int driverProfileId,
            IAdminService adminService) =>
        {
            return await adminService.ApproveDriverAsync(
                driverProfileId);
        });

        group.MapPut("/drivers/{driverProfileId:int}/reject", async (
            int driverProfileId,
            RejectDriverRequestDto request,
            IAdminService adminService) =>
        {
            return await adminService.RejectDriverAsync(
                driverProfileId,
                request);
        });

        group.MapGet("/rides", async (
            IAdminService adminService) =>
        {
            return await adminService.GetRidesAsync();
        });

        group.MapGet("/audit-logs", async (
            IAdminService adminService) =>
        {
            return await adminService.GetAuditLogsAsync();
        });
    }
}