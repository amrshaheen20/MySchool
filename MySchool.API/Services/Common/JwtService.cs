using Microsoft.IdentityModel.Tokens;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MySchool.API.Services.Common
{
    public class MyJwtSecurityToken
    {
        public MyJwtSecurityToken(JwtSecurityToken securityToken)
        {
            Token = securityToken;
        }

        public JwtSecurityToken Token { get; }

        public string GenerateToken()
        {
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
    }

    public class JwtService(IConfiguration config) : IServiceInjector
    {

        public MyJwtSecurityToken GenerateJwtToken(User user)
        {
            int.TryParse(config["JWT:ExpiryDays"], out int days);
            if (days <= 0)
                days = 30;

            var expires = DateTime.UtcNow.Add(TimeSpan.FromDays(days));

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Expired, expires.ToString()),
                new Claim("ForceChangePassword",user.MustChangePassword.ToString()),
            };


            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecretKey"]!));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: config["JWT:ValidIssuer"],
                audience: config["JWT:ValidAudience"],
                expires: expires,
                claims: claims,
                signingCredentials: signingCredentials
            );

            return new MyJwtSecurityToken(jwtToken);
        }


    }
}
