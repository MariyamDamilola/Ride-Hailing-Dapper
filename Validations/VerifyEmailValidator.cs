using FluentValidation;
using RideHailingApi_Dapper.DTO.Request.Auth;

namespace RideHailingApi_Dapper.Validations;

public class VerifyEmailValidator : AbstractValidator<VerifyEmailRequestDto>
{
    public VerifyEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP code is required.")
            .Length(6).WithMessage("OTP code must be 6 digits.")
            .Matches(@"^\d{6}$").WithMessage("OTP code must contain only digits.");
    }

    
}