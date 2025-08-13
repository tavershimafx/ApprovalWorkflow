using ApprovalSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ApprovalSystem.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Returns the currently logged in user with valid credentials in the session
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="AggregateException"></exception>
        public async static Task<User> GetCurrentUser(this IHttpContextAccessor context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var nameClaim = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                var usr = await userManager.FindByNameAsync(nameClaim) ?? throw new AggregateException();

                return usr;
            }

            throw new AggregateException("User not logged in. Cannot retrieve a user in an unauthenticated session");
        }
    }
}
