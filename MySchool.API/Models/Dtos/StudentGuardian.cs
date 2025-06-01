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
        public AccountResponseDto Guardian { get; set; }= default!;
        public AccountResponseDto Student { get; set; } = default!;
        public string RelationToStudent { get; set; } = string.Empty;
    }

    public class GuardianResponseDto
    {
        public AccountResponseDto Guardian { get; set; } = default!;
        public int TotalChildren { get; set; }
    }
}
