using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ExamEntities;

namespace MySchool.API.Services.AssignmentContainer.Injector
{
    public class AssignmentInjector : CommandsInjector<Assignment>, IServiceInjector
    {
        public AssignmentInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();


            switch (UserRole)
            {
                case Enums.eRole.Admin:
                    //do nothing
                    break;
                case Enums.eRole.Teacher:
                    Where(x => x.CreatedById == UserId);
                    break;

                case Enums.eRole.Student:
                    Where(x => x.ClassRoom.Enrollments.Any(s => s.StudentId == UserId) && x.IsActive);
                    break;

                case Enums.eRole.Guardian:
                    var guardianRepo = unitOfWork.GetRepository<StudentGuardian>();
                    var studentIds = guardianRepo
                        .GetAll()
                        .Where(x => x.GuardianId == UserId)
                        .Select(x => x.StudentId);
                    Where(x => x.ClassRoom.Enrollments.Any(s => studentIds.Contains(s.StudentId)) && x.IsActive);
                    break;

                default:
                    Where(x => x.Id == 0); //return empty list
                    break;

            }

            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }
}
