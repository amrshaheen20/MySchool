using MySchool.API.Enums;
using MySchool.API.Models.DbSet;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class FeeRequestDto
    {
        [IsValid(typeof(RequiredAttribute))]
        [IsExists<User>(Role = eRole.Student)]
        public int? StudentId { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public decimal? TotalAmount { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public decimal? PaidAmount { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public DateTime? DueDate { get; set; }

        public string? Description { get; set; }
    }

    public class FeeResponseDto : BaseResponseDto
    {
        public AccountResponseDto Student { get; set; } = default!;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime PaidDate { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}
