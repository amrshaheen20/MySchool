using MySchool.API.Enums;
using MySchool.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class AccountRequestDto
    {

        [IsValid(typeof(RequiredAttribute))]
        public eRole? Role { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string? UserName { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string? Password { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be exactly 14 digits.")]
        public string? NationalId { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string? Name { get; set; }

        public eGender? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string? Address { get; set; }

        /// <example>01012345678</example>
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid Phone Number, it should be in this format: 01XXXXXXXXX")]
        public string? PhoneNumber { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public bool? IsActive { get; set; }

        public bool? MustChangePassword { get; set; }

    }

    public class AccountAdminResponseDto : AccountResponseDto
    {
        public string? UserName { get; set; }
        public string? NationalId { get; set; }
        public bool? IsActive { get; set; }
        public bool? MustChangePassword { get; set; }
    }

    public class AccountResponseDto : BaseResponseDto
    {
        public eRole? Role { get; set; }
        public string? Name { get; set; }
        public eGender? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime LastActiveTime { get; set; }
    }

}
