using MySchool.API.Models.DbSet;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class StudentGuardianRequestDto
    {
        [IsExists<User>]
        public int StudentId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "RelationToStudent must be less than 100 characters")]
        public string? RelationToStudent { get; set; }
    }

    public class StudentGuardianResponseDto
    {
        public int GuardianId { get; set; }
        public AccountResponseDto Student { get; set; } = default!;
        public string RelationToStudent { get; set; } = string.Empty;
    }
}
