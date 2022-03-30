using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {
        /// <summary>
        /// Authorization Grant
        /// </summary>
        /// <param name="response_type"></param>
        /// <param name="client_id"></param>
        /// <param name="redirect_uri"></param>
        /// <param name="scope"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Authorize(
            string response_type,
            string client_id,
            string redirect_uri,
            string scope,
            string state)
        {
            var query = new QueryBuilder();
            query.Add("redirectUri", redirect_uri);
            query.Add("state", state);

            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string username,
            string password,
            string redirectUri,
            string state)
        {
            string code = Guid.NewGuid().ToString();

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);

            return Redirect($"{redirectUri}{query.ToString()}");
        }

        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id,
            string refresh_token)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "id"),
                new Claim("hello", "hero")
            };

            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: grant_type == "refresh_token"
                    ? DateTime.Now.AddSeconds(30)
                    : DateTime.Now.AddSeconds(10),
                signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "OAuth",
                refresh_token = "ThisIsUseToRefresh"
            };

            return Ok(responseObject);
        }

        [HttpGet]
        public IActionResult Verify()
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var value))
            {
                var accessToken = value.ToString().Split(' ')[1];

                var base64Payload = accessToken.Split('.')[1];
                base64Payload = base64Payload.Replace('-', '+').Replace('_', '/').PadRight(4 * ((base64Payload.Length + 3) / 4), '=');
                var bytes = Convert.FromBase64String(base64Payload);
                var jsonPayload = Encoding.UTF8.GetString(bytes);
                var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                if (claims.TryGetValue("exp", out string expireString))
                {
                    DateTime expireTime = UnixTimeStampToDateTime(double.Parse(expireString));

                    return DateTime.Compare(DateTime.Now, expireTime) > 0 ? BadRequest() : Ok();
                }
            }
            return BadRequest();
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}