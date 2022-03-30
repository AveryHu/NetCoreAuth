using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult NeedToken()
        {
            return Ok("You get my secret");
        }
    }
}