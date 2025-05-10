using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Extensions;
using MySchool.API.Middlewares;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AccountContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AccountService accountService) : BaseController
    {

        /// <summary>
        /// Login - Anonymous
        /// </summary>
        /// <param name="account">The account to login</param>
        /// <returns>The login response</returns>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto account)
        {
            return BuildResponse(await accountService.LoginAsync(account));
        }

        /// <summary>
        /// Change password - Authenticated
        /// </summary>
        /// <param name="request">The old and new password</param>
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [Authorize(Policy = Policies.AllUsers)]
        [AllowIfPasswordNeedToChange]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
        {
            return BuildResponse(await accountService.ChangePassword(request));
        }

        /// <summary>
        /// Logout - Authenticated
        /// </summary>
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.AllUsers)]
        [AllowIfPasswordNeedToChange]
        public async Task<IActionResult> Logout()
        {
            return BuildResponse(await accountService.Logout());
        }
    }
}
