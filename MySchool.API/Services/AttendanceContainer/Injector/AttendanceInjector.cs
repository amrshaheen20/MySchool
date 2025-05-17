using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Services.AttendanceContainer.Injector
{
    public class AttendanceInjector : CommandsInjector<Attendance>, IServiceInjector
    {
        public AttendanceInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();


            switch (UserRole)
            {
                case eRole.Teacher:

                    Where(x => x.CreatedById == UserId);
                    break;
                case eRole.Student:
                    Where(x => x.StudentId == UserId);
                    break;

                case eRole.Guardian:
                    var guardianRepo = unitOfWork.GetRepository<StudentGuardian>();
                    var studentIds = guardianRepo
                        .GetAll()
                        .Where(x => x.GuardianId == UserId)
                        .Select(x => x.StudentId);

                    Where(x => studentIds.Contains(x.StudentId));
                    break;

                case eRole.Admin:
                    //do nothing
                    break;

                default:
                    Where(x => x.Id == 0); //return empty list
                    break;
            }

            // AddCommand(q => q.Include(x => x.ClassRoom));
            // AddCommand(q => q.Include(x => x.Student));
            // AddCommand(q => q.Include(x => x.CreatedBy));
            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }
}
