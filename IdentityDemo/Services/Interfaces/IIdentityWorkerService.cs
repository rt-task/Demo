using IdentityDemo.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityDemo.Services.Interfaces
{
    public interface IIdentityWorkerService
    {
        Task<IdentityResult> SignUp(SignUpRequest request);
        Task<(SignInResult Result, string Token)> SignIn(SignInRequest request);
        Task<IdentityResult> RequestResetPassword(RequestResetPasswordRequest request);
        Task<IdentityResult> ResetPassword(ResetPasswordRequest request);
        Task<IdentityResult> ChangePassword(ChangePasswordRequest request, int id);
    }
}
