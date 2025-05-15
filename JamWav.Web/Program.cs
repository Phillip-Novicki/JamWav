using System.Text;
using JamWav.Domain.Entities;
using JamWav.Infrastructure.Persistence;
using JamWav.Application.Interfaces;
using JamWav.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1) EF + your single DbContext
builder.Services.AddDbContext<JamWavDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("JamWavDb")));

// 2) Identity over that DbContext
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = true;
        // tweak password strength etc here...
    })
    .AddEntityFrameworkStores<JamWavDbContext>()
    .AddDefaultTokenProviders();

// 3) JWT‑Bearer configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    var key = builder.Configuration["Jwt:Key"]
              ?? throw new InvalidOperationException("Jwt:Key not configured");
    var keyBytes = Encoding.UTF8.GetBytes(key);
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer           = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidateAudience         = true,
        ValidAudience            = builder.Configuration["Jwt:Audience"],
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero
    };
});

// 3.1) register authorization (so [Authorize] actually works)
builder.Services.AddAuthorization();

// 3.2) register application repositories for DI
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();

// 4) MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// only enable swagger when *not* running under your IntegrationTests environment
if (!app.Environment.IsEnvironment("IntegrationTests"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

namespace JamWav.Web
{
    // expose the generated Program class so your tests can tie into it
    public partial class Program { }
}
