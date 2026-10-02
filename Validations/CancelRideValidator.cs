using FluentValidation;
using RideHailingApi_Dapper.DTO.Request.Ride;

namespace RideHailingApi_Dapper.Validations;

public class CancelRideValidator : AbstractValidator<CancelRideRequestDto>
{
    public CancelRideValidator()
    {
        RuleFor(x => x.CancellationReason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.CancellationReason));
    }

    
}