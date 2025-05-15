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
using JamWav.Web.Tests.Integration.Auth;  

namespace JamWav.Web.Tests.Integration.Utils
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // configure the test host for detailed errors and logging
            builder
                .UseEnvironment("Development")
                .CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true")
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                });

            // 1) Provide minimal Jwt config so Program.cs doesn’t blow up
            builder.ConfigureAppConfiguration((ctx, config) =>
            {
                var jwtSettings = new Dictionary<string, string?>
                {
                    ["Jwt:Key"]      = "TestJwtKey123456789012345678901234567890",
                    ["Jwt:Issuer"]   = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience"
                };
                config.AddInMemoryCollection(jwtSettings);
            });

            builder.ConfigureServices(services =>
            {
                // 2) Swap out your real DbContext for EF InMemory
                var real = services.Single(
                    d => d.ServiceType == typeof(DbContextOptions<JamWavDbContext>)
                );
                services.Remove(real);

                services.AddDbContext<JamWavDbContext>(opts =>
                    opts.UseInMemoryDatabase("IntegrationDb"));

                // 3) Override the authentication scheme entirely
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "Test", options => { }
                    );

                // enforce our Test scheme as the default
                services.PostConfigure<AuthenticationOptions>(opts =>
                {
                    opts.DefaultAuthenticateScheme = "Test";
                    opts.DefaultChallengeScheme    = "Test";
                });

                // 4) Build and seed the in‑memory database
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<JamWavDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            });
        }
    }
}
