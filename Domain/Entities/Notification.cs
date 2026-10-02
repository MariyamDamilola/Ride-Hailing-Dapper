using RideHailingApi_Dapper.Domain.Enum;

namespace RideHailingApi_Dapper.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? RideId { get; set; }
    public NotificationType Type { get; set; }
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string Message { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
  
}