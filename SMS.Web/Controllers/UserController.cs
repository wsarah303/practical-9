using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using SMS.Data.Services;
using SMS.Data.Models;
using SMS.Web.Models;

namespace SMS.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IStudentService _svc;

        public UserController()
        {
            _svc = new StudentServiceDb();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password")]User m)
        {        
            // call service to Authenticate User
            var user = _svc.Authenticate(m.Email, m.Password);
            // user not authenticated so manually add validation errors for email and password
            if (user == null)
            {
                ModelState.AddModelError("Email", "Invalid Login Credentials");
                ModelState.AddModelError("Password", "Invalid Login Credentials");
                return View(m);
            }
           
            // authenticated so sign user in using cookie authentication to store principal
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                BuildClaimsPrincipal(user)
            );
            return RedirectToAction("Index","Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
 
        public IActionResult Register(/* use bind */ UserViewModel m)
        {
            // if email address is already in use 
            //  add model state error for Email
            // endif

            // if valid modelstate
            //   call service to register user
            //   Add alert indicating success and redirect to login
            // endif

            // redisplay view with validation errors
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult ErrorNotAuthorised()
        {   
            Alert("Not Authorized", AlertType.warning);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorNotAuthenticated()
        {
            Alert("Not Authenticated", AlertType.warning);
            return RedirectToAction("Login", "User"); 
        }        

      
        // ==================================== Build Claims Principle =================================
        // https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-5.0
        // https://andrewlock.net/introduction-to-authentication-with-asp-net-core/
        
        // return claims principal based on authenticated user
        private  ClaimsPrincipal BuildClaimsPrincipal(User user)
        { 
            // define user claims - you can add as many as required
            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())                              
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            // build principal using claims
            return  new ClaimsPrincipal(claims);
        }

    }
}
