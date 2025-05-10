using Microsoft.AspNetCore.Mvc;
using MySchool.API.Models.DbSet;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class ConversationRequsetDto
    {
        [IsExists<User>(AllRoles = true), FromQuery, Required]
        public int UserId { get; set; }
    }
    public class ConversationResponseDto : BaseResponseDto
    {
        public required AccountResponseDto User { get; set; } = default!;
        public required MessageResponseDto? LastMessage { get; set; } = default!;
        public required int UnreadMessagesCount { get; set; }
    }

    public class ConversationPeopleResponseDto
    {
        public required AccountResponseDto User { get; set; } = default!;
        public required string Label { get; set; } = default!;
    }
}
