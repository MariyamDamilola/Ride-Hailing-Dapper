using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Domain.Enum;
using RideHailingApi_Dapper.DTO.Request.User;
using RideHailingApi_Dapper.DTO.Response;
using RideHailingApi_Dapper.Repository.Interfaces;
using RideHailingApi_Dapper.Services.Interfaces;

namespace RideHailingApi_Dapper.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IAdminRepository adminRepository,
        IUserRepository userRepository,
        IDriverRepository driverRepository,
        IRideRepository rideRepository,
        IAuditLogRepository auditLogRepository,
        ILogger<AdminService> logger)
    {
        _adminRepository = adminRepository;
        _userRepository = userRepository;
        _driverRepository = driverRepository;
        _rideRepository = rideRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<ApiResponse> GetUsersAsync()
    {
        try
        {
            var users = await _adminRepository.GetUsersAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Users retrieved successfully",
                Data = users.Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.IsEmailVerified,
                    user.IsPhoneNumberVerified,
                    user.IsActive,
                    user.CreatedAt
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving users");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving users"
            };
        }
    }

    public async Task<ApiResponse> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "User not found"
                };
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "User retrieved successfully",
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
                $"Error occurred while retrieving user {userId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving user"
            };
        }
    }

    public async Task<ApiResponse> GetPendingDriversAsync()
    {
        try
        {
            var drivers =
                await _adminRepository.GetPendingDriversAsync();

            var result = new List<object>();

            foreach (var driver in drivers)
            {
                var user =
                    await _userRepository.GetByIdAsync(driver.UserId);

                result.Add(new
                {
                    driver.Id,
                    driver.Status,
                    driver.ApprovalReason,
                    User = user == null
                        ? null
                        : new
                        {
                            user.Id,
                            user.FullName,
                            user.Email,
                            user.PhoneNumber
                        }
                });
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage =
                    "Pending drivers retrieved successfully",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving pending drivers");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while retrieving pending drivers"
            };
        }
    }

    public async Task<ApiResponse> ApproveDriverAsync(
        int driverProfileId)
    {
        try
        {
            var driver =
                await _adminRepository
                    .GetDriverProfileByIdAsync(driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            if (driver.Status == ApprovalStatus.Approved)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Driver is already approved"
                };
            }

            var user =
                await _userRepository.GetByIdAsync(driver.UserId);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver user not found"
                };
            }

            driver.Status = ApprovalStatus.Approved;
            driver.ApprovalReason = null;
            driver.ApprovedAt = DateTime.UtcNow;

            await _adminRepository.UpdateDriverProfileAsync(driver);

            _logger.LogInformation(
                $"Driver {driverProfileId} approved successfully");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Driver Approved",
                Status = AuditStatus.Success,
                EntityType = "DriverProfile",
                EntityId = driver.Id,
                Description =
                    $"Driver profile {driver.Id} was approved",
                CreatedAt = DateTime.UtcNow
            });

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Driver approved successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while approving driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while approving driver"
            };
        }
    }

    public async Task<ApiResponse> RejectDriverAsync(
        int driverProfileId,
        RejectDriverRequestDto request)
    {
        try
        {
            var driver =
                await _adminRepository
                    .GetDriverProfileByIdAsync(driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            if (driver.Status == ApprovalStatus.Rejected)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Driver is already rejected"
                };
            }

            var user =
                await _userRepository.GetByIdAsync(driver.UserId);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver user not found"
                };
            }

            driver.Status = ApprovalStatus.Rejected;
            driver.ApprovalReason = request.RejectionReason;
            driver.ApprovedAt = null;

            await _adminRepository.UpdateDriverProfileAsync(driver);

            _logger.LogInformation(
                $"Driver {driverProfileId} rejected successfully");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Driver Rejected",
                Status = AuditStatus.Success,
                EntityType = "DriverProfile",
                EntityId = driver.Id,
                Description =
                    $"Driver profile {driver.Id} was rejected. " +
                    $"Reason: {request.RejectionReason}",
                CreatedAt = DateTime.UtcNow
            });

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Driver rejected successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while rejecting driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while rejecting driver"
            };
        }
    }

    public async Task<ApiResponse> GetRidesAsync()
    {
        try
        {
            var rides = await _adminRepository.GetRidesAsync();

            var result = new List<object>();

            foreach (var ride in rides)
            {
                var passenger =
                    await _userRepository.GetByIdAsync(ride.PassengerId);

                object? driver = null;

                if (ride.DriverProfileId.HasValue)
                {
                    var driverProfile =
                        await _driverRepository
                            .GetDriverProfileByIdAsync(
                                ride.DriverProfileId.Value);

                    if (driverProfile != null)
                    {
                        var driverUser =
                            await _userRepository
                                .GetByIdAsync(driverProfile.UserId);

                        if (driverUser != null)
                        {
                            driver = new
                            {
                                driverProfile.Id,
                                Name = driverUser.FullName,
                                PhoneNumber =
                                    driverUser.PhoneNumber
                            };
                        }
                    }
                }

                result.Add(new
                {
                    ride.Id,
                    ride.RideReference,
                    ride.PickupAddress,
                    ride.DestinationAddress,
                    ride.Status,
                    ride.RequestedAt,
                    ride.CompletedAt,
                    ride.CancelledAt,
                    ride.CancellationReason,

                    Passenger = passenger == null
                        ? null
                        : new
                        {
                            passenger.Id,
                            passenger.FullName,
                            passenger.Email,
                            passenger.PhoneNumber
                        },

                    Driver = driver
                });
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Rides retrieved successfully",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving rides");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while retrieving rides"
            };
        }
    }

    public async Task<ApiResponse> GetAuditLogsAsync()
    {
        try
        {
            var logs = await _adminRepository.GetAuditLogsAsync();

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage =
                    "Audit logs retrieved successfully",
                Data = logs.Select(log => new
                {
                    log.Id,
                    log.UserId,
                    log.UserRole,
                    log.Action,
                    log.Status,
                    log.EntityType,
                    log.EntityId,
                    log.Description,
                    log.CreatedAt
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving audit logs");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while retrieving audit logs"
            };
        }
    }
}   