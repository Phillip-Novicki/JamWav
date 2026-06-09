using JamWav.Infrastructure.Persistence;
using JamWav.Web.Tests.Integration.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JamWav.Web.Tests.Integration.Utils;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureLogging(log =>
        {
            log.ClearProviders();
            log.AddConsole();
            log.SetMinimumLevel(LogLevel.Warning);
        });

        builder.ConfigureServices(services =>
        {
            // Replace SQL Server DbContext with InMemory
            services.RemoveAll<DbContextOptions<JamWavDbContext>>();
            services.AddDbContext<JamWavDbContext>(opts =>
                opts.UseInMemoryDatabase("IntegrationTestDb"));

            // Replace JWT auth with a passthrough test handler
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            services.PostConfigure<AuthenticationOptions>(opts =>
            {
                opts.DefaultAuthenticateScheme = "Test";
                opts.DefaultChallengeScheme    = "Test";
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Seed the in-memory database using the real app's service provider
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<JamWavDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        return host;
    }
}
