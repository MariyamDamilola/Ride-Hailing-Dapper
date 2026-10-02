using RideHailingApi_Dapper.Domain.Enum;

namespace RideHailingApi_Dapper.Domain.Entities;

public class Ride
{
    public int Id { get; set; }
    public string? RideReference { get; set; }
    public int PassengerId { get; set; }
    public int? DriverProfileId { get; set; }
    public string? PickupAddress { get; set; }
    public string? DestinationAddress { get; set; }
    public RideStatus Status { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
   
}