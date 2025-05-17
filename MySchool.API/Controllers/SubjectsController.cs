using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.SubjectContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[ApiExplorerSettings(GroupName = "Admin Routes")]
    public class SubjectsController(SubjectService subjectService) : BaseController
    {
        /// <summary>
        /// Create a new subject - Admin Only
        /// </summary>
        /// <param name="subject">The subject to create</param>
        /// <returns>The created subject</returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SubjectResponseDto))]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> Create(SubjectRequestDto subject)
        {
            return BuildResponse(await subjectService.CreateSubjectAsync(subject));
        }

        /// <summary>
        /// Get all subjects or a subject by ID - For all
        /// </summary>
        /// <param name="id">subject ID</param>
        /// <param name="filter">The filter to apply</param>
        /// <returns>The subjects</returns>     
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<SubjectResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetSubjects([FromQuery] int? id, [FromQuery] PaginationFilter<SubjectResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await subjectService.GetSubjectByIdAsync(id.Value));
            }
            return BuildResponse(subjectService.GetAllSubjects(filter));
        }



        /// <summary>
        /// Delete a subject by ID - Admin Only
        /// </summary>
        /// <param name="subject_id">The ID of the subject to delete</param>
        /// <returns>The deleted subject</returns>
        [HttpDelete("{subject_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> DeleteSubject(int subject_id)
        {
            return BuildResponse(await subjectService.DeleteSubjectByIdAsync(subject_id));
        }

        /// <summary>
        /// Update a subject by ID - Admin Only
        /// </summary>
        /// <param name="subject_id">The ID of the subject to update</param>
        /// <param name="subject">The subject to update</param>
        /// <returns>The updated subject</returns>
        [HttpPatch("{subject_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> UpdateSubject(int subject_id, SubjectRequestDto subject)
        {
            return BuildResponse(await subjectService.UpdateSubjectAsync(subject_id, subject));
        }
    }
}
