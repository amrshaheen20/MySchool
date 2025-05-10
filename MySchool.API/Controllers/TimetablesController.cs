using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.TimeTableContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TimetablesController(TimeTableService timetableService) : BaseController
    {
        /// <summary>
        /// Create a new timetable - Admin Only
        /// </summary>
        /// <param name="timetable">The timetable to create</param>
        /// <returns>The created timetable</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TimeTableResponseDto))]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> Create([FromBody] TimeTableRequestDto timetable)
        {
            return BuildResponse(await timetableService.CreateTimetableAsync(timetable));
        }

        /// <summary>
        /// Get all timetables or a timetable by ID - For All
        /// </summary>
        /// <param name="timetable_id">Timetable ID</param>
        /// <param name="filter">Pagination filter</param>
        /// <returns>The timetables</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<TimeTableResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetTimeTables([FromQuery] int? timetable_id, [FromQuery] PaginationFilter<TimeTableResponseDto> filter)
        {
            if (timetable_id.HasValue)
                return BuildResponse(await timetableService.GetTimeTableByIdAsync(timetable_id.Value));

            return BuildResponse(timetableService.GetAllTimetables(filter));
        }

        /// <summary>
        /// Delete a timetable by ID - Admin Only
        /// </summary>
        /// <param name="timetable_id">The ID of the timetable to delete</param>
        /// <returns>The deleted timetable</returns>
        [HttpDelete("{timetable_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeTableResponseDto))]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> DeleteTimeTable(int timetable_id)
        {
            return BuildResponse(await timetableService.DeleteTimeTableByIdAsync(timetable_id));
        }

        /// <summary>
        /// Update a timetable by ID - Admin Only
        /// </summary>
        /// <param name="timetable_id">The ID of the timetable to update</param>
        /// <param name="timetable">The timetable data to update</param>
        /// <returns>The updated timetable</returns>
        [HttpPatch("{timetable_id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeTableResponseDto))]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> UpdateTimeTable(int timetable_id, [FromBody] TimeTableRequestDto timetable)
        {
            return BuildResponse(await timetableService.UpdateTimeTableAsync(timetable_id, timetable));
        }
    }
}
