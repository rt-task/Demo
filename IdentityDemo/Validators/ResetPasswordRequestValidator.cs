using FluentValidation;
using IdentityDemo.Models;

namespace IdentityDemo.Validators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(e => e.Token)
                .NotEmpty();

            RuleFor(e => e.NewPassword)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}
