// JamWav.Web.Tests/Integration/Utils/CustomWebApplicationFactory.cs
using System.Collections.Generic;
using System.Linq;
using JamWav.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JamWav.Web.Tests.Integration.Auth;  // <-- where your TestAuthHandler lives

namespace JamWav.Web.Tests.Integration.Utils
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // 1) Tell the host “we’re running integration tests”
            builder.UseEnvironment("IntegrationTests")
                   .CaptureStartupErrors(true)
                   .ConfigureLogging(log =>
                   {
                       log.ClearProviders();
                       log.AddConsole();
                       log.SetMinimumLevel(LogLevel.Debug);
                   });

            // 2) Provide in‐memory JWT settings so Program.cs can bind Jwt:Key, etc.
            builder.ConfigureAppConfiguration((ctx, cfg) =>
            {
                var jwtSettings = new Dictionary<string, string?>
                {
                    ["Jwt:Key"]      = "TestJwtKey123456789012345678901234567890",
                    ["Jwt:Issuer"]   = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience"
                };
                cfg.AddInMemoryCollection(jwtSettings);
            });

            // 3) Swap out your real DbContext + real auth for in‐memory versions
            builder.ConfigureServices(services =>
            {
                // --- a) Replace SQL Server with EF InMemory
                var dbDescriptor = services
                    .Single(d => d.ServiceType == typeof(DbContextOptions<JamWavDbContext>));
                services.Remove(dbDescriptor);
                services.AddDbContext<JamWavDbContext>(opts =>
                    opts.UseInMemoryDatabase("IntegrationDb"));

                // --- b) Override the authentication scheme
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", _ => { });
                services.PostConfigure<AuthenticationOptions>(opts =>
                {
                    opts.DefaultAuthenticateScheme = "Test";
                    opts.DefaultChallengeScheme    = "Test";
                });

                // --- c) Build & seed an empty in‐memory database
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<JamWavDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        }
    }
}

