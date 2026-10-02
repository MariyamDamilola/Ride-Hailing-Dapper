using FluentValidation;
using RideHailingApi_Dapper.DTO.Request.Auth;

namespace RideHailingApi_Dapper.Validations;

public class ForgetPasswordValidator : AbstractValidator<ForgetPasswordRequestDto>
{
    public ForgetPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
    
}