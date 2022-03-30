using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Try to get protected resource
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var claims = HttpContext.User.Claims;

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await _client.GetAsync("https://localhost:7110/secret/needtoken");

            return Ok(await response.Content.ReadAsStringAsync());
        }
    }
}