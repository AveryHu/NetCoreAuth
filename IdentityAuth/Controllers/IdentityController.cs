using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAuth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class IdentityController : ControllerBase
    {
        public record UserInfo(string Username, string Password);

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IdentityController(UserManager<IdentityUser> userManager
            , SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Hello()
        {
            return Ok("Hello");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] UserInfo userInfo)
        {
            var user = new IdentityUser
            {
                UserName = userInfo.Username,
            };

            var result = await _userManager.CreateAsync(user, userInfo.Password);

            if (result.Succeeded)
            {
                return Ok("Register succeeded");
            }

            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] UserInfo userInfo)
        {
            var user = await _userManager.FindByNameAsync(userInfo.Username);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, userInfo.Password, false, false);

                if (result.Succeeded)
                {
                    return Ok("Log in succeeded");
                }
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Hello");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Secret()
        {
            return Ok("This is protected content");
        }
    }
}