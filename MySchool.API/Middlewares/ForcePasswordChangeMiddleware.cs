using MySchool.API.Exceptions;
using MySchool.API.Extensions;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;

namespace MySchool.API.Middlewares
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AllowIfPasswordNeedToChangeAttribute : Attribute
    {

    }

    public class ForcePasswordChangeMiddleware
    {
        private readonly RequestDelegate _next;

        public ForcePasswordChangeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }



            var user = context.Items[HttpContextItemKeys.CurrentUser] as User;
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found in context.");
                ;
            }

            if (user.MustChangePassword)
            {
                var endpoint = context.GetEndpoint();
                var isAllowed = endpoint?.Metadata.GetMetadata<AllowIfPasswordNeedToChangeAttribute>() != null;

                if (!isAllowed)
                {
                    throw new ForbiddenAccessException("Password change required.");
                }
            }

            await _next(context);
        }
    }


}
