using eCommerce.Core.DTO;
using FluentValidation;

namespace eCommerce.Core.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(temp => temp.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress();
            
            RuleFor(temp => temp.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8);
            
            RuleFor(temp => temp.PersonName)
                .NotEmpty().WithMessage("PersonName is required")
                .Length(1, 50);

            RuleFor(temp => temp.Gender)
                .IsInEnum().WithMessage("Not a valid gender");
        }
    }
}
