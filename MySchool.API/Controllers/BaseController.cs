using Microsoft.AspNetCore.Mvc;
using MySchool.API.Common;
using MySchool.API.Interfaces;

namespace MySchool.API.Controllers
{
    [Consumes("application/json"), Produces("application/json")]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public virtual IActionResult BuildResponse<T>(IBaseResponse<T> response)
        {
            if ((int)response.Status >= 400 || response.Data == null && response.Message != null)
            {
                return StatusCode((int)response.Status, response);
            }

            if (response.Data != null)
            {
                if (response.Data is { } data)
                {
                    var dataType = data.GetType();

                    if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(PaginateBlock<>))
                    {
                        var paginateBlock = data as dynamic;

                        if (paginateBlock.IsList)
                        {
                            return StatusCode((int)response.Status, paginateBlock.Data);
                        }
                    }
                }


                return StatusCode((int)response.Status, response.Data);
            }


            return StatusCode((int)response.Status);
        }
    }
}