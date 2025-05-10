using MySchool.API.Models.DbSet;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class AnnouncementRequestDto
    {
        [Required, IsExists<User>(AllRoles = true)]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string Content { get; set; } = string.Empty;
    }

    public class AnnouncementResponseDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime time { get; set; }
    }
}
