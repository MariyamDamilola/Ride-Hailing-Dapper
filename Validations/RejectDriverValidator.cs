using FluentValidation;
using RideHailingApi_Dapper.DTO.Request.User;

namespace RideHailingApi_Dapper.Validations;

public class RejectDriverValidator : AbstractValidator<RejectDriverRequestDto>
{
    public RejectDriverValidator()
    {
        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("A rejection reason is required.")
            .MaximumLength(500);
    }
}