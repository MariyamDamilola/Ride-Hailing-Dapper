using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Domain.Enum;
using RideHailingApi_Dapper.DTO.Request.Ride;
using RideHailingApi_Dapper.DTO.Response;
using RideHailingApi_Dapper.Helper;
using RideHailingApi_Dapper.Repository.Interfaces;
using RideHailingApi_Dapper.Services.Interfaces;

namespace RideHailingApi_Dapper.Services.Implementations;

public class RideService : IRideService
{
    private readonly IRideRepository _rideRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<RideService> _logger;

    public RideService(
        IRideRepository rideRepository,
        IUserRepository userRepository,
        IDriverRepository driverRepository,
        IAuditLogRepository auditLogRepository,
        IEmailService emailService,
        ISmsService smsService,
        ILogger<RideService> logger)
    {
        _rideRepository = rideRepository;
        _userRepository = userRepository;
        _driverRepository = driverRepository;
        _auditLogRepository = auditLogRepository;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<ApiResponse> RequestRideAsync(
        int passengerId,
        CreateRideRequestDto request)
    {
        try
        {
            var rideReference =
                $"RIDE-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

            var ride = new Ride
            {
                RideReference = rideReference,
                PassengerId = passengerId,
                PickupAddress = request.PickupAddress,
                DestinationAddress = request.DestinationAddress,
                Status = RideStatus.Requested,
                RequestedAt = DateTime.UtcNow
            };

            ride = await _rideRepository.CreateRideAsync(ride);

            _logger.LogInformation(
                $"Ride {ride.RideReference} successfully requested by passenger {passengerId}");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = passengerId,
                UserRole = UserRole.Passenger,
                Action = "Ride Requested",
                Status = AuditStatus.Success,
                EntityType = "Ride",
                EntityId = ride.Id,
                Description =
                    $"Passenger requested ride {ride.RideReference}",
                CreatedAt = DateTime.UtcNow
            });

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Ride requested successfully",
                Data = new
                {
                    ride.Id,
                    ride.RideReference,
                    ride.PickupAddress,
                    ride.DestinationAddress,
                    ride.Status,
                    ride.RequestedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while requesting ride for passenger {passengerId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while requesting ride"
            };
        }
    }

