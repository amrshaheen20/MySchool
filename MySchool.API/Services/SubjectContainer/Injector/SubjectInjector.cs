using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Services.SubjectContainer.Injector
{
    public class SubjectInjector : CommandsInjector<Subject>, IServiceInjector
    {
        public SubjectInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {

            var userId = contextAccessor.GetUserId();
            var userRole = contextAccessor.GetUserRole();

            switch (userRole)
            {
                case Enums.eRole.Admin:
                    //do nothing
                    break;
                case Enums.eRole.Teacher:
                    Where(x => x.Timetables.Any(t => t.TeacherId == userId));
                    break;
                case Enums.eRole.Student:
                    Where(x => x.Timetables.Any(s => s.ClassRoom.Enrollments.Any(e => e.StudentId == userId)));
                    break;
                case Enums.eRole.Guardian:

                    var guardianRepo = unitOfWork.GetRepository<StudentGuardian>();
                    var studentIds = guardianRepo
                        .GetAll()
                        .Where(x => x.GuardianId == userId)
                        .Select(x => x.StudentId);

                    Where(x => x.Timetables.Any(s => s.ClassRoom.Enrollments.Any(e => studentIds.Contains(e.StudentId))));
                    break;
                default:
                    Where(x => x.Id == 0); //return empty list
                    break;
            }




            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }
}
