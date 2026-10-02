namespace RideHailingApi_Dapper.DTO.Request.Auth;

public class VerifyPhoneRequestDto
{
    public string? PhoneNumber { get; set; }
    public string? Code { get; set; }
}