using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreAuth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class OperationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public OperationController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Open()
        {
            var cookieJar = new CookieJar();
            var authResult = await _authorizationService.AuthorizeAsync(User, cookieJar, CookieJarAuthOperations.Open);
            if (authResult.Succeeded)
                return Ok("Open cookie");
            return BadRequest();
        }
    }

    public class CookieJarAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, CookieJar>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            CookieJar cookieJar)
        {
            if (requirement.Name == CookieJarOperations.Open)
            {
                if ((context.User.Identity.IsAuthenticated)
                    && (cookieJar.Name.NotNullorEmpty())
                    && (cookieJar.Name.Equals("Admin")))
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement.Name == CookieJarOperations.TakeCookie)
            {
                if (context.User.HasClaim("Hey", "Admin"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }

    public static class CookieJarAuthOperations
    {
        public static OperationAuthorizationRequirement Open = new OperationAuthorizationRequirement
        {
            Name = CookieJarOperations.Open
        };
    }

    public static class CookieJarOperations
    {
        public static string Open = "Open";
        public static string TakeCookie = "TakeCookie";
    }

    public class CookieJar
    {
        public string Name { get; set; }
    }

    public static class StringExtensions
    {
        public static bool NotNullorEmpty(this string _)
        {
            return !string.IsNullOrEmpty(_);
        }
    }
}