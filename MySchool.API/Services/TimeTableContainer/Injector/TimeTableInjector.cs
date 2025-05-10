using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ClassRoomEntities;

namespace MySchool.API.Services.TimeTableContainer.Injector
{
    public class TimeTableInjector : CommandsInjector<Timetable>, IServiceInjector
    {
        public TimeTableInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();
            switch (UserRole)
            {
                case eRole.Teacher:
                    Where(x => x.TeacherId == UserId);
                    break;
                case eRole.Student:
                    Where(x => x.ClassRoom.Enrollments.Any(s => s.StudentId == UserId));
                    break;
                case eRole.Guardian:

                    var guardianRepo = unitOfWork.GetRepository<StudentGuardian>();
                    var studentIds = guardianRepo
                        .GetAll()
                        .Where(x => x.GuardianId == UserId)
                        .Select(x => x.StudentId);
                    Where(x => x.ClassRoom.Enrollments.Any(s => studentIds.Contains(s.StudentId)));
                    break;
                case eRole.Admin:
                    //do nothing
                    break;
                default:
                    Where(x => x.Id == 0); //return empty list
                    break;
            }
            AddCommand(q => q.Include(x => x.Subject));
            AddCommand(q => q.Include(x => x.Teacher));
            AddCommand(q => q.Include(x => x.ClassRoom));
            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }
}
