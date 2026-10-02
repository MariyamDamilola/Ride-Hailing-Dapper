namespace RideHailingApi_Dapper.DTO.Request.Auth;

public class LoginRequestDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}