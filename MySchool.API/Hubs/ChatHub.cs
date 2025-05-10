using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MySchool.API.Extensions;
using MySchool.API.Services.ConversationContainer;
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
    public class ChatHub
    (ConversationService conversationService) : Hub
    {
        private static Dictionary<string, List<string>> ConnectedUsers = new();

        /// <summary>
        /// Sends a message to all connected clients in the specified conversation.
        /// </summary>
        /// <param name="conversationId">Conversation ID to send the message
        /// to.</param> <param name="message">the message to send.</param>
        /// <returns></returns>
        public async Task SendMessage(int conversationId, string message)
        {
            // Here you can add logic to save the message to the database if needed
            // For example: await _messageService.SaveMessage(conversationId,
            // message); Broadcast the message to all connected clients
            await Clients.All.SendAsync("receive-message", conversationId, message);
        }

        /// <summary>
        /// Set last read message for the conversation
        /// </summary>
        /// <param name="conversationId">Conversation ID to send the message
        /// to.</param> <param name="messageId">Message ID to set as read.</param>
        public async Task SetLastReadMessage(int conversationId, int messageId)
        {
            await conversationService.SetLastReadMessageAsync(conversationId,
                                                              messageId);
        }

        [SignalRHidden]
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User?.Identity?.Name!;

            if (!ConnectedUsers.ContainsKey(userId))
                ConnectedUsers[userId] = new List<string>();

            ConnectedUsers[userId].Add(connectionId);

            Console.WriteLine(
                $"User {userId} connected with connection {connectionId}");

            await base.OnConnectedAsync();
        }

        [SignalRHidden]
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User?.Identity?.Name!;

            if (ConnectedUsers.ContainsKey(userId))
            {
                ConnectedUsers[userId].Remove(connectionId);

                if (ConnectedUsers[userId].Count == 0)
                    ConnectedUsers.Remove(userId);

                Console.WriteLine(
                    $"User {userId} disconnected from connection {connectionId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
