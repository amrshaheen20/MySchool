using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.ClassContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController(ClassService classService) : BaseController
    {
        /// <summary>
        /// Create a new class - Admin only
        /// </summary>
        /// <param name="Class">The class to create</param>
        /// <returns>The created class</returns>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ClassResponseDto))]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> Create(ClassRequestDto Class)
        {
            return BuildResponse(await classService.CreateClassAsync(Class));
        }

        /// <summary>
        /// Get all classes or a class by ID - For All
        /// </summary>
        /// <param name="id">Class ID</param>
        /// <param name="filter">The filter for the classes</param>
        /// <returns>The classes</returns>
        /// <remarks>
        /// **For Admin: Get all classes<br/>
        /// For Teacher: Get all classes that he is teaching<br/>
        /// For Student: Get all classes that he is in<br/>
        /// For Guardian: Get all classes that his childs are in<br/>
        /// For Student: Get all classes that he is in<br/>
        /// Other Wise: Get empty list<br/>
        /// </remarks>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<ClassResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetClasses([FromQuery] int? id, [FromQuery] PaginationFilter<ClassResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await classService.GetClassByIdAsync(id.Value));
            }
            return BuildResponse(classService.GetAllClasses(filter));
        }

        /// <summary>
        /// Delete a class - Admin only
        /// </summary>
        /// <param name="class_id">The id of the class</param>
        /// <returns>The deleted class</returns>
        [HttpDelete("{class_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> DeleteClass(int class_id)
        {
            return BuildResponse(await classService.DeleteClassByIdAsync(class_id));
        }

        /// <summary>
        /// Update a class - Admin only
        /// </summary>
        /// <param name="class_id">The id of the class</param>
        /// <param name="Class">The class to update</param>
        /// <returns>The updated class</returns>
        [HttpPatch("{class_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> UpdateClass(int class_id, ClassRequestDto Class)
        {
            return BuildResponse(await classService.UpdateClassAsync(class_id, Class));
        }




    }
}
