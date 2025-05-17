using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MySchool.API.Common;
using MySchool.API.DataSeed;
using System.Net;

namespace MySchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController(DataBaseContext context, IMemoryCache cache) : BaseController
    {
        private string cacheKey = "lastDatabaseReset";

        /// <summary>
        /// Rest and seed the database.
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetDatabase()
        {
            if (cache.TryGetValue(cacheKey, out _))
            {
                return BuildResponse(new BaseResponse()
                    .SetStatus(HttpStatusCode.TooManyRequests)
                    .SetMessage("You can only reset the database once every 24 hours."));
            }

            try
            {
                await DataSeeder.Instance.ResetDatabaseWithGeneratedDataAsync(context);

                cache.Set(cacheKey, true, TimeSpan.FromHours(24));

                return BuildResponse(new BaseResponse()
                    .SetStatus(HttpStatusCode.OK)
                    .SetMessage("Database reset and seeded successfully."));
            }
            catch
            {
                return BuildResponse(new BaseResponse()
                    .SetStatus(HttpStatusCode.InternalServerError)
                    .SetMessage("An error occurred while resetting the database."));
            }
        }
    }
}
