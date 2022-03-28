using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NetCoreAuth.AuthRequirements;
using NetCoreAuth.Controllers;
using NetCoreAuth.PolicyProvider;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            var secretBytes = Encoding.UTF8.GetBytes(NetCoreAuth.Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);

            options.Events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Query.ContainsKey("access_token"))
                    {
                        context.Token = context.Request.Query["access_token"];
                    }
                    return Task.CompletedTask;
                }
            };

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = NetCoreAuth.Constants.Issuer,
                ValidAudience = NetCoreAuth.Constants.Audiance,
                IssuerSigningKey = key,
            };
        })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("CookieSettings", options));

builder.Services.AddAuthorization(config =>
{
    // Policy base role check
    config.AddPolicy("MultiRoles", policyBuilder => { policyBuilder.RequireRole("Backend", "Admin"); });
    config.AddPolicy("Birth", policyBuilder =>
    {
        policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });
});

builder.Services.AddSingleton<IAuthorizationPolicyProvider, MyAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();

builder.Services.AddScoped<IAuthorizationHandler, BirthRequireClaimHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();