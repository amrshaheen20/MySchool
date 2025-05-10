using MySchool.API.Enums;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.SubjectEntities;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class GradeRequestDto
    {
        [IsValid(typeof(RequiredAttribute)), IsExists<User>(Role = eRole.Student)]
        public int? StudentId { get; set; }

        [IsValid(typeof(RequiredAttribute)), IsExists<Subject>()]
        public int? SubjectId { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [Range(1, 2, ErrorMessage = "Term number must be between 1 and 2.")]
        public int? TermNumber { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public float? Mark { get; set; }
    }

    public class GradeResponseDto : BaseResponseDto
    {
        public AccountResponseDto Student { get; set; } = default!;
        public SubjectResponseDto Subject { get; set; } = default!;
        public float Mark { get; set; }
    }
}
