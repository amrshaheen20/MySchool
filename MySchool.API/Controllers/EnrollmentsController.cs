using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.EnrollmentContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController(EnrollmentService enrollmentService) : BaseController
    {
        /// <summary>
        /// Enroll a student in a class - Admin only
        /// </summary>
        /// <param name="request">Enrollment data (studentId and classId)</param>
        /// <returns>Status of the enrollment operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> EnrollStudent([FromBody] EnrollmentRequestDto request)
        {
            return BuildResponse(await enrollmentService.EnrollStudentAsync(request));
        }

        /// <summary>
        /// Get all students enrolled in a all classes that the user is allowed to see - For All
        /// </summary>
        /// <param name="filter">Pagination and filtering options</param>
        /// <returns>Paginated list of enrolled students</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<EnrollmentResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public IActionResult GetClassStudents([FromQuery] PaginationFilter<EnrollmentResponseDto> filter)
        {
            return BuildResponse(enrollmentService.GetAllEnrollmentssAsync(filter));
        }

        /// <summary>
        /// Remove enrollment - Admin only
        /// </summary>
        /// <param name="enrollment_id">The enrollment record ID</param>
        /// <returns>Status of the unenrollment operation</returns>
        [HttpDelete("{enrollment_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> RemoveEnrollment(int enrollment_id)
        {
            return BuildResponse(await enrollmentService.RemoveEnrollmentAsync(enrollment_id));
        }
    }
}
