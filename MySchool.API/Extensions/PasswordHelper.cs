
namespace MySchool.API.Extensions
{
    public static class PasswordHelper
    {
        public static string HashPassword(this string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(this string hashPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }

    }
}
