using Microsoft.AspNetCore.Diagnostics;
using MySchool.API.Common;
using System.Net;

namespace MySchool.API.Extensions
{
    public static partial class ApiExtensions
    {
        private static string[] memes = new[]
        {
                    "Holla master Sherlock, it looks like the page is playing hide and seek!",
                    "Elementary, my dear Watson. The page has disappeared.",
                    "Watson, it appears the page you seek has gone into hiding.",
                    "Sherlock never misses a clue, but it seems the page has gone missing.",
                    "Ah, a mystery indeed. The page has vanished into the void.",
                    "Error 404: Page went out for snacks and forgot to return."
         };

        public static void UseErrorHandler(this IApplicationBuilder app)
        {
            app.UseStatusCodePagesWithRedirects("/error?status={0}");

            app.Map("/error", errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var statusCode = int.Parse(context.Request.Query["status"]!);

                    var response = new BaseResponse(
                        (HttpStatusCode)statusCode
                     , context.Features.Get<IExceptionHandlerFeature>()?.Error.Message
                    );

                    if (statusCode == 404)
                    {
                        response.SetMessage(memes[Random.Shared.Next(memes.Length)]);
                    }
                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsJsonAsync(response, ApiExtensions.JsonSerializerOptions);
                });
            });
        }
    }
}
