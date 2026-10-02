namespace RideHailingApi_Dapper.DTO.Request.Ride;

public class CreateRideRequestDto
{
    public string? PickupAddress { get; set; }
    public string? DestinationAddress { get; set; }
}