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
    });

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