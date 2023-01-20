using AutoMapper;
using IdentityDemo.Controllers;
using IdentityDemo.Dal;
using IdentityDemo.Dal.Entities;
using IdentityDemo.Identity.Context;
using IdentityDemo.Models;
using IdentityDemo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemo.Services.Concrete
{
    public class IdentityWorkerService : IIdentityWorkerService
    {
        private readonly AppDbContext _appCtx;
        private readonly AuthDbContext _authCtx;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public IdentityWorkerService(AppDbContext appCtx,
            SignInManager<IdentityUser<int>> signInManager,
            AuthDbContext authCtx,
            IMapper mapper,
            IEmailService emailService,
            ITokenService tokenService)
        {
            _appCtx = appCtx;
            _signInManager = signInManager;
            _authCtx = authCtx;
            _mapper = mapper;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task<IdentityResult> ChangePassword(ChangePasswordRequest request, int id)
        {
            var userEmail = (await _appCtx.Companies.FindAsync(id))!.Email;
            var user = await _signInManager.UserManager.FindByEmailAsync(userEmail);
            if (user == null)
                return IdentityResult.Failed();

            return await _signInManager.UserManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        }

        public async Task<IdentityResult> RequestResetPassword(RequestResetPasswordRequest request)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(request.Email);
            if (user == null)
                return IdentityResult.Failed();

            var token = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendResetPasswordTokenAsync(user.UserName, user.Email, token);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(request.Email);
            if (user == null)
                return IdentityResult.Failed();

            return await _signInManager.UserManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        }

        public async Task<(SignInResult, string)> SignIn(SignInRequest request)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(request.Username);
            if (user == null)
                return (SignInResult.Failed, string.Empty);

            var company = await _appCtx.Companies.SingleAsync(c => c.Email == user.Email);
            var role = (await _signInManager.UserManager.GetRolesAsync(user)).Single();

            return (await _signInManager.CheckPasswordSignInAsync(user, request.Password, false), 
                _tokenService.CreateToken(user.UserName, user.Email, company.Id.ToString(), role));
        }

        public async Task<IdentityResult> SignUp(SignUpRequest request)
        {
            var company = _mapper.Map<CompanyEntity>(request);
            var authInfo = _mapper.Map<IdentityUser<int>>(request);

            using var outerTransaction = await _appCtx.Database.BeginTransactionAsync();
            using var innerTransaction = await _authCtx.Database.BeginTransactionAsync();
            try
            {
                var appSignUpResult = await _appCtx.Companies.Add(company).Context.SaveChangesAsync();
                var authSignUpResult = await _signInManager.UserManager.CreateAsync(authInfo, request.Password);
                var addToRoleResult = IdentityResult.Failed();
                if (authSignUpResult.Succeeded)
                    addToRoleResult = await _signInManager.UserManager.AddToRoleAsync(authInfo, "User");

                if(!(appSignUpResult > 0 && authSignUpResult.Succeeded && addToRoleResult.Succeeded))
                {
                    await innerTransaction.RollbackAsync();
                    await outerTransaction.RollbackAsync();
                    return IdentityResult.Failed();
                }

                await innerTransaction.CommitAsync();
                await outerTransaction.CommitAsync();
                return IdentityResult.Success;
            }
            catch
            {
                await innerTransaction.RollbackAsync();
                await outerTransaction.RollbackAsync();
                throw;
            }
        }
    }
}
