using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Controllers;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AssignmentContainer;
using MySchool.API.Validators;

[Route("api/[controller]")]
[ApiController]
public class StudentsController(AssignmentService assignmentService) : BaseController
{
    /// <summary>
    /// Retrieves all assignment submissions for a specific student, including missing assignments - For All
    /// </summary>
    /// <param name="student_id">The unique identifier of the student.</param>
    /// <param name="filter">Filtering and pagination</param>
    [HttpGet("{student_id}/assignments/submissions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<AssignmentSubmissionWithMissingResponseDto>))]
    [Authorize(Policy = Policies.AllUsers)]
    public IActionResult GetStudentAssignmentsWithSubmissions(
        [IsExists<User>(Role = eRole.Student)] int student_id,
        [FromQuery] PaginationFilter<AssignmentSubmissionWithMissingResponseDto> filter)
    {
        return BuildResponse(assignmentService.GetAllStudentSubmissionsWithMissing(student_id, filter));
    }
}
