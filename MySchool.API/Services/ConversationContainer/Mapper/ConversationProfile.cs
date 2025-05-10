using AutoMapper;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.ConversationContainer.Mapper
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            // Request
            CreateMap<ConversationRequsetDto, Conversation>()
                .ForMember(dest => dest.UserTwoId, opt => opt.MapFrom(src => src.UserId));

            // Response

        }


        public static IQueryable<ConversationResponseDto> MapConversation(IQueryable<Conversation> query, IMapper mapper, int userId)
        {
            //this is stupid, but it is the only way to pass the userId to the mapping right now :\
            return query.Select(x => new ConversationResponseDto()
            {
                Id = x.Id,
                User = x.UserOne.Id == userId
                           ? mapper.Map<AccountResponseDto>(x.UserTwo)
                           : mapper.Map<AccountResponseDto>(x.UserOne),

                LastMessage = x.Messages
                       .OrderByDescending(m => m.Id)
                       .Select(m => new MessageResponseDto
                       {
                           Id = m.Id,
                           User = mapper.Map<AccountResponseDto>(m.User),
                           IsEdited = m.UpdatedAt != m.CreatedAt,
                           CreatedAt = m.CreatedAt,
                           Content = m.Content,
                           ConversationId = m.ConversationId,
                           IsUnread = m.UserId == userId ? false :
                           (x.UserOne.Id == userId
                               ? (x.UserTwoLastReadMessageId ?? 0) < m.Id
                               : (x.UserOneLastReadMessageId ?? 0) < m.Id
                           )
                       })
                       .FirstOrDefault(),

                UnreadMessagesCount = x.Messages.Count(i =>
                    i.Id > ((x.UserOne.Id == userId ? x.UserOneLastReadMessageId : x.UserTwoLastReadMessageId) ?? 0)
                    && i.UserId != userId
                   )

            });
        }




    }
}