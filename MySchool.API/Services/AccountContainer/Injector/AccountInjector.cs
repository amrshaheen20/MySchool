using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.DbSet.ClassRoomEntities;

namespace MySchool.API.Services.AccountContainer.Injector
{
    public class AccountInjector : CommandsInjector<User>, IServiceInjector
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUnitOfWork unitOfWork;

        public AccountInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            //AddCommand(q => q.Include(x => x.CustomFields));
            AddCommand(q => q.OrderByDescending(x => x.Id));
            this.contextAccessor = contextAccessor;
            this.unitOfWork = unitOfWork;
        }

        public void ConversationPeopleInject()
        {
            var userId = contextAccessor.GetUserId();
            var userRole = contextAccessor.GetUserRole();
            switch (userRole)
            {
                case Enums.eRole.Admin:
                    //do nothing
                    break;
                case Enums.eRole.Student: // student can see only his teachers

                    var teacherIds = unitOfWork.GetRepository<Enrollment>()
                        .GetAll()
                        .Where(e => e.StudentId == userId)
                        .SelectMany(e => e.ClassRoom.Timetables)
                        .Select(t => t.TeacherId)
                        .Distinct();

                    Where(x => teacherIds.Contains(x.Id));
                    break;
                case Enums.eRole.Teacher:// teacher can see only his students and guardians

                    var studentIds = unitOfWork.GetRepository<Enrollment>()
                        .GetAll()
                        .Where(e => e.ClassRoom.Timetables.Any(t => t.TeacherId == userId))
                        .Select(e => e.StudentId)
                        .Distinct();
                    var guardianIds = unitOfWork.GetRepository<StudentGuardian>()
                        .GetAll()
                        .Where(e => studentIds.Contains(e.StudentId))
                        .Select(e => e.GuardianId)
                        .Distinct();

                    Where(x => studentIds.Contains(x.Id) || guardianIds.Contains(x.Id));
                    break;
                case Enums.eRole.Guardian:// guardian can see teachers of his children
                    var studentIdsForGuardian = unitOfWork.GetRepository<StudentGuardian>()
                        .GetAll()
                        .Where(e => e.GuardianId == userId)
                        .Select(e => e.StudentId)
                        .Distinct();

                    var teacherIdsForGuardian = unitOfWork.GetRepository<Enrollment>()
                        .GetAll()
                        .Where(e => studentIdsForGuardian.Contains(e.StudentId))
                        .SelectMany(e => e.ClassRoom.Timetables)
                        .Select(t => t.TeacherId)
                        .Distinct();

                    Where(x => teacherIdsForGuardian.Contains(x.Id));
                    break;
                default:
                    Where(x => x.Id == 0); //return empty list
                    break;
            }
        }

    }
}