using Microsoft.EntityFrameworkCore;
using MySchool.API.Common;
using MySchool.API.Exceptions;
using System.Net;

namespace MySchool.API.Middlewares
{
    public class ErrorHandlingMiddleware(RequestDelegate _next, ILogger<ErrorHandlingMiddleware> _logger, IWebHostEnvironment env)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (env.IsDevelopment())
                {
                    _logger.LogError(ex, "An unexpected error occurred.");
                    throw;
                }
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error handling middleware will not be executed.");
                    throw;
                }

                _logger.LogError(ex, "An unexpected error occurred.");

                context.Response.Clear();


                var errorResponseMessasge = "An unexpected error occurred. Please try again later.";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                if (ex is NotFoundException NotFoundException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponseMessasge = NotFoundException.Message;
                }
                else if (ex is UnauthorizedAccessException UnauthorizedAccessException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponseMessasge = UnauthorizedAccessException.Message ?? "Unauthorized access.";
                }
                else if (ex is ForbiddenAccessException forbiddenAccess)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponseMessasge = forbiddenAccess.Message ?? "Forbidden access.";
                }
                else if (ex is DbUpdateException dbUpdateEx)
                {
                    if (dbUpdateEx.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
                        dbUpdateEx.InnerException?.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) == true ||
                        dbUpdateEx.InnerException?.Message.Contains("conflict", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                        errorResponseMessasge = "A conflict occurred. The item may already exist.";
                    }
                }

                var response = new BaseResponse()
                    .SetMessage(errorResponseMessasge)
                    .SetStatus((HttpStatusCode)context.Response.StatusCode);

                await context.Response.WriteAsJsonAsync(response);
            }


        }
    }
}

