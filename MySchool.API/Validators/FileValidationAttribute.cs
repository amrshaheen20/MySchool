using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Validators
{
    public class FileValidationAttribute : ValidationAttribute
    {
        public string[] AllowedExtensions { get; set; } = [];
        public long MaxFileSizeInBytes { get; set; } = 10 * 1024 * 1024;

        public FileValidationAttribute() { }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IFormFile file)
                return new ValidationResult("File is required.");

            if (file.Length == 0)
                return new ValidationResult("File cannot be empty.");

            if (file.Length > MaxFileSizeInBytes)
                return new ValidationResult($"File size cannot exceed {MaxFileSizeInBytes / (1024 * 1024)}MB.");

            var extension = Path.GetExtension(file.FileName)?.ToLower();
            if (AllowedExtensions.Length > 0 && !AllowedExtensions.Contains(extension))
            {
                return new ValidationResult($"Only the following file types are allowed: {string.Join(", ", AllowedExtensions)}");
            }

            return ValidationResult.Success;
        }
    }
}
