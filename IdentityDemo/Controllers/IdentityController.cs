using IdentityDemo.Models;
using IdentityDemo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IIdentityWorkerService _service;

        public IdentityController(ILogger<IdentityController> logger,
            IIdentityWorkerService service)
        {
            _logger = logger;
            _service = service;
        }
        
        [HttpPost("SignUp")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            var result = await _service.SignUp(request);

            return result.Succeeded ?
                Ok("Registration succeeded") :
                BadRequest("Registration failed");
        }

        [HttpPost("SignIn")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            var signInResult = await _service.SignIn(request);

            return signInResult.Result.Succeeded ?
                Ok(signInResult.Token) :
                Unauthorized();
        }

        [HttpPost("RequestResetPassword")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RequestResetPassword(RequestResetPasswordRequest request)
        {
            var result = await _service.RequestResetPassword(request);
            if (!result.Succeeded)
                _logger.LogWarning("Error: user not found while trying to reset password. Email: {0}", request.Email);
            return NoContent();
        }

        [HttpPost("ResetPassword")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _service.ResetPassword(request);

            return result.Succeeded ?
                Ok("Reset password succeeded") :
                BadRequest("Reset password failed");
        }

        [Authorize(Policy = "Authenticated")]
        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.ChangePassword(request, userId);

            return result.Succeeded ?
                Ok("Change password succeeded") :
                BadRequest("Change password failed");
        }
    }
}
