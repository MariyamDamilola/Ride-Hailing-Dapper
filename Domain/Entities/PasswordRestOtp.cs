namespace RideHailingApi_Dapper.Domain.Entities;

public class PasswordResetOtp
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}