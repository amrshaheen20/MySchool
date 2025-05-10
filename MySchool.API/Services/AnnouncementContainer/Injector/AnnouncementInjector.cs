using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Services.AnnouncementContainer.Injector
{
    public class AnnouncementInjector : CommandsInjector<Announcement>, IServiceInjector
    {
        public AnnouncementInjector(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();


            switch (UserRole)
            {
                case eRole.Student:
                    AddCommand(q => q.Where(x => x.UserId == UserId));
                    break;

                case eRole.Guardian:
                    var guardianRepo = unitOfWork.GetRepository<StudentGuardian>();
                    var studentIds = guardianRepo
                        .GetAll()
                        .Where(x => x.GuardianId == UserId)
                        .Select(x => x.StudentId);

                    AddCommand(query => query.Where(x => studentIds.Contains(x.UserId)));
                    break;

                case eRole.Teacher:
                    AddCommand(q => q.Where(x => x.CreatedById == UserId));//return all announcements created by the teacher
                    break;

                case eRole.Admin:
                    //do nothing
                    break;

                default:
                    AddCommand(q => q.Where(x => x.Id == 0));//return empty list
                    break;
            }





            // AddCommand(q => q.Include(x => x.Student));
            // AddCommand(q => q.Include(x => x.CreatedBy));
            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }
}
