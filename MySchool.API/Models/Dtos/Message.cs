using MySchool.API.Models.DbSet;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class MessageRequestDto
    {
        [IsValid(typeof(RequiredAttribute)), IsExists<Conversation>]//no need in patch request
        public int? ConversationId { get; set; }
        [Required, StringLength(500)]
        public string Content { get; set; } = default!;
    }

    public class MessageResponseDto : BaseResponseDto
    {
        public required int ConversationId { get; set; }
        public required string Content { get; set; } = default!;
        public required AccountResponseDto User { get; set; } = default!;
        public required bool IsEdited { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required bool IsUnread { get; set; }
    }
}
