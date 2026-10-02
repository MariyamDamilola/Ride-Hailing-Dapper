namespace RideHailingApi_Dapper.DTO.Request.Auth;

public class VerifyEmailRequestDto
{
    public string? Email { get; set; }
    public string? Code { get; set; }
}