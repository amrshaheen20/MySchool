using MySchool.API.Enums;
using MySchool.API.Models.DbSet;
using System.Security.Claims;

namespace MySchool.API.Extensions
{
    public static class UserContextHelper
    {
        public static int GetUserId(this IHttpContextAccessor contextAccessor)
        {
            var userIdClaim = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim == null ? throw new UnauthorizedAccessException("User not found in context.") : int.Parse(userIdClaim);
        }

        public static eRole GetUserRole(this IHttpContextAccessor contextAccessor)
        {
            var roleClaim = contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim == null ? throw new UnauthorizedAccessException("Role not found in context.") : Enum.Parse<eRole>(roleClaim);
        }

        public static string GetTokenJti(this IHttpContextAccessor contextAccessor)
        {
            var jtiClaim = contextAccessor.HttpContext?.User.FindFirst("jti");
            return jtiClaim == null ? throw new UnauthorizedAccessException("User is not authenticated") : jtiClaim.Value;
        }

        public static User GetCurrentUser(this IHttpContextAccessor contextAccessor)
        {
            return (User)contextAccessor.HttpContext!.Items[HttpContextItemKeys.CurrentUser]!;
        }

        public static User GetCurrentUser(this HttpContext httpContext)
        {
            return (User)httpContext.Items[HttpContextItemKeys.CurrentUser]!;
        }
    }
}
