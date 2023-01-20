using FluentValidation;
using IdentityDemo.Models;

namespace IdentityDemo.Validators
{
    public class RequestResetPasswordRequestValidator : AbstractValidator<RequestResetPasswordRequest>
    {
        public RequestResetPasswordRequestValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
