using IdentityAuth.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<InMemoryContext>(config =>
{
    config.UseInMemoryDatabase("IdentityAuth");
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
{
    config.Password.RequiredLength = 6;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
    config.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<InMemoryContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
    builder.Configuration.Bind("CookieSettings", options));

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