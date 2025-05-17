using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.GradeContainer;

namespace MySchool.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class GradesController(GradeService gradeService) : BaseController
    {

        /// <summary> 
        /// Create a new grade record - Teacher Only 
        /// </summary>
        /// <param name="request">The grade information to create</param>
        /// <returns>The created grade record</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GradeResponseDto))]
        [Authorize(Policy = Policies.Teacher)]
        public async Task<IActionResult> Create([FromBody] GradeRequestDto request)
        {
            return BuildResponse(await gradeService.CreateGradeAsync(request));
        }


        /// <summary> 
        /// Get all grades related to the user - For All 
        /// </summary> 
        /// <param name="id">Get a specific grade by ID (optional)</param> 
        /// <param name="filter">Pagination and filter parameters for grade retrieval</param>
        /// <returns>Either a single grade or a paginated list of grades</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<GradeResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetAll([FromQuery] int? id, [FromQuery] PaginationFilter<GradeResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await gradeService.GetGradeByIdAsync(id.Value));
            }
            return BuildResponse(gradeService.GetAllGrades(filter));

        }

        /// <summary> 
        /// Update a grade by ID - Teacher Only 
        /// </summary>
        /// <param name="id">ID of the grade to update</param>
        /// <param name="request">The updated grade information</param>
        /// <returns>The updated grade record</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Teacher)]
        public async Task<IActionResult> Update(int id, [FromBody] GradeRequestDto request)
        {
            return BuildResponse(await gradeService.UpdateGradeAsync(id, request));
        }

        /// <summary> 
        /// Delete a specific grade by ID - Admin and Teacher Only 
        /// </summary>
        /// <param name="id">ID of the grade to delete</param>
        /// <returns>The deleted grade record</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> Delete(int id)
        {
            return BuildResponse(await gradeService.DeleteGradeAsync(id));
        }

        /// <summary> 
        /// Publish all grades created - Admin Only 
        /// </summary>
        /// <returns>Result of the publish operation</returns>
        [HttpPut("publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> Publish()
        {
            return BuildResponse(await gradeService.PublishGradesAsync());
        }

        /// <summary> 
        /// Unpublish all grades - Admin Only 
        /// </summary>
        /// <returns>Result of the unpublish operation</returns>
        [HttpPut("unpublish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> Unpublish()
        {
            return BuildResponse(await gradeService.UnpublishGradesAsync());
        }

        /// <summary> 
        /// Delete all grades - Admin Only 
        /// </summary>
        /// <returns>Result of the delete all operation</returns>
        [HttpDelete("delete-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> DeleteAll()
        {
            return BuildResponse(await gradeService.DeleteAllGradesAsync());
        }

        /// <summary>
        /// Get grades for a specific subject - Admin and Teacher Only 
        /// </summary>
        /// <param name="request">Subject filter parameters</param>
        /// <param name="filter">Pagination filter</param>
        /// <returns>Paginated list of subject grades</returns>
        [HttpGet("subject-grades")]
        [ProducesResponseType(typeof(IBaseResponse<PaginateBlock<SubjectGradesResponseDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.Moderator)]
        public IActionResult GetSubjectGrades([FromQuery] SubjectGradesRequestDto request, [FromQuery] PaginationFilter<SubjectGradesResponseDto> filter)
        {
            var response = gradeService.GetSubjectGradesById(request, filter);
            return BuildResponse(response);
        }
    }
}