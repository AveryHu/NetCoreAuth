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

        /// <summary>
        /// This API can be accessed by the one who is authenticated
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public IActionResult Secret()
        {
            return Ok("This is protected content");
        }

        /// <summary>
        /// This API can only be accessed by the one who has Birth Claims
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "Birth")]
        public IActionResult BirthSecret()
        {
            return Ok("This is Birth protected content");
        }

        /// <summary>
        /// This API can only be accessed by Backend Role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Backend")]
        public IActionResult RoleSecret()
        {
            return Ok("This is Role protected content");
        }

        /// <summary>
        /// This API can be accessed by Backend or Admin Role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "MultiRoles")]
        public IActionResult MultiRoleSecret()
        {
            return Ok("This is MultiRole protected content");
        }

        /// <summary>
        /// Get the Claim which has Name
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get the Claim which has Birth
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AuthenitcateBirthAsync()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.DateOfBirth, "1995/10/10")
            };

            var identity = new ClaimsIdentity(claims, "Birth Identity");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("BirthSecret");
        }

        /// <summary>
        /// Get the Claim which has Backend Role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AuthenitcateRoleAsync()
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role, "Backend")
            };

            var identity = new ClaimsIdentity(claims, "Backend Identity");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("RoleSecret");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}