using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Services.MessageContainer.Injector
{
    public class MessageInjector : CommandsInjector<Message>, IServiceInjector
    {
        public MessageInjector(IHttpContextAccessor contextAccessor)
        {
            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();
            /*
            Scenario Case:
             -  If UserRole is Admin, show all messages we have no privacy :)
             -  Other wise, show only messages that belong to the user
            */

            if (UserRole != eRole.Admin)
            {
                Where(x => x.Conversation.UserOneId == UserId || x.Conversation.UserTwoId == UserId);
            }



            AddCommand(q => q.Include(x => x.Conversation));
            AddCommand(q => q.OrderByDescending(x => x.Id));
        }
    }

}
