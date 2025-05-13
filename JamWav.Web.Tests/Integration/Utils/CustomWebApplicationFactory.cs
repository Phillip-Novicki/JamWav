// JamWav.Web.Tests/Utils/CustomWebApplicationFactory.cs
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using JamWav.Infrastructure.Persistence;

namespace JamWav.Web.Tests.Utils
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => builder.ConfigureServices(services =>
            {
                // 1) remove the real DbContext registration
                var descriptor = services
                    .FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<JamWavDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // 2) re‑add it pointing at a fresh in‑memory SQLite
                var builderString = new SqliteConnectionStringBuilder
                {
                    DataSource = ":memory:",
                    Mode       = SqliteOpenMode.Memory,
                    Cache      = SqliteCacheMode.Shared
                }.ToString();

                var connection = new SqliteConnection(builderString);
                connection.Open();

                services.AddDbContext<JamWavDbContext>(opts =>
                    opts.UseInMemoryDatabase("IntegrationDb"));

                // 3) create the schema once
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<JamWavDbContext>();
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            });
    }
}