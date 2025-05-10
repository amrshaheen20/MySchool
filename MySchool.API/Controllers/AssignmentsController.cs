using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.DbSet.ExamEntities;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AssignmentContainer;
using MySchool.API.Validators;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AssignmentsController(AssignmentService assignmentService) : BaseController
    {
        /// <summary>
        /// Create a new assignment - Teacher Only
        /// </summary>
        /// <param name="assignmentDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = Policies.Teacher)]
        public async Task<IActionResult> PostAssignment([FromForm] AssignmentRequestDto assignmentDto)
        {
            return BuildResponse(await assignmentService.CreateAssignment(assignmentDto));
        }

        /// <summary>
        /// Get all Assignments - For All
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// For Admin: Get all assignments<br/>
        /// For Theacher: Get all assignments that he made<br/>
        /// For Student: Get all assignments that his class teacher made<br/>
        /// For Guardian: Get all assignments that his child's 
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<AssignmentResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetAssignments([FromQuery] int? id, [FromQuery] PaginationFilter<AssignmentResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await assignmentService.GetAssignmentByIdAsync(id.Value));
            }
            return BuildResponse(assignmentService.GetAllAssignments(filter));
        }


        ///<summary>
        /// Update an Assignment - Teacher Only
        ///</summary>
        /// <param name="assignment_id"></param>
        /// <param name="assignmentDto"></param>
        /// <returns></returns>
        [HttpPatch("{assignment_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.Teacher)]
        public async Task<IActionResult> UpdateAssignment(int assignment_id, [FromForm] AssignmentRequestDto assignmentDto)
        {
            return BuildResponse(await assignmentService.UpdateAssignment(assignment_id, assignmentDto));
        }



        /// <summary>
        /// Delete an Assignment - Admin and Teacher Only
        /// </summary>
        /// <param name="assignment_id"></param>
        /// <returns></returns> 
        [HttpDelete("{assignment_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> DeleteAssignment(int assignment_id)
        {
            return BuildResponse(await assignmentService.DeleteAssignment(assignment_id));
        }

        //send submissions

        /// <summary>
        /// Send assignment submissions or upload assignment submissions - Student Only
        /// </summary>

        [HttpPost("{assignment_id}/Submissions")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AssignmentSubmissionResponseDto))]
        [Authorize(Policy = Policies.Student)]
        public async Task<IActionResult> SendAssignmentSubmissions(int assignment_id, [FromForm] AssignmentSubmissionRequestDto assignmentSubmissionDto)
        {
            return BuildResponse(await assignmentService.SendSubmissions(assignment_id, assignmentSubmissionDto));
        }

        /// <summary>
        /// Get all assignment submissionss - For All
        /// </summary>

        [HttpGet("{assignment_id}/Submissions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<AssignmentSubmissionResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetAssignmentSubmissionss([IsExists<Assignment>] int assignment_id, [FromQuery] int? id, [FromQuery] PaginationFilter<AssignmentSubmissionResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await assignmentService.GetSubmissionsByIdAsync(assignment_id, id.Value));
            }

            return BuildResponse(assignmentService.GetAllSubmissionss(assignment_id, filter));
        }

        /// <summary>
        /// Retrieves the submission status of students for a specific assignment, including both submitted and missing submissions - Admin and Teacher Only
        /// </summary>
        /// <param name="assignment_id">The unique identifier of the assignment to check submissions for</param>
        /// <param name="filter"> Filtering and pagination</param>
        [HttpGet("{assignment_id}/submission-status")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<AssignmentSubmissionWithMissingResponseDto>))]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> GetAssignmentSubmissionssWithMissing(int assignment_id, [FromQuery] PaginationFilter<AssignmentSubmissionWithMissingResponseDto> filter)
        {
            return BuildResponse(await assignmentService.GetAllSubmissionsWithMissingAsync(assignment_id, filter));
        }

        /// <summary>
        /// Delete assignment submissions - Admin and Teacher Only
        /// </summary>

        [HttpDelete("{assignment_id}/Submissions/{submissions_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> DeleteAssignmentSubmissions(int assignment_id, int submissions_id)
        {
            return BuildResponse(await assignmentService.DeleteSubmissions(assignment_id, submissions_id));
        }


    }

}
