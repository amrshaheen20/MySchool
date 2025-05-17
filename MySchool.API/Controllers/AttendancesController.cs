using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AttendanceContainer;

namespace MySchool.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController(AttendanceService attendanceService) : BaseController
    {
        /// <summary>
        /// Record attendance - Admin and Teacher Only
        /// </summary>
        /// <param name="requestDto">The attendance request</param>
        /// <returns>The created attendance</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AttendanceResponseDto))]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> RecordAttendance(AttendanceRequestDto requestDto)
        {
            return BuildResponse(await attendanceService.CreateAttendanceAsync(requestDto));
        }

        /// <summary>
        /// Get all attendance or an attendance by ID - For All
        /// </summary>
        /// <param name="id">Attendance ID</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <remarks>
        /// For Admin: Get all attendance records <br/>
        /// For Teacher: Get attendance that he recorded<br/>
        /// For Student: Get his attendance<br/>
        /// For Guardian: Get his child's attendance<br/>
        /// Other wise, return empty list<br/>
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<AttendanceResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetAllAttendance([FromQuery] int? id, [FromQuery] PaginationFilter<AttendanceResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await attendanceService.GetAttendanceByIdAsync(id.Value));
            }
            return BuildResponse(attendanceService.GetAllAttendances(filter));
        }

        /// <summary>
        /// Update attendance status - Admin and Teacher Only
        /// </summary>
        /// <param name="attendance_id">The ID of the attendance to update</param>
        /// <param name="requestDto">The attendance request</param>
        /// <returns>The updated attendance</returns>
        [HttpPatch("{attendance_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> UpdateAttendanceStatus(int attendance_id, AttendanceRequestDto requestDto)
        {
            return BuildResponse(await attendanceService.UpdateAttendanceAsync(attendance_id, requestDto));
        }

        /// <summary>
        /// Delete attendance - Admin and Teacher Only
        /// </summary>
        /// <param name="attendance_id">The ID of the attendance to delete</param>
        /// <returns>The deleted attendance</returns>
        [HttpDelete("{attendance_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> DeleteAttendance(int attendance_id)
        {
            return BuildResponse(await attendanceService.DeleteAttendanceAsync(attendance_id));
        }


        /// <summary>
        /// Get class attendance - Admin and Teacher Only
        /// </summary>
        /// <param name="request">Request parameters for the class attendance</param>
        /// <param name="filter">Pagination parameters for the attendance list</param>
        /// <returns>Paginated list of class attendance records</returns>
        [HttpGet("class-attendance")]
        [Authorize(Policy = Policies.Moderator)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<ClassAttendanceResponseDto>))]
        public IActionResult GetClassAttendance(
           [FromQuery] ClassAttendanceRequestDto request,

            [FromQuery] PaginationFilter<ClassAttendanceResponseDto> filter)
        {
            return BuildResponse(attendanceService.GetClassAttendanceById(request, filter));
        }


    }
}
