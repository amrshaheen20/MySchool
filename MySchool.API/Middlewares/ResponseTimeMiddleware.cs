using System.Diagnostics;

namespace MySchool.API.Middlewares
{
    public class ResponseTimeMiddleware(RequestDelegate _next)
    {
        public async Task Invoke(HttpContext context)
        {
            var watch = Stopwatch.StartNew();

            context.Response.OnStarting(() =>
            {
                watch.Stop();
                var responseTime = watch.ElapsedMilliseconds;
                context.Response.Headers["X-Response-Time-ms"] = responseTime.ToString();
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
