using RideHailingApi_Dapper.DTO.Request.Auth;
using RideHailingApi_Dapper.DTO.Request.User;
using RideHailingApi_Dapper.Services.Interfaces;

namespace RideHailingApi_Dapper.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/register", async (
            RegisterRequestDto request,
            IAuthService authService) =>
        {
            return await authService.RegisterAsync(request);
        });

        group.MapPost("/verify-email", async (
            VerifyEmailRequestDto request,
            IAuthService authService) =>
        {
            return await authService.VerifyEmailAsync(request);
        });

        group.MapPost("/verify-phone", async (
            VerifyPhoneRequestDto request,
            IAuthService authService) =>
        {
            return await authService.VerifyPhoneAsync(request);
        });

        group.MapPost("/login", async (
            LoginRequestDto request,
            IAuthService authService) =>
        {
            return await authService.LoginAsync(request);
        });

        group.MapPost("/forgot-password", async (
            ForgetPasswordRequestDto request,
            IAuthService authService) =>
        {
            return await authService.ForgotPasswordAsync(request);
        });

        group.MapPost("/reset-password", async (
            ResetPasswordRequestDto request,
            IAuthService authService) =>
        {
            return await authService.ResetPasswordAsync(request);
        });

        group.MapPost("/change-password/{userId:int}", async (
            int userId,
            ChangePasswordRequestDto request,
            IAuthService authService) =>
        {
            return await authService.ChangePasswordAsync(
                userId,
                request);
        });

        group.MapPost("/resend-email-otp", async (
            string email,
            IAuthService authService) =>
        {
            return await authService.ResendEmailOtpAsync(email);
        });

        group.MapPost("/resend-phone-otp", async (
            string phoneNumber,
            IAuthService authService) =>
        {
            return await authService.ResendPhoneOtpAsync(phoneNumber);
        });

        group.MapPost("/resend-password-reset-otp", async (
            string email,
            IAuthService authService) =>
        {
            return await authService.ResendPasswordResetOtpAsync(email);
        });
    }
}