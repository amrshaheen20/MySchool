using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IsValidAttribute : ValidationAttribute
    {
        private readonly Type[] _validatorTypes;

        public IsValidAttribute(params Type[] validatorTypes)
        {
            _validatorTypes = validatorTypes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var httpContextAccessor = validationContext.GetService<IHttpContextAccessor>();

            if (httpContextAccessor == null || httpContextAccessor.HttpContext == null)
            {
                return new ValidationResult("Unable to determine the request method.");
            }

            var request = httpContextAccessor.HttpContext.Request;
            bool isPatch = request.Method.Equals(HttpMethod.Patch.Method, StringComparison.OrdinalIgnoreCase);

            if (!isPatch || (isPatch && value != null))
            {
                foreach (var validatorType in _validatorTypes)
                {
                    if (Activator.CreateInstance(validatorType) is ValidationAttribute validator)
                    {
                        var result = validator.GetValidationResult(value, validationContext);
                        if (result != ValidationResult.Success)
                        {
                            return result;
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
