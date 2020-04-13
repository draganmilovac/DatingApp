using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repos = resultContext.HttpContext.RequestServices.GetServices<IDatingRepository>();
            foreach(var repo in repos)
            {
                var user =await repo.GetUser(userId);
                user.LastActive = DateTime.Now;
                await repo.SaveAll();
            }
        }
    }
}