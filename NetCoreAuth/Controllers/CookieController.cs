using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NetCoreAuth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CookieController : ControllerBase
    {
        [HttpGet]
        public IActionResult Login()
        {
            return Ok("Call Authenitcate to log in ");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Secret()
        {
            return Ok("This is protected content");
        }

        [HttpGet]
        public async Task<IActionResult> AuthenitcateAsync()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Visitor")
            };

            var identity = new ClaimsIdentity(claims, "VisitorId");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Secret");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}