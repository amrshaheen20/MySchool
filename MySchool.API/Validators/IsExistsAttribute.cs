using MySchool.API.Enums;
using MySchool.API.Exceptions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using System.ComponentModel.DataAnnotations;

namespace MySchool.API.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IsExists<T> : ValidationAttribute where T : BaseEntity
    {
        public eRole Role { get; set; }
        public bool AllRoles { get; set; } = false;
        public string? AccountErrorMessage { get; set; }

        public IsExists()
        {
        }
        public IsExists(eRole role)
        {
            Role = role;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {

            if (value == null)
                return ValidationResult.Success!;

            var unitOfWork = validationContext.GetService<IUnitOfWork>();
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork), "Unit of work is not registered.");
            }

            var entityTask = unitOfWork.GetRepository<T>().GetByIdAsync((int)value);
            entityTask.Wait();
            var entity = entityTask.Result;

            if (entity == null)
            {
                throw new NotFoundException(ErrorMessage ?? $"{validationContext.DisplayName} invalid id.");
            }


            if (entity is User user && !AllRoles)
            {
                var ErrorMessageText = AccountErrorMessage ?? $"{validationContext.DisplayName} invalid user id.";
                if (Role != eRole.None && Role != user.Role)
                {
                    throw new NotFoundException(ErrorMessageText);
                }
                else
                {
                    foreach (var role in Enum.GetValues(typeof(eRole)))
                    {
                        if (validationContext.DisplayName.Contains(role.ToString()!, StringComparison.InvariantCultureIgnoreCase) && user.Role != (eRole)role)
                        {
                            throw new NotFoundException($"{validationContext.DisplayName} invalid user id.");
                        }
                    }
                }
            }

            return ValidationResult.Success!;
        }
    }

}
