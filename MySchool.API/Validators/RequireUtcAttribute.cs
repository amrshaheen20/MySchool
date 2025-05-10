using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Validators
{
    public class RequireUtcAttribute : ValidationAttribute
    {
        public RequireUtcAttribute()
        {
            ErrorMessage = "The date and time must be in UTC format.";
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime.Kind != DateTimeKind.Utc)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            else if (value is DateTime?)
            {
                var nullableDate = (DateTime?)value;
                if (nullableDate.HasValue && nullableDate.Value.Kind != DateTimeKind.Utc)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success!;
        }
    }

}
