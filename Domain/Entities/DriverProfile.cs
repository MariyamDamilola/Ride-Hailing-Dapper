using RideHailingApi_Dapper.Domain.Enum;

namespace RideHailingApi_Dapper.Domain.Entities;


public class DriverProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public bool IsAvailable { get; set; }
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
    public string? ApprovalReason { get; set; }
    public DateTime? ApprovedAt { get; set; } 
  

}
