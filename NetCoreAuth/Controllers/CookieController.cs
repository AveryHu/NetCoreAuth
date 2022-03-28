using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreAuth.PolicyProvider;
using System.Security.Claims;

namespace NetCoreAuth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CookieController : ControllerBase
    {
        public CookieController()
        {
        }

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
        /// This API can bee accessed by Rank equals to 3
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "Rank.3")]
        public IActionResult Rank3Secret()
        {
            return Ok("This is Rank 3 protected content");
        }

        /// <summary>
        /// This API can bee accessed by Rank equals to 7
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "Rank.7")]
        public IActionResult Rank7Secret()
        {
            return Ok("This is Rank 7 protected content");
        }

        /// <summary>
        /// This API can bee accessed by SecurityLevel equals to or upper than 5
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SecurityLevel(5)]
        public IActionResult SecutiryLevel5Secret()
        {
            return Ok("This is SecutiryLevel 5 protected content");
        }

        /// <summary>
        /// This API can bee accessed by SecurityLevel equals to or upper than 10
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SecurityLevel(10)]
        public IActionResult SecutiryLevel10Secret()
        {
            return Ok("This is SecutiryLevel 10 protected content");
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

        /// <summary>
        /// Get the Claim with Hey
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AuthenitcateHeyAsync()
        {
            var claims = new List<Claim>()
            {
                new Claim("Hey", "Nice to meet you")
            };

            var identity = new ClaimsIdentity(claims, "Hey Identity");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("DoSomething");
        }

        [HttpGet]
        public async Task<IActionResult> AuthenitcateSecurityLevel8Async()
        {
            var claims = new List<Claim>()
            {
                new Claim(DynamicPolicies.SecurityLevel, "8")
            };

            var identity = new ClaimsIdentity(claims, "SecurityLevel Identity");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("SecutiryLevel5Secret");
        }

        [HttpGet]
        public async Task<IActionResult> AuthenitcateRank7Async()
        {
            var claims = new List<Claim>()
            {
                new Claim("Rank", "7")
            };

            var identity = new ClaimsIdentity(claims, "Rank Identity");

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);

            return RedirectToAction("Rank7Secret");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> DoSomething(
            [FromServices] IAuthorizationService authorizationService)
        {
            var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hey").Build();

            var authResult = await authorizationService.AuthorizeAsync(User, customPolicy);

            if (authResult.Succeeded)
            {
                return Ok("You got hey");
            }

            return BadRequest(authResult);
        }
    }
}