
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace SMS.Web
{
    public static class Extensions
    {       
        // AddCookieAuthentication extension method - to be called in Startup service configuration
        public static void AddCookieAuthentication(
            this IServiceCollection services, 
            string notAuthorised = "/User/ErrorNotAuthorised", 
            string notAuthenticated= "/User/ErrorNotAuthenticated"
        )
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => {
                        options.AccessDeniedPath = notAuthorised;
                        options.LoginPath = notAuthenticated;
            });
        }        
    }
}
