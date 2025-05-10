using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.FeeContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeesController(FeeService feeService) : BaseController
    {
        /// <summary>
        /// Create a new fee - Admin Only
        /// </summary>
        /// <param name="requestDto">The fee data to create.</param>
        /// <returns>The created fee.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FeeResponseDto))]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> CreateFee([FromBody] FeeRequestDto requestDto)
        {
            return BuildResponse(await feeService.CreateFeeAsync(requestDto));
        }


        /// <summary>
        /// Get all fees with optional pagination and filtering - For All
        /// </summary>
        /// <param name="id">The ID of the fee to retrieve.</param>
        /// <param name="filter">Pagination and filter parameters.</param>
        /// <returns>A paginated list of fees.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<FeeResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetAllFees([FromQuery] int? id, [FromQuery] PaginationFilter<FeeResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await feeService.GetFeeByIdAsync(id.Value));
            }

            return BuildResponse(feeService.GetAllFees(filter));
        }

        /// <summary>
        /// Update a specific fee by ID - Admin Only
        /// </summary>
        /// <param name="id">The ID of the fee to update.</param>
        /// <param name="requestDto">The new data for the fee.</param>
        /// <returns>Status of the update operation.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> UpdateFee(int id, [FromBody] FeeRequestDto requestDto)
        {
            return BuildResponse(await feeService.UpdateFeeAsync(id, requestDto));
        }

        /// <summary>
        /// Delete a specific fee by ID - Admin Only
        /// </summary>
        /// <param name="id">The ID of the fee to delete.</param>
        /// <returns>Status of the deletion.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> DeleteFee(int id)
        {
            return BuildResponse(await feeService.DeleteFeeAsync(id));
        }
    }
}
