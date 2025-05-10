using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Models.Dtos
{
    public class LoginRequestDto
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public required string UserName { get; set; }
        public required string Token { get; set; }
        public required DateTime Expiration { get; set; }
        public required bool MustChangePassword { get; set; }
    }

    public class ChangePasswordRequestDto
    {
        [Required]
        public string OldPassword { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string NewPassword { get; set; } = default!;
    }
}
