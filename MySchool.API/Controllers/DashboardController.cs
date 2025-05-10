using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.DashboardContainer;

namespace MySchool.API.Controllers
{
    /// <summary>
    /// Provides dashboard data for different user roles in the school system.
    /// Each endpoint retrieves role-specific information and statistics tailored to the user type.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(DashboardService dashboardService) : BaseController
    {
        /// <summary>
        /// Retrieves dashboard information specific to a student - For All
        /// </summary>
        /// <param name="id">The unique identifier of the student</param>
        [HttpGet("student/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentDashboardResponseDto))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetStudentDashboard(int id)
            => BuildResponse(await dashboardService.GetStudentDashboardAsync(id));

        /// <summary>
        /// Retrieves dashboard information specific to a teacher - For All
        /// </summary>
        /// <param name="id">The unique identifier of the teacher</param>
        [HttpGet("teacher/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeacherDashboardResponseDto))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetTeacherDashboard(int id)
            => BuildResponse(await dashboardService.GetTeacherDashboardAsync(id));

        /// <summary>
        /// Retrieves the administrative dashboard - Admin Only
        /// </summary>
        [HttpGet("admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AdminDashboardResponseDto))]
        [Authorize(Policy = Policies.Admin)]
        public IActionResult GetAdminDashboard()
            => BuildResponse(dashboardService.GetAdminDashboard());

        /// <summary>
        /// Retrieves dashboard information specific to a guardian - For All
        /// </summary>
        /// <param name="id">The unique identifier of the guardian</param>
        [HttpGet("guardian/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GuardianDashboardResponseDto))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetGuardianDashboard(int id)
            => BuildResponse(await dashboardService.GetGuardianDashboardAsync(id));
    }
}