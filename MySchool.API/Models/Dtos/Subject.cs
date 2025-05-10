using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class SubjectRequestDto
    {
        [IsValid(typeof(RequiredAttribute))]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }

    public class SubjectResponseDto : BaseResponseDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
