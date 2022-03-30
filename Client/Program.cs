using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = "ClientCookie";
    config.DefaultSignInScheme = "ClientCookie";
    config.DefaultChallengeScheme = "OAuthServer";
})
    .AddCookie("ClientCookie")
    .AddOAuth("OAuthServer", config =>
    {
        config.ClientId = "client_id";
        config.ClientSecret = "client_secret";
        config.CallbackPath = "/oauth/callback";
        config.AuthorizationEndpoint = "https://localhost:7262/oauth/authorize";
        config.TokenEndpoint = "https://localhost:7262/oauth/token";

        config.SaveTokens = true;

        config.Events = new OAuthEvents()
        {
            OnCreatingTicket = context =>
            {
                var accessToken = context.AccessToken;
                var base64Payload = accessToken.Split('.')[1];
                base64Payload = base64Payload.Replace('-', '+').Replace('_', '/').PadRight(4 * ((base64Payload.Length + 3) / 4), '=');
                var bytes = Convert.FromBase64String(base64Payload);
                var jsonPayload = Encoding.UTF8.GetString(bytes);
                var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                foreach (var claim in claims)
                    context.Identity.AddClaim(new Claim(claim.Key, claim.Value));

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();