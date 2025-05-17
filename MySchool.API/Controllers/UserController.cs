using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AccountContainer;

namespace MySchool.API.Controllers
{

    [Route("api/me")]
    [ApiController]
    [Authorize(Policy = Policies.AllUsers)]
    [ApiExplorerSettings(GroupName = "@me")]
    public class UserController(AccountService accountService) : BaseController
    {
        /// <summary>
        /// Get the current account - For All
        /// </summary>
        /// <returns>The current account</returns>
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountAdminResponseDto))]
        public IActionResult GetCurrentAccount() => BuildResponse(accountService.GetAccountByAuthAsync());

    }
}
