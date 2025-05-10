using AutoMapper;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.MessageContainer.Mapper
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            // Message Mapping
            CreateMap<MessageRequestDto, Message>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));

            CreateMap<Message, MessageResponseDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.UpdatedAt != src.CreatedAt))
                .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));
        }

        public static IQueryable<MessageResponseDto> MapMessage(IQueryable<Message> query, IMapper mapper, int userId)
        {

            return query.Select(m => new MessageResponseDto
            {
                Id = m.Id,
                User = mapper.Map<AccountResponseDto>(m.User),
                IsEdited = m.UpdatedAt != m.CreatedAt,
                CreatedAt = m.CreatedAt,
                Content = m.Content,
                ConversationId = m.ConversationId,
                IsUnread = m.UserId == userId ? false :
                           (m.Conversation.UserOne.Id == userId
                               ? (m.Conversation.UserTwoLastReadMessageId ?? 0) < m.Id
                               : (m.Conversation.UserOneLastReadMessageId ?? 0) < m.Id
                           )
            });

        }

    }
}
