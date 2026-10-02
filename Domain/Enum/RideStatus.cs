namespace RideHailingApi_Dapper.Domain.Enum;

public enum RideStatus
{
    Requested,
    Accepted,
    DriverArriving,
    DriverArrived,
    InProgress,
    Completed,
    Cancelled
}