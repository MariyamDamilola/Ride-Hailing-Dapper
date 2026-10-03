using RideHailingApi_Dapper.DTO.Request.Auth;
using RideHailingApi_Dapper.DTO.Request.User;
using RideHailingApi_Dapper.DTO.Response;

namespace RideHailingApi_Dapper.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse> RegisterAsync(RegisterRequestDto request);

    Task<ApiResponse> VerifyEmailAsync(VerifyEmailRequestDto request);

    Task<ApiResponse> VerifyPhoneAsync(VerifyPhoneRequestDto request);

    Task<ApiResponse> LoginAsync(LoginRequestDto request);

    Task<ApiResponse> ForgotPasswordAsync(ForgetPasswordRequestDto request);

    Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request);

    Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);

    Task<ApiResponse> ResendEmailOtpAsync(string email);

    Task<ApiResponse> ResendPhoneOtpAsync(string phoneNumber);

    Task<ApiResponse> ResendPasswordResetOtpAsync(string email);
}