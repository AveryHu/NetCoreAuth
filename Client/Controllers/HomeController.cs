using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
            var serverResponse = await AccessTokenRefreshWrapper(
                () => SecuredGetRequest("https://localhost:7262/home/secret"));

            var response = await AccessTokenRefreshWrapper(
                () => SecuredGetRequest("https://localhost:7110/secret/needtoken"));

            return Ok(await response.Content.ReadAsStringAsync());
        }

        private async Task<HttpResponseMessage> SecuredGetRequest(string url)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> AccessTokenRefreshWrapper(
            Func<Task<HttpResponseMessage>> initialRequest
            )
        {
            var response = await initialRequest();

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await RefreshAccessToken();
                response = await initialRequest();
            }

            return response;
        }

        private async Task RefreshAccessToken()
        {
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var refreshTokenClient = _httpClientFactory.CreateClient();

            var requestData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7262/oauth/token")
            {
                Content = new FormUrlEncodedContent(requestData)
            };

            var basicCredentials = "username:password";
            var encodeCredentials = Encoding.UTF8.GetBytes(basicCredentials);
            var base64Credentials = Convert.ToBase64String(encodeCredentials);

            request.Headers.Add("Authorization", $"Basic {base64Credentials}");

            var response = await refreshTokenClient.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

            var newAccessToken = responseData.GetValueOrDefault("access_token");
            var newRefreshToken = responseData.GetValueOrDefault("refresh_token");

            var authInfo = await HttpContext.AuthenticateAsync("ClientCookie");

            authInfo.Properties.UpdateTokenValue("access_token", newAccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", newRefreshToken);

            await HttpContext.SignInAsync("ClientCookie", authInfo.Principal, authInfo.Properties);
        }
    }
}