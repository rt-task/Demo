using FluentValidation;
using IdentityDemo.Models;

namespace IdentityDemo.Validators
{
    public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpRequestValidator()
        {
            RuleFor(e => e.Username)
                .NotEmpty();

            RuleFor(e => e.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(e => e.Password)
                .NotEmpty()
                .MinimumLength(8);

            RuleFor(e => e.Description)
                .NotEmpty();

            RuleFor(e => e.Name)
                .NotEmpty();

            RuleFor(e => e.Image)
                .NotEmpty();
        }
    }
}
