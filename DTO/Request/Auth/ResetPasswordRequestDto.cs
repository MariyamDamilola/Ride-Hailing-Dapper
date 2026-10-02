namespace RideHailingApi_Dapper.DTO.Request.Auth;

public class ResetPasswordRequestDto
{
    public string? Email { get; set; }
    public string? Code { get; set; }
    public string? NewPassword { get; set; }
}