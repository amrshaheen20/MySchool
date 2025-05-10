using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.MessageContainer;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.AllUsers)]
    public class MessagesController(MessageService messageService) : BaseController
    {
        /// <summary>
        /// Send a message - For All
        /// </summary>
        /// <remarks>
        /// if the message already exists, it will return the existing message with response 200
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MessageResponseDto))]
        public async Task<IActionResult> Create(MessageRequestDto requset)
        {
            return BuildResponse(await messageService.CreateMessageAsync(requset));
        }

        /// <summary>
        /// Get all messages - For All
        /// </summary>
        /// <param name="message_id">message ID</param>
        /// <param name="conversation_id">Conversation ID</param>
        /// <param name="filter">The filter to apply</param>
        /// <returns>The messages</returns>     
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<MessageResponseDto>))]
        public async Task<IActionResult> GetMessages([FromQuery] int? message_id, [FromQuery] int? conversation_id, [FromQuery] PaginationFilter<MessageResponseDto> filter)
        {
            if (message_id != null)
            {
                return BuildResponse(await messageService.GetMessageByIdAsync(message_id.Value));
            }
            return BuildResponse(messageService.GetAllMessages(conversation_id, filter));
        }


        ///<summary>
        /// Update a message - For All
        /// </summary>
        /// <param name="message_id">The ID of the message to update</param>
        /// <param name="request">Message request</param>
        [HttpPatch("{message_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMessage(int message_id, MessageRequestDto request)
        {
            return BuildResponse(await messageService.UpdateMessageAsync(message_id, request));
        }


        /// <summary>
        /// Delete a messages by ID - For All
        /// </summary>
        /// <param name="message_id">The ID of the message to delete</param>
        /// <returns>The deleted message</returns>
        [HttpDelete("{message_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMessage(int message_id)
        {
            return BuildResponse(await messageService.DeleteMessageAsync(message_id));
        }

    }
}
