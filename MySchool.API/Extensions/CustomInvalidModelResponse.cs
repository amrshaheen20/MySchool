using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using System.Net;

namespace MySchool.API.Extensions
{
    public static class CustomInvalidModelResponse
    {
        public static IActionResult Generate(ActionContext context)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(kvp => kvp.Value!.Errors.Select(e => new
                {
                    Field = kvp.Key,
                    Message = e.ErrorMessage
                }));

            var response = new BaseResponse<string>(
                HttpStatusCode.BadRequest,
                "Validation Failed",
               errors: errors
            );

            return new BadRequestObjectResult(response);
        }
    }

}
