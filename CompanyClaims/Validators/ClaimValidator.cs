using CompanyClaims.Mappers;
using FluentValidation;

namespace CompanyClaims.Validators;

public class ClaimValidator : AbstractValidator<ClaimDto>
{
    public ClaimValidator()
    {
        RuleFor(clm => clm.Ucr).MaximumLength(20).WithMessage("UCR is too long.  Must be less than 20 characters");
        RuleFor(clm => clm.Ucr).NotEmpty().WithMessage("UCR must be provided.");
        RuleFor(clm => clm.CompanyId).NotEmpty().WithMessage("A company Id is required.");
        RuleFor(clm => clm.IncurredLoss).GreaterThanOrEqualTo(0).WithMessage("Incurred loss can't be less than zero.");
        RuleFor(clm => clm.LossDate).GreaterThan(new DateTime(2015, 1, 1)).WithMessage("Loss date must be after 1/1/2015.")
            .LessThanOrEqualTo(c => c.ClaimDate).WithMessage("Loss date must be before or on the claim date");
    }
}