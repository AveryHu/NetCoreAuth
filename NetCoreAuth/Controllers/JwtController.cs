using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetCoreAuth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class JwtController : ControllerBase
    {
        [HttpGet]
        public IActionResult Login()
        {
            return Ok("Call Authenitcate to log in ");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Secret()
        {
            return Ok("This is protected content");
        }

        [HttpGet]
        public async Task<IActionResult> AuthenitcateAsync()
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
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { access_token = tokenJson });
        }
    }
}