using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MySchool.API.Common;
using MySchool.API.Interfaces;
using MySchool.API.Models.DbSet;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MySchool.API.Extensions
{
    public static partial class ApiExtensions
    {
        public static void AddJwtConfiguration(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidateAudience = false,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Headers["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var cancellationToken = context.HttpContext.RequestAborted;
                        var unitOfWork = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                        var claimsPrincipal = context.Principal;
                        var userId = claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        var TokenId = claimsPrincipal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                        if (userId == null)
                        {
                            context.Fail("Invalid token. name identifier not found");
                            return;
                        }

                        if (TokenId == null)
                        {
                            context.Fail("Invalid token. jti not found");
                            return;
                        }


                        var user = await unitOfWork.GetRepository<User>().GetByIdAsync(int.Parse(userId));

                        context.HttpContext.Items[HttpContextItemKeys.CurrentUser] = user;


                        var command = new CommandsInjector<UserSession>();
                        command.Where(x => x.UserId == int.Parse(userId) && x.TokenId == TokenId && x.ExpirationTime >= DateTime.UtcNow);

                        var token = await unitOfWork.GetRepository<UserSession>().GetByAsync(command, cancellationToken);


                        if (user is null || token is null)
                        {
                            context.Fail("Invalid or expired token. Please log in again.");
                            return;
                        }

                        if (user.MustChangePassword)
                        {
                            var identity = (ClaimsIdentity)context.Principal?.Identity!;
                            identity.AddClaim(new Claim("ForceChangePassword", "true"));
                        }

                        user.LastActiveTime = DateTime.UtcNow;
                        await unitOfWork.SaveAsync(cancellationToken);

                    }
                };
            });
        }
    }
}
