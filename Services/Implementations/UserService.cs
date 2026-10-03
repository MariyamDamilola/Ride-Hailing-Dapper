using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Domain.Enum;
using RideHailingApi_Dapper.DTO.Request.User;
using RideHailingApi_Dapper.DTO.Response;
using RideHailingApi_Dapper.Helper;
using RideHailingApi_Dapper.Repository.Interfaces;
using RideHailingApi_Dapper.Services.Interfaces;

namespace RideHailingApi_Dapper.Services.Implementations;

public class UserService : IUserService
{
     private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IEmailService emailService,
        ISmsService smsService,
        IAuditLogRepository auditLogRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _smsService = smsService;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<ApiResponse> GetProfileAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning(
                    $"Profile request failed: User {userId} not found");

                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "User not found"
                };
            }

            _logger.LogInformation(
                $"Profile retrieved successfully for user {userId}");

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Profile retrieved successfully",
                Data = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.IsEmailVerified,
                    user.IsPhoneNumberVerified,
                    user.IsActive,
                    user.CreatedAt,
                    user.UpdatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving profile for user {userId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving profile"
            };
        }
    }

    public async Task<ApiResponse> DeactivateUserAsync(
        int userId,
        DeactivateUserRequestDto request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning(
                    $"Account deactivation failed: User {userId} not found");

                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "User not found"
                };
            }

            if (!user.IsActive)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Account is already deactivated"
                };
            }

            var deactivated = await _userRepository.DeactivateAsync(userId);

            if (!deactivated)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Account could not be deactivated"
                };
            }

            _logger.LogInformation(
                $"User {userId} successfully deactivated their account");

            await _emailService.SendAccountDeactivatedEmailAsync(
                user.Email,
                user.FullName,
                request.Reason);

            await _smsService.SendSmsAsync(
                user.PhoneNumber,
                SmsUtils.GetAccountDeactivatedSms(
                    user.FullName,
                    user.Id));

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = user.Id,
                UserRole = user.Role,
                Action = "Account Deactivated",
                Status = AuditStatus.Success,
                EntityType = "User",
                EntityId = user.Id,
                Description =
                    $"User deactivated their account. Reason: {request.Reason}",
                CreatedAt = DateTime.UtcNow
            });

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Account deactivated successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while deactivating account for user {userId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while deactivating account"
            };
        }
    }
}