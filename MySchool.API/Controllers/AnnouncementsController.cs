using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AnnouncementContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AnnouncementsController(AnnouncementService announcementService) : BaseController
    {
        /// <summary>
        /// Create a new announcement - Admin and Teacher Only
        /// </summary>
        /// <param name="announcementDto">Announcement to create</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnnouncementResponseDto))]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> PostAnnouncement(AnnouncementRequestDto announcementDto)
        {
            return BuildResponse(await announcementService.CreateAnnouncementAsync(announcementDto));
        }

        /// <summary>
        /// Get all announcements or an announcement by ID - For All
        /// </summary>
        /// <param name="id"> The ID of the announcement to get</param>
        /// <param name="filter"> The filter to apply to the announcements</param>
        /// <remarks>
        /// For admins, it will return all announcements.<br/>
        /// For students, it will return all announcements for the student.<br/>
        /// For guardians, it will return all announcements for the students they are guardians of.<br/>
        /// For teachers, it will return all announcements created by the teacher.<br/>
        /// Other Wise, it will return an empty list.<br/>
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<AnnouncementResponseDto>))]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> GetAnnouncements([FromQuery] int? id, [FromQuery] PaginationFilter<AnnouncementResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await announcementService.GetAnnouncementByIdAsync(id.Value));
            }

            return BuildResponse(announcementService.GetAllAnnouncementAsync(filter));
        }

        /// <summary>
        /// Delete an announcement - Admin and Teacher Only
        /// </summary>
        /// <param name="announcement_id">The ID of the announcement to delete</param>
        /// <returns></returns> 
        [HttpDelete("{announcement_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Policy = Policies.Moderator)]
        public async Task<IActionResult> DeleteAnnouncement(int announcement_id)
        {
            return BuildResponse(await announcementService.DeleteAnnouncementAsync(announcement_id));
        }
    }
}
