using FluentValidation;
using RideHailingApi_Dapper.DTO.Request.Auth;

namespace RideHailingApi_Dapper.Validations;

public class VerifyPhoneValidator : AbstractValidator<VerifyPhoneRequestDto>
{
    public VerifyPhoneValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^0[7-9]\d{9}$")
            .WithMessage("Phone number must be a valid Nigerian phone number, e.g. 08012345678.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP code is required.")
            .Length(6).WithMessage("OTP code must be 6 digits.")
            .Matches(@"^\d{6}$").WithMessage("OTP code must contain only digits.");
    }
}