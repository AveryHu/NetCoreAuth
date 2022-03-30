using Microsoft.AspNetCore.Authorization;

namespace API.AuthRequirements
{
    public class DefaultRequirement : IAuthorizationRequirement
    { }

    public class DefaultRequirementHandler : AuthorizationHandler<DefaultRequirement>
    {
        private readonly HttpClient _client;
        private readonly HttpContext _httpContext;

        public DefaultRequirementHandler(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _client = httpClientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            DefaultRequirement requirement)
        {
            if (_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                _client.DefaultRequestHeaders.Add("Authorization", authHeader.ToString());

                var verifyResponse = await _client.GetAsync("https://localhost:7262/oauth/verify");

                if (verifyResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}