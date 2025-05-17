using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Services.GradeContainer.Injector
{
    public class GradeInjector : CommandsInjector<Grade>, IServiceInjector
    {
        public GradeInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();


            switch (UserRole)
            {
                case Enums.eRole.Admin:
                    //do nothing
                    break;

                case Enums.eRole.Teacher:
                    var enrollmentRepo = unitOfWork.GetRepository<Enrollment>();
                    var studentsOfTeacher = enrollmentRepo
                        .GetAll()
                        .Where(x => x.ClassRoom.Timetables.Any(t => t.TeacherId == UserId))
                        .Select(x => x.StudentId);

                    Where(x => studentsOfTeacher.Contains(x.StudentId) || x.CreatedById == UserId);
                    break;
                case Enums.eRole.Student:
                    Where(x => x.StudentId == UserId && x.IsPublished);
                    break;

                case Enums.eRole.Guardian:
                    var guardianRepo = unitOfWork.GetRepository<StudentGuardian>();
                    var studentIds = guardianRepo
                        .GetAll()
                        .Where(x => x.GuardianId == UserId)
                        .Select(x => x.StudentId);

                    Where(x => studentIds.Contains(x.StudentId));
                    break;

                default:
                    AddCommand(q => q.Where(x => x.Id == 0)); //return empty list
                    break;

            }


            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }
}
