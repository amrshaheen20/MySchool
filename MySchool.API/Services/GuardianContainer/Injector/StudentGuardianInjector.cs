using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Services.GuardianContainer.Injector
{
    public class StudentGuardianInjector : CommandsInjector<StudentGuardian>, IServiceInjector
    {
        public StudentGuardianInjector(IHttpContextAccessor contextAccessor)
        {

            var userId = contextAccessor.GetUserId();

            switch (contextAccessor.GetUserRole())
            {
                case eRole.Admin:
                case eRole.Teacher:
                    break;
                case eRole.Student:
                    AddCommand(q => q.Where(q => q.StudentId == userId));
                    break;

                default:
                    AddCommand(q => q.Where(q => q.GuardianId == userId));
                    break;
            }



            AddCommand(q => q.OrderByDescending(x => x.Id));
            AddCommand(q => q.Include(x => x.Student));
            // AddCommand(q => q.Include(x => x.Guardian));
        }
    }
}
