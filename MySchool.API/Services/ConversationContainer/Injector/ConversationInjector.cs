using MySchool.API.Common;
using MySchool.API.Enums;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Services.ConversationContainer.Injector
{
    public class ConversationInjector : CommandsInjector<Conversation>, IServiceInjector
    {
        public ConversationInjector(IHttpContextAccessor contextAccessor)
        {
            var UserRole = contextAccessor.GetUserRole();
            var UserId = contextAccessor.GetUserId();
            /*
            Scenario Case:
             -  If UserRole is Admin, show all conversations
             -  Other wise, show only conversations where the user is either UserOne or UserTwo
            */

            if (UserRole != eRole.Admin)
            {
                Where(x => x.UserOneId == UserId || x.UserTwoId == UserId);
            }

            AddCommand(q => q.OrderByDescending(x => x.Id));
            //AddCommand(q => q.Include(x => x.UserOne));
            //AddCommand(q => q.Include(x => x.UserTwo));
        }
    }


}

