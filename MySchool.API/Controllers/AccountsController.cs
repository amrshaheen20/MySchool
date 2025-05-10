using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AccountContainer;

namespace MySchool.API.Controllers
{
    /// <summary>
    /// For Admin Only
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.Admin)]
    public class AccountsController(AccountService accountService) : BaseController
    {
        /// <summary>          
        /// Create a new account - Admin Only      
        /// </summary>
        /// <param name="account">The account to create</param>
        /// <returns>The created account</returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountAdminResponseDto))]
        public async Task<IActionResult> Create([FromBody] AccountRequestDto account)
        {
            return BuildResponse(await accountService.CreateAccountAsync(account));
        }

        /// <summary>
        /// Get all accounts or an account by ID - Admin Only
        /// </summary>
        /// <param name="Id">The ID of the account to get</param>
        /// <param name="filter">The filter to apply to the accounts</param>
        /// <returns>The accounts</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<AccountAdminResponseDto>))]
        public async Task<IActionResult> GetUsers([FromQuery] int? Id, [FromQuery] PaginationFilter<AccountAdminResponseDto> filter)
        {
            if (Id != null)
            {
                return BuildResponse(await accountService.GetAccountByIdAsync(Id.Value));
            }
            return BuildResponse(accountService.GetAllAccounts(filter));
        }

        /// <summary>
        /// Delete an account by ID - Admin Only
        /// </summary>
        /// <param name="account_id">The ID of the account to delete</param>
        /// <returns>The deleted account</returns>
        [HttpDelete("{account_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountAdminResponseDto))]
        public async Task<IActionResult> DeleteUser(int account_id)
        {
            return BuildResponse(await accountService.DeleteAccountByIdAsync(account_id));
        }

        /// <summary>
        /// Update an account by ID - Admin Only
        /// </summary>
        /// <param name="account_id">The ID of the account to update</param>
        /// <param name="account">The account to update</param>
        /// <returns>The updated account</returns>
        [HttpPatch("{account_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountAdminResponseDto))]
        public async Task<IActionResult> UpdateUser(int account_id, AccountRequestDto account)
        {
            return BuildResponse(await accountService.UpdateAccountAsync(account_id, account));
        }
    }
}
