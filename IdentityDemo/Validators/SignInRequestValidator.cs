using FluentValidation;
using IdentityDemo.Models;

namespace IdentityDemo.Validators
{
    public class SignInRequestValidator : AbstractValidator<SignInRequest>
    {
        public SignInRequestValidator()
        {
            RuleFor(e => e.Username)
                .NotEmpty();

            RuleFor(e => e.Password)
                .NotEmpty();
        }
    }
}
