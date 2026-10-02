using FluentValidation;
using RideHailingApi_Dapper.DTO.Request.User;

namespace RideHailingApi_Dapper.Validations;

public class DeactivateUserValidator : AbstractValidator<DeactivateUserRequestDto>
{
    public DeactivateUserValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("A reason is required to deactivate a user.")
            .MaximumLength(500);
    }

}