    public async Task<ApiResponse> GetMyRidesAsync(int passengerId)
    {
        try
        {
            var rides = await _rideRepository
                .GetPassengerRidesAsync(passengerId);

            _logger.LogInformation(
                $"Retrieved ride history for passenger {passengerId}");

            var result = new List<object>();

            foreach (var ride in rides)
            {
                object? driver = null;

                if (ride.DriverProfileId.HasValue)
                {
                    var driverProfile =
                        await _driverRepository.GetDriverProfileByIdAsync(
                            ride.DriverProfileId.Value);

                    if (driverProfile != null)
                    {
                        var driverUser =
                            await _userRepository.GetByIdAsync(
                                driverProfile.UserId);

                        if (driverUser != null)
                        {
                            driver = new
                            {
                                driverProfile.Id,
                                Name = driverUser.FullName,
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
                    Driver = driver
                });
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Ride history retrieved successfully",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving rides for passenger {passengerId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving ride history"
            };
        }
    }

    public async Task<ApiResponse> GetMyCurrentRideAsync(int passengerId)
    {
        try
        {
            var rides = await _rideRepository
                .GetPassengerRidesAsync(passengerId);

            var currentRide = rides.FirstOrDefault(ride =>
                ride.Status != RideStatus.Completed &&
                ride.Status != RideStatus.Cancelled);

            if (currentRide == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "No current ride found"
                };
            }

            object? driver = null;

            if (currentRide.DriverProfileId.HasValue)
            {
                var driverProfile =
                    await _driverRepository.GetDriverProfileByIdAsync(
                        currentRide.DriverProfileId.Value);

                if (driverProfile != null)
                {
                    var driverUser =
                        await _userRepository.GetByIdAsync(
                            driverProfile.UserId);

                    if (driverUser != null)
                    {
                        driver = new
                        {
                            driverProfile.Id,
                            Name = driverUser.FullName,
                            driverUser.PhoneNumber
                        };
                    }
                }
            }

            _logger.LogInformation(
                $"Retrieved current ride {currentRide.RideReference} for passenger {passengerId}");

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Current ride retrieved successfully",
                Data = new
                {
                    currentRide.Id,
                    currentRide.RideReference,
                    currentRide.PickupAddress,
                    currentRide.DestinationAddress,
                    currentRide.Status,
                    currentRide.RequestedAt,
                    Driver = driver
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving current ride for passenger {passengerId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while retrieving current ride"
            };
        }
    }

    public async Task<ApiResponse> CancelRideAsync(
        int passengerId,
        int rideId,
        CancelRideRequestDto request)
    {
        try
        {
            var ride = await _rideRepository.GetRideByIdAsync(rideId);

            if (ride == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Ride not found"
                };
            }

            if (ride.PassengerId != passengerId)
            {
                _logger.LogWarning(
                    $"Passenger {passengerId} attempted to cancel ride {rideId} belonging to another passenger");

                return new ApiResponse
                {
                    ResponseCode = "403",
                    ResponseMessage =
                        "You are not authorized to cancel this ride"
                };
            }

            if (ride.Status == RideStatus.Completed ||
                ride.Status == RideStatus.Cancelled ||
                ride.Status == RideStatus.InProgress ||
                ride.Status == RideStatus.DriverArrived)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage =
                        $"Ride cannot be cancelled when it is {ride.Status}"
                };
            }

            ride.Status = RideStatus.Cancelled;
            ride.CancelledAt = DateTime.UtcNow;
            ride.CancellationReason = request.CancellationReason;

            await _rideRepository.UpdateRideAsync(ride);

            await _rideRepository.CreateStatusHistoryAsync(
                new RideStatusHistory
                {
                    RideId = ride.Id,
                    Status = RideStatus.Cancelled,
                    ChangedByUserId = passengerId,
                    CreatedAt = DateTime.UtcNow
                });

            _logger.LogInformation(
                $"Ride {ride.RideReference} cancelled by passenger {passengerId}");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = passengerId,
                UserRole = UserRole.Passenger,
                Action = "Ride Cancelled",
                Status = AuditStatus.Success,
                EntityType = "Ride",
                EntityId = ride.Id,
                Description =
                    $"Passenger cancelled ride {ride.RideReference}. Reason: {request.CancellationReason}",
                CreatedAt = DateTime.UtcNow
            });

            if (ride.DriverProfileId.HasValue)
            {
                var driverProfile =
                    await _driverRepository.GetDriverProfileByIdAsync(
                        ride.DriverProfileId.Value);

                if (driverProfile != null)
                {
                    var driverUser =
                        await _userRepository.GetByIdAsync(
                            driverProfile.UserId);

                    if (driverUser != null)
                    {
                        await _smsService.SendSmsAsync(
                            driverUser.PhoneNumber,
                            SmsUtils.GetRideRejectedDriverSms(
                                driverUser.FullName,
                                ride.RideReference!));
                    }
                }
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Ride cancelled successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while cancelling ride {rideId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while cancelling ride"
            };
        }
    }

    public async Task<ApiResponse> AcceptRideAsync(
        int driverProfileId,
        int rideId)
    {
        try
        {
            var ride = await _rideRepository.GetRideByIdAsync(rideId);

            if (ride == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Ride not found"
                };
            }

            if (ride.Status != RideStatus.Requested)
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage =
                        "Only requested rides can be accepted"
                };
            }

            var availableDrivers =
                await _rideRepository.GetAvailableDriversAsync();

            var driver = availableDrivers
                .FirstOrDefault(d => d.Id == driverProfileId);

            if (driver == null)
            {
                _logger.LogWarning(
                    $"Driver {driverProfileId} attempted to accept ride {rideId} but is not available");

                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage =
                        "Driver is not available or approved"
                };
            }

            ride.DriverProfileId = driverProfileId;
            ride.Status = RideStatus.Accepted;

            await _rideRepository.UpdateRideAsync(ride);

            await _rideRepository.CreateStatusHistoryAsync(
                new RideStatusHistory
                {
                    RideId = ride.Id,
                    Status = RideStatus.Accepted,
                    ChangedByUserId = driver.UserId,
                    CreatedAt = DateTime.UtcNow
                });

            _logger.LogInformation(
                $"Driver {driverProfileId} accepted ride {ride.RideReference}");

            var passenger =
                await _userRepository.GetByIdAsync(ride.PassengerId);

            var driverUser =
                await _userRepository.GetByIdAsync(driver.UserId);

            if (passenger != null && driverUser != null)
            {
                await _emailService.SendRideAcceptedEmailAsync(
                    passenger.Email,
                    passenger.FullName,
                    driverUser.FullName,
                    ride.RideReference!);

                await _smsService.SendSmsAsync(
                    passenger.PhoneNumber,
                    SmsUtils.GetRideAcceptedSms(
                        passenger.FullName,
                        driverUser.FullName,
                        ride.RideReference!));
            }

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Ride Accepted",
                Status = AuditStatus.Success,
                EntityType = "Ride",
                EntityId = ride.Id,
                Description =
                    $"Driver accepted ride {ride.RideReference}",
                CreatedAt = DateTime.UtcNow
            });

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Ride accepted successfully",
                Data = new
                {
                    ride.Id,
                    ride.RideReference,
                    ride.Status
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while accepting ride {rideId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while accepting ride"
            };
        }
    }

    public async Task<ApiResponse> UpdateRideStatusAsync(
        int driverProfileId,
        int rideId,
        UpdateRideStatusRequestDto request)
    {
        try
        {
            var ride = await _rideRepository.GetRideByIdAsync(rideId);

            if (ride == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Ride not found"
                };
            }

            if (ride.DriverProfileId != driverProfileId)
            {
                _logger.LogWarning(
                    $"Driver {driverProfileId} attempted to update ride {rideId} assigned to another driver");

                return new ApiResponse
                {
                    ResponseCode = "403",
                    ResponseMessage =
                        "You are not authorized to update this ride"
                };
            }

            if (!Enum.TryParse<RideStatus>(
                    request.Status,
                    true,
                    out var newStatus))
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage = "Invalid ride status"
                };
            }

            if (!IsValidTransition(ride.Status, newStatus))
            {
                return new ApiResponse
                {
                    ResponseCode = "400",
                    ResponseMessage =
                        $"Invalid ride status transition from {ride.Status} to {newStatus}"
                };
            }

            ride.Status = newStatus;

            if (newStatus == RideStatus.Completed)
            {
                ride.CompletedAt = DateTime.UtcNow;
            }

            await _rideRepository.UpdateRideAsync(ride);

            var driverProfile =
                await _driverRepository.GetDriverProfileByIdAsync(
                    driverProfileId);

            if (driverProfile == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            await _rideRepository.CreateStatusHistoryAsync(
                new RideStatusHistory
                {
                    RideId = ride.Id,
                    Status = newStatus,
                    ChangedByUserId = driverProfile.UserId,
                    CreatedAt = DateTime.UtcNow
                });

            _logger.LogInformation(
                $"Ride {ride.RideReference} status changed to {newStatus} by driver {driverProfileId}");

            var passenger =
                await _userRepository.GetByIdAsync(ride.PassengerId);

            var driver =
                await _userRepository.GetByIdAsync(driverProfile.UserId);

            if (passenger != null && driver != null)
            {
                switch (newStatus)
                {
                    case RideStatus.DriverArriving:

                        await _emailService.SendDriverArrivedEmailAsync(
                            passenger.Email,
                            passenger.FullName,
                            driver.FullName,
                            ride.RideReference!);

                        await _smsService.SendSmsAsync(
                            passenger.PhoneNumber,
                            SmsUtils.GetDriverArrivingSms(
                                passenger.FullName,
                                driver.FullName,
                                ride.RideReference!));

                        break;

                    case RideStatus.DriverArrived:

                        await _smsService.SendSmsAsync(
                            passenger.PhoneNumber,
                            SmsUtils.GetDriverArrivedSms(
                                passenger.FullName,
                                driver.FullName,
                                ride.RideReference!));

                        break;

                    case RideStatus.InProgress:

                        await _smsService.SendSmsAsync(
                            passenger.PhoneNumber,
                            SmsUtils.GetRideStartedSms(
                                passenger.FullName,
                                ride.RideReference!));

                        break;

                    case RideStatus.Completed:

                        await _emailService.SendRideCompletedEmailAsync(
                            passenger.Email,
                            passenger.FullName,
                            ride.RideReference!,
                            driver.FullName);

                        await _smsService.SendSmsAsync(
                            passenger.PhoneNumber,
                            SmsUtils.GetRideCompletedSms(
                                passenger.FullName,
                                ride.RideReference!));

                        break;
                }
            }

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driverProfile.UserId,
                UserRole = UserRole.Driver,
                Action = "Ride Status Updated",
                Status = AuditStatus.Success,
                EntityType = "Ride",
                EntityId = ride.Id,
                Description =
                    $"Ride {ride.RideReference} status changed to {newStatus}",
                CreatedAt = DateTime.UtcNow
            });

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Ride status updated successfully",
                Data = new
                {
                    ride.Id,
                    ride.RideReference,
                    ride.Status,
                    ride.CompletedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while updating status for ride {rideId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while updating ride status"
            };
        }
    }

    private bool IsValidTransition(
        RideStatus currentStatus,
        RideStatus newStatus)
    {
        return currentStatus switch
        {
            RideStatus.Accepted =>
                newStatus == RideStatus.DriverArriving,

            RideStatus.DriverArriving =>
                newStatus == RideStatus.DriverArrived,

            RideStatus.DriverArrived =>
                newStatus == RideStatus.InProgress,

            RideStatus.InProgress =>
                newStatus == RideStatus.Completed,

            _ => false
        };
    }
}