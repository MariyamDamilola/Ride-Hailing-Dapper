using FluentValidation;
using RideHailingApi_Dapper.DTO.Request.Ride;

namespace RideHailingApi_Dapper.Validations;

public class UpdateRideValidator : AbstractValidator<UpdateRideStatusRequestDto>
{
    private static readonly string[] AllowedStatuses =
        { "DriverArriving", "DriverArrived", "InProgress", "Completed" };

    public UpdateRideValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => AllowedStatuses.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}.");
    }

}