using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.GuardianContainer;
using MySchool.API.Validators;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuardianController(GuardianService guardianService) : BaseController
    {
        /// <summary>
        /// Add a new child (student) under a guardian's account - Admin Only
        /// </summary>
        /// <param name="guardian_account_id">The account ID of the guardian</param>
        /// <param name="request"></param>
        /// <returns>Status of the operation</returns>
        [HttpPost("{guardian_account_id}/children")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> AddChildren([IsExists<User>(Role = eRole.Guardian)] int guardian_account_id, [FromBody] StudentGuardianRequestDto request)
        {
            return BuildResponse(await guardianService.AddChildAsync(guardian_account_id, request));
        }

        /// <summary>
        /// Get all children (students) under a guardian's account - For All  
        /// </summary>
        /// <param name="guardian_account_id">The account ID of the guardian</param>
        /// <param name="filter">Pagination and filtering options</param>
        /// <returns>List of children for the guardian</returns>
        /// <remarks>
        /// </remarks>
        [HttpGet("{guardian_account_id}/children")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<StudentGuardianResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public IActionResult GetChildren([IsExists<User>(Role = eRole.Guardian)] int guardian_account_id, [FromQuery] PaginationFilter<StudentGuardianResponseDto> filter)
        {
            return BuildResponse(guardianService.Getchildren(guardian_account_id, filter));
        }

        /// <summary>
        /// Get all children (students) under all guardians - Admin Only
        /// </summary>
        /// <param name="filter">Pagination and filtering options</param>
        /// <returns>List of children for all guardians</returns>
        [HttpGet("children")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<StudentGuardianResponseDto>))]
        [Authorize(Policy = Policies.Admin)]
        public IActionResult GetAllchildren([FromQuery] PaginationFilter<StudentGuardianResponseDto> filter)
        {
            return BuildResponse(guardianService.GetAllchildren(filter));
        }

        /// <summary>
        /// Remove a specific child (student) from a guardian - Admin Only
        /// </summary>
        /// <param name="guardian_account_id">The account ID of the guardian</param>
        /// <param name="student_id">The ID of the student to remove</param>
        /// <returns>Status of the operation</returns>
        [HttpDelete("{guardian_account_id}/children/{student_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> DeleteChild(int guardian_account_id, [FromRoute] int student_id)
        {
            return BuildResponse(await guardianService.DeleteChildAsync(guardian_account_id, student_id));
        }
    }

}
