using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MySchool.API.Extensions;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AccountContainer;
using MySchool.API.Services.ConversationContainer;
using MySchool.API.Services.MessageContainer;
using SignalRSwaggerGen.Attributes;

namespace MySchool.API.Hubs
{
    public static class ChatHubExtensions
    {
        public const string Route = "api/chathub";
        public static IEndpointRouteBuilder MapChatHub(
            this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<ChatHub>(Route);
            return endpoints;
        }
    }

    [SignalRHub(ChatHubExtensions.Route)]
    [Authorize(Policy = Policies.AllUsers)]
    public class ChatHub(
        ConversationService conversationService,
        MessageService messageService,
        ILogger<ChatHub> logger
        ) : Hub
    {
        private static Dictionary<int, List<string>> ConnectedUsers = new();

        /// <summary>
        /// Sends a message to all connected clients in the specified conversation.
        /// </summary>
        /// <param name="conversationId">Conversation ID to send the message
        /// to.</param> <param name="message">the message to send.</param>
        /// <returns></returns>
        public async Task<MessageResponseDto?> SendMessage(int conversationId, string message)
        {
            var response = (await messageService.CreateMessageAsync(new()
            {
                ConversationId = conversationId,
                Content = message
            }));

            if (!response.IsSuccess)
            {
                throw new ApplicationException(response.Message);
            }

            return response.Data;
        }

        /// <summary>
        /// Set last read message for the conversation
        /// </summary>
        /// <param name="conversationId">Conversation ID to send the message
        /// to.</param> <param name="messageId">Message ID to set as read.</param>
        public async Task SetLastReadMessage(int conversationId, int messageId)
        {
            var response = await conversationService.SetLastReadMessageAsync(conversationId, messageId);
            if (!response.IsSuccess)
            {
                throw new ApplicationException(response.Message);
            }
        }

        /// <summary>
        /// get User online status - For All
        /// </summary>
        /// <param name="userId">User ID to get his online status
        public bool OnlineStatus(int userId)
        {
            if (ConnectedUsers.ContainsKey(userId))
                return true;

            return false;
        }


        [SignalRHidden]
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.GetHttpContext()!.GetUserId();



            if (!ConnectedUsers.ContainsKey(userId))
                ConnectedUsers[userId] = new List<string>();

            ConnectedUsers[userId].Add(connectionId);

            logger.LogInformation($"User {userId} connected with connection {connectionId}");

            await base.OnConnectedAsync();
        }

        [SignalRHidden]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.GetHttpContext()!.GetUserId();

            if (ConnectedUsers.ContainsKey(userId))
            {
                ConnectedUsers[userId].Remove(connectionId);

                if (ConnectedUsers[userId].Count == 0)
                    ConnectedUsers.Remove(userId);

                logger.LogInformation($"User {userId} disconnected from connection {connectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
