namespace RideHailingApi_Dapper.DTO.Request.Auth;

public class ChangePasswordRequestDto
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}