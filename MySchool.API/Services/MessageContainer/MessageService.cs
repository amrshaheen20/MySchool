using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Hubs;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.ConversationContainer.Injector;
using MySchool.API.Services.MessageContainer.Injector;
using MySchool.API.Services.MessageContainer.Mapper;
using System.Net;

namespace MySchool.API.Services.MessageContainer
{
    public class MessageService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        MessageInjector messageInjector,
        ConversationInjector conversationInjector,
        IHttpContextAccessor contextAccessor,
        IHubContext<ChatHub> hubContext
        ) : IServiceInjector
    {
        private IGenericRepository<Message> GetRepository()
        {
            return unitOfWork.GetRepository<Message>().AddInjector(messageInjector);
        }

        private IQueryable<MessageResponseDto> GetMessagesQuery(IGenericRepository<Message> repository, CommandsInjector<Message> filter, int? conversationId = null)
        {
            var messages = repository.GetAllBy(filter);

            if (conversationId.HasValue && conversationId.Value > 0)
            {
                messages = messages.Where(m => m.ConversationId == conversationId.Value);
            }

            return MessageProfile.MapMessage(messages, mapper, contextAccessor.GetUserId());
        }


        public async Task<IBaseResponse<MessageResponseDto>> CreateMessageAsync(MessageRequestDto request)
        {
            var ConversationRepo = await unitOfWork.GetRepository<Conversation>().AddInjector(conversationInjector).GetByIdAsync(request.ConversationId!.Value);
            if (ConversationRepo == null)
            {
                return new BaseResponse<MessageResponseDto>()
                    .SetStatus(HttpStatusCode.Forbidden)
                    .SetMessage("You are not allowed to send messages in this conversation.");
            }

            var messageRepo = GetRepository();

            var newMessage = mapper.Map<Message>(request);
            newMessage.UserId = contextAccessor.GetUserId();
            newMessage.User = contextAccessor.HttpContext!.GetCurrentUser();

            if (ConversationRepo.UserOneId == contextAccessor.GetUserId())
            {
                ConversationRepo.UserOneLastReadMessageId = newMessage.Id;
            }
            else
            {
                ConversationRepo.UserTwoLastReadMessageId = newMessage.Id;
            }

            await messageRepo.AddAsync(newMessage);
            await unitOfWork.SaveAsync();

            var message = (await GetMessageByIdAsync(newMessage.Id))
                            .SetStatus(HttpStatusCode.Created);


            await hubContext.Clients.Users([ConversationRepo.UserOneId.ToString(), ConversationRepo.UserTwoId.ToString()])
                .SendAsync("receive-message", message.Data);


            return message;
        }

        public async Task<IBaseResponse<MessageResponseDto>> GetMessageByIdAsync(int messageId)
        {
            var messageRepo = GetRepository();
            var injector = new CommandsInjector<Message>().Where(c => c.Id == messageId);

            var existingMessage = await GetMessagesQuery(messageRepo, injector).FirstOrDefaultAsync();
            if (existingMessage == null)
            {
                return new BaseResponse<MessageResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Message not found.");
            }

            return new BaseResponse<MessageResponseDto>()
                   .SetStatus(HttpStatusCode.OK)
                   .SetData(existingMessage);
        }

        public IBaseResponse<PaginateBlock<MessageResponseDto>> GetAllMessages(int? conversationId, PaginationFilter<MessageResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<MessageResponseDto>>()
                   .SetData(filter.Apply(GetMessagesQuery(GetRepository(), new CommandsInjector<Message>(), conversationId)));
        }


        public async Task<IBaseResponse<object>> UpdateMessageAsync(int id, MessageRequestDto request)
        {
            var repository = GetRepository();
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Message not found.");
            }

            if (entity.UserId != contextAccessor.GetUserId())
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.Forbidden)
                    .SetMessage("You are not allowed to update this message.");
            }

            entity.Content = request.Content;
            repository.Update(entity);
            await unitOfWork.SaveAsync();


            await hubContext.Clients.Users([entity.Conversation.UserOneId.ToString(), entity.Conversation.UserTwoId.ToString()])
                .SendAsync("update-message",
                new
                {
                    entity.Id,
                    entity.UserId,
                    entity.ConversationId,
                    entity.Content
                });

            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Message updated successfully.");
        }


        public async Task<IBaseResponse<object>> DeleteMessageAsync(int id)
        {
            var repository = GetRepository();
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Message not found.");
            }

            if (entity.UserId != contextAccessor.GetUserId() && contextAccessor.HttpContext!.GetCurrentUser().Role == eRole.Admin)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.Forbidden)
                    .SetMessage("You are not allowed to delete this message.");
            }


            repository.Delete(entity);
            await unitOfWork.SaveAsync();

            await hubContext.Clients.Users([entity.Conversation.UserOneId.ToString(), entity.Conversation.UserTwoId.ToString()])
                .SendAsync("delete-message",
                new
                {
                    entity.Id,
                    entity.UserId,
                    entity.ConversationId,
                    entity.Content
                });


            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Message deleted successfully.");

        }
    }
}
