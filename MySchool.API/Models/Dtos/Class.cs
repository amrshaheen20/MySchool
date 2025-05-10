using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class ClassRequestDto
    {
        [IsValid(typeof(RequiredAttribute))]
        [StringLength(100, MinimumLength = 1)]
        public string? Name { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [Range(1, 12, ErrorMessage = "Grade must be between 1 and 12.")]
        public int? Grade { get; set; }
    }

    public class ClassResponseDto : BaseResponseDto
    {
        public required string Name { get; set; }
        public required int Grade { get; set; }

        public int StudentCount { get; set; }
    }
}
