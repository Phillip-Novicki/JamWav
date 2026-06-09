using System.Text;
using JamWav.Domain.Entities;
using JamWav.Infrastructure.Persistence;
using JamWav.Application.Interfaces;
using JamWav.Infrastructure.Persistence.Repositories;
using JamWav.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) EF + single DbContext
builder.Services.AddDbContext<JamWavDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("JamWavDb")));

// 2) Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(opts =>
    {
        opts.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<JamWavDbContext>()
    .AddDefaultTokenProviders();

// 3) JWT-Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    var jwtKey = builder.Configuration["Jwt:Key"]
                 ?? throw new InvalidOperationException("Jwt:Key not configured");
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer           = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidateAudience         = true,
        ValidAudience            = builder.Configuration["Jwt:Audience"],
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero
    };
});

// 4) Swagger + JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JamWav API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.ApiKey,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter your JWT as: Bearer {your token here}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 5) Authorization & Repositories & Services
builder.Services.AddAuthorization();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(JamWav.Application.AssemblyMarker).Assembly));
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IBandRepository, BandRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// 6) Controllers
builder.Services.AddControllers();

var app = builder.Build();

// apply any pending migrations at startup (skipped for InMemory in integration tests)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<JamWavDbContext>();
    if (db.Database.IsRelational())
        db.Database.Migrate();
}

// only show swagger UI outside integration‐test runs
if (!app.Environment.IsEnvironment("IntegrationTests"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JamWav API V1"));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

namespace JamWav.Web
{
    public partial class Program { }
}
