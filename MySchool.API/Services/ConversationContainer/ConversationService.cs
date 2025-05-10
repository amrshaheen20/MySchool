using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Hubs;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ClassRoomEntities;
using MySchool.API.Models.Dtos;
using MySchool.API.Services.AccountContainer.Injector;
using MySchool.API.Services.ConversationContainer.Injector;
using MySchool.API.Services.ConversationContainer.Mapper;
using System.Net;

namespace MySchool.API.Services.ConversationContainer
{
    public class ConversationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ConversationInjector conversationInjector,
        IHttpContextAccessor contextAccessor,
        IHubContext<ChatHub> hubContext,
        AccountInjector accountInjector
        ) : IServiceInjector
    {
        private IGenericRepository<Conversation> GetRepository()
        {
            return unitOfWork.GetRepository<Conversation>().AddInjector(conversationInjector);
        }

        private IQueryable<ConversationResponseDto> GetConversationsQuery(IGenericRepository<Conversation> repository, CommandsInjector<Conversation> filter)
        {
            var conversations = repository.GetAllBy(filter);
            return ConversationProfile.MapConversation(conversations, mapper, contextAccessor.GetUserId());
        }


        public async Task<IBaseResponse<ConversationResponseDto>> CreateConversationAsync(ConversationRequsetDto request)
        {
            /*
            Scenario Case:
            - If the conversation already exists, return the existing one
            - If not, create a new conversation
            */

            var conversationRepo = GetRepository();


            var injector = new CommandsInjector<Conversation>();
            injector.Where(c => (c.UserOneId == contextAccessor.GetUserId() && c.UserTwoId == request.UserId)
                     || (c.UserOneId == request.UserId && c.UserTwoId == contextAccessor.GetUserId()));

            var existingConversation = await GetConversationsQuery(conversationRepo, injector).FirstOrDefaultAsync();
            if (existingConversation != null)
            {
                return new BaseResponse<ConversationResponseDto>()
                    .SetStatus(HttpStatusCode.OK)
                    .SetMessage("Conversation already exists.")
                    .SetData(existingConversation);
            }


            var newConversation = mapper.Map<Conversation>(request);
            newConversation.UserOneId = contextAccessor.GetUserId();

            await conversationRepo.AddAsync(newConversation);
            await unitOfWork.SaveAsync();


            return (await GetConversationByIdAsync(newConversation.Id))
                            .SetStatus(HttpStatusCode.Created);
        }

        public async Task<IBaseResponse<ConversationResponseDto>> GetConversationByIdAsync(int conversationId)
        {
            var conversationRepo = GetRepository();
            var injector = new CommandsInjector<Conversation>().Where(c => c.Id == conversationId);

            var existingConversation = await GetConversationsQuery(conversationRepo, injector).FirstOrDefaultAsync();
            if (existingConversation == null)
            {
                return new BaseResponse<ConversationResponseDto>()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Conversation not found.");
            }

            return new BaseResponse<ConversationResponseDto>()
                   .SetStatus(HttpStatusCode.OK)
                   .SetData(existingConversation);
        }

        public IBaseResponse<PaginateBlock<ConversationResponseDto>> GetAllConversations(PaginationFilter<ConversationResponseDto> filter)
        {
            return new BaseResponse<PaginateBlock<ConversationResponseDto>>()
                   .SetData(filter.Apply(GetConversationsQuery(GetRepository(), new CommandsInjector<Conversation>())));
        }

        public async Task<IBaseResponse<object>> RemoveConversationAsync(int id)
        {
            var repository = GetRepository();
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Conversation not found.");
            }
            repository.Delete(entity);
            await unitOfWork.SaveAsync();
            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Conversation deleted successfully.");

        }

        public async Task<IBaseResponse<object>> SetLastReadMessageAsync(int conversation_id, int message_id)
        {
            var conversationRepo = GetRepository();
            var conversation = await conversationRepo.GetByIdAsync(conversation_id);
            if (conversation == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Conversation not found.");
            }

            var message = await unitOfWork.GetRepository<Message>().GetByAsync(new CommandsInjector<Message>().Where(x => x.Id == message_id && x.ConversationId == conversation_id));
            if (message == null)
            {
                return new BaseResponse()
                    .SetStatus(HttpStatusCode.NotFound)
                    .SetMessage("Message not found in this conversation.");
            }

            if (contextAccessor.GetUserId() == conversation.UserOneId)
                conversation.UserOneLastReadMessageId = message_id;
            else if (contextAccessor.GetUserId() == conversation.UserTwoId)
                conversation.UserTwoLastReadMessageId = message_id;

            conversationRepo.Update(conversation);
            await unitOfWork.SaveAsync();

            await hubContext.Clients.Users([conversation.UserOneId.ToString(), conversation.UserTwoId.ToString()])
                  .SendAsync("update-last-read-message",
                  new
                  {
                      conversation_id,
                      message_id
                  });


            return new BaseResponse()
                .SetStatus(HttpStatusCode.OK)
                .SetMessage("Last read message updated successfully.");
        }


        public IBaseResponse<PaginateBlock<ConversationPeopleResponseDto>> GetAllPeople(PaginationFilter<ConversationPeopleResponseDto> filter)
        {
            accountInjector.ConversationPeopleInject();
            var userRepo = unitOfWork.GetRepository<User>().AddInjector(accountInjector);
            var timetableRepo = unitOfWork.GetRepository<Timetable>();

            var users = userRepo.GetAll();

            var peoplesList = users
                .Select(x => new ConversationPeopleResponseDto
                {
                    User = mapper.Map<AccountResponseDto>(x),
                    Label = (timetableRepo.GetAll()
                                      .Where(t => t.TeacherId == x.Id)
                                      .Select(t => t.Subject.Name)
                                      .FirstOrDefault()) ?? x.Role.ToString()
                });

            return new BaseResponse<PaginateBlock<ConversationPeopleResponseDto>>()
               .SetStatus(HttpStatusCode.OK)
               .SetData(filter.Apply(peoplesList));
        }

    }
}