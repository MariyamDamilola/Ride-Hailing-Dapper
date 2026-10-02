using RideHailingApi_Dapper.Domain.Entities;
using RideHailingApi_Dapper.Domain.Enum;
using RideHailingApi_Dapper.DTO.Request.Ride;
using RideHailingApi_Dapper.DTO.Response;
using RideHailingApi_Dapper.Repository.Interfaces;
using RideHailingApi_Dapper.Services.Interfaces;

namespace RideHailingApi_Dapper.Services.Implementations;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ILogger<DriverService> _logger;

    public DriverService(
        IDriverRepository driverRepository,
        IUserRepository userRepository,
        IRideRepository rideRepository,
        IAuditLogRepository auditLogRepository,
        ILogger<DriverService> logger)
    {
        _driverRepository = driverRepository;
        _userRepository = userRepository;
        _rideRepository = rideRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<ApiResponse> GetProfileAsync(int driverProfileId)
    {
        try
        {
            var driver = await _driverRepository
                .GetDriverProfileByIdAsync(driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            var user = await _userRepository.GetByIdAsync(driver.UserId);

            if (user == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver user not found"
                };
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Driver profile retrieved successfully",
                Data = new
                {
                    DriverProfileId = driver.Id,
                    driver.Status,
                    driver.IsAvailable,
                    driver.ApprovalReason,
                    driver.ApprovedAt,

                    User = new
                    {
                        user.Id,
                        user.FullName,
                        user.Email,
                        user.PhoneNumber
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving driver profile {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while retrieving driver profile"
            };
        }
    }

    public async Task<ApiResponse> GetAssignedRidesAsync(
        int driverProfileId)
    {
        try
        {
            var rides = await _driverRepository
                .GetAssignedRidesAsync(driverProfileId);

            var result = new List<object>();

            foreach (var ride in rides)
            {
                var passenger =
                    await _userRepository.GetByIdAsync(ride.PassengerId);

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
                            passenger.PhoneNumber
                        }
                });
            }

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Assigned rides retrieved successfully",
                Data = result
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving assigned rides for driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while retrieving assigned rides"
            };
        }
    }

    public async Task<ApiResponse> GetCurrentRideAsync(
        int driverProfileId)
    {
        try
        {
            var ride = await _driverRepository
                .GetCurrentRideAsync(driverProfileId);

            if (ride == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "No current ride found"
                };
            }

            var passenger =
                await _userRepository.GetByIdAsync(ride.PassengerId);

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage = "Current ride retrieved successfully",
                Data = new
                {
                    ride.Id,
                    ride.RideReference,
                    ride.PickupAddress,
                    ride.DestinationAddress,
                    ride.Status,
                    ride.RequestedAt,

                    Passenger = passenger == null
                        ? null
                        : new
                        {
                            passenger.Id,
                            passenger.FullName,
                            passenger.PhoneNumber
                        }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while retrieving current ride for driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while retrieving current ride"
            };
        }
    }

    public async Task<ApiResponse> SetAvailabilityAsync(
        int driverProfileId,
        bool isAvailable)
    {
        try
        {
            var driver = await _driverRepository
                .GetDriverProfileByIdAsync(driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            if (driver.Status != ApprovalStatus.Approved)
            {
                return new ApiResponse
                {
                    ResponseCode = "403",
                    ResponseMessage = "Driver is not approved"
                };
            }

            driver.IsAvailable = isAvailable;

            await _driverRepository.UpdateDriverProfileAsync(driver);

            _logger.LogInformation(
                $"Driver {driverProfileId} availability changed to {isAvailable}");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Driver Availability Updated",
                Status = AuditStatus.Success,
                EntityType = "DriverProfile",
                EntityId = driver.Id,
                Description =
                    $"Driver availability changed to {isAvailable}",
                CreatedAt = DateTime.UtcNow
            });

            return new ApiResponse
            {
                ResponseCode = "200",
                ResponseMessage =
                    "Driver availability updated successfully",
                Data = new
                {
                    driver.Id,
                    driver.IsAvailable
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while updating availability for driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage =
                    "Error occurred while updating driver availability"
            };
        }
    }

    public async Task<int?> GetDriverProfileIdByUserIdAsync(int userId)
    {
        var driver =
            await _driverRepository.GetDriverProfileByUserIdAsync(userId);

        return driver?.Id;
    }

    public async Task<ApiResponse> AddVehicleAsync(
        int driverProfileId,
        CreateVehicleRequestDto request)
    {
        try
        {
            var driver = await _driverRepository
                .GetDriverProfileByIdAsync(driverProfileId);

            if (driver == null)
            {
                return new ApiResponse
                {
                    ResponseCode = "404",
                    ResponseMessage = "Driver profile not found"
                };
            }

            if (driver.Status != ApprovalStatus.Approved)
            {
                return new ApiResponse
                {
                    ResponseCode = "403",
                    ResponseMessage = "Driver is not approved"
                };
            }

            var vehicle = new Vehicle
            {
                DriverProfileId = driverProfileId,
                Make = request.Make!,
                Model = request.Model!,
                Color = request.Color!,
                Year = request.Year,
                LicensePlate = request.LicensePlate
            };

            vehicle = await _driverRepository.AddVehicleAsync(vehicle);

            _logger.LogInformation(
                $"Vehicle added for driver profile {driverProfileId}");

            await _auditLogRepository.CreateAsync(new AuditLog
            {
                UserId = driver.UserId,
                UserRole = UserRole.Driver,
                Action = "Vehicle Added",
                Status = AuditStatus.Success,
                EntityType = "Vehicle",
                EntityId = vehicle.Id,
                Description = "Driver added vehicle details",
                CreatedAt = DateTime.UtcNow
            });

            return new ApiResponse
            {
                ResponseCode = "201",
                ResponseMessage = "Vehicle added successfully",
                Data = new
                {
                    vehicle.Id,
                    vehicle.Make,
                    vehicle.Model,
                    vehicle.Color,
                    vehicle.Year,
                    vehicle.LicensePlate
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Error occurred while adding vehicle for driver {driverProfileId}");

            return new ApiResponse
            {
                ResponseCode = "500",
                ResponseMessage = "Error occurred while adding vehicle"
            };
        }
    }
}