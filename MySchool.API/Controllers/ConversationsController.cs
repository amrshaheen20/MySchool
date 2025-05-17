using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.ConversationContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.AllUsers)]
    public class ConversationsController(ConversationService conversationService) : BaseController
    {
        /// <summary>
        /// Create a conversation - For All
        /// </summary>
        /// <remarks>
        /// if the conversation already exists, it will return the existing conversation with response 200
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ConversationResponseDto))]
        public async Task<IActionResult> Create(ConversationRequsetDto requset)
        {
            return BuildResponse(await conversationService.CreateConversationAsync(requset));
        }

        /// <summary>
        /// Get all conversations - For All
        /// </summary>
        /// <param name="id">conversation ID</param>
        /// <param name="filter">The filter to apply</param>
        /// <returns>The conversations</returns>     
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<ConversationResponseDto>))]
        public async Task<IActionResult> GetConversations([FromQuery] int? id, [FromQuery] PaginationFilter<ConversationResponseDto> filter)
        {
            if (id != null)
            {
                return BuildResponse(await conversationService.GetConversationByIdAsync(id.Value));
            }
            return BuildResponse(conversationService.GetAllConversations(filter));
        }



        /// <summary>
        /// Delete a conversations by ID - For All
        /// </summary>
        /// <param name="conversation_id">The ID of the conversation to delete</param>
        /// <returns>The deleted conversation</returns>
        [HttpDelete("{conversation_id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteConversation(int conversation_id)
        {
            return BuildResponse(await conversationService.RemoveConversationAsync(conversation_id));
        }


        /// <summary>
        /// Sets the last read message in the conversation - For All
        /// </summary>
        /// <param name="conversation_id">The ID of the conversation.</param>
        /// <param name="message_id">The ID of the message.</param>
        [HttpPut("{conversation_id}/last-read/{message_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SetLastReadMessage(int conversation_id, int message_id)
        {
            return BuildResponse(await conversationService.SetLastReadMessageAsync(conversation_id, message_id));
        }


        /// <summary>
        /// Get all people that can make conversation with the user - For All
        /// </summary>

        [HttpGet("people")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<ConversationPeopleResponseDto>))]
        public IActionResult GetPeople([FromQuery] PaginationFilter<ConversationPeopleResponseDto> filter)
        {
            return BuildResponse(conversationService.GetAllPeople(filter));
        }

    }
}
