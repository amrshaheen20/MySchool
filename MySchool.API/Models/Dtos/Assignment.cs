using Microsoft.AspNetCore.Mvc;
using MySchool.API.Models.DbSet;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    //^___Form data 
    public class AssignmentRequestDto
    {
        [FromForm(Name = "mark")]
        [IsValid(typeof(RequiredAttribute))]
        public int? Mark { get; set; }

        [FromForm(Name = "title")]
        [IsValid(typeof(RequiredAttribute))]
        [StringLength(100, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string? Title { get; set; } = string.Empty;

        [FromForm(Name = "deadline")]
        [IsValid(typeof(RequiredAttribute))]
        [RequireUtc]
        public DateTime? Deadline { get; set; }

        [FromForm(Name = "attachment")]
        [IsValid(typeof(RequiredAttribute))]
        [FileValidation]
        public IFormFile? Attachment { get; set; }

        [FromForm(Name = "class_id")]
        [IsValid(typeof(RequiredAttribute)), IsExists<ClassRoom>]
        public int? ClassId { get; set; }

        [FromForm(Name = "subject_id")]
        [IsValid(typeof(RequiredAttribute)), IsExists<Subject>]
        public int? SubjectId { get; set; }

        [FromForm(Name = "is_active")]
        public bool IsActive { get; set; } = true;
    }

    public class AssignmentSubmissionRequestDto
    {
        [FromForm(Name = "assignment")]
        [FileValidation]
        public required IFormFile Attachment { get; set; }
    }

    //response
    public class AssignmentResponseDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public ClassResponseDto Class { get; set; } = default!;
        public SubjectResponseDto Subject { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public bool IsDeadlinePassed  => Deadline < DateTime.UtcNow;

        public int TotalSubmissions { get; set; }
    }

    public class AssignmentSubmissionResponseDto : BaseResponseDto
    {
        public required AssignmentResponseDto Assignment { get; set; } = default!;
        public required AccountResponseDto Student { get; set; } = default!;
        public required string FilePath { get; set; } = string.Empty;
    }

    public class AssignmentSubmissionWithMissingResponseDto
    {
        public required AccountResponseDto Student { get; set; } = default!;
        public required AssignmentResponseDto Assignment { get; set; } = default!;
        public required AssignmentSubmissionResponseDto? Submission { get; set; } = default!;
    }



}