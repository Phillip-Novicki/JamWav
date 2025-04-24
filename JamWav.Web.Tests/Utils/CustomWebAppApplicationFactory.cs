using System;
using System.Linq;
using JamWav.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JamWav.Web.Tests.Utils;

public class CustomWebAppApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.CurrentDirectory = AppContext.BaseDirectory;   // key path fix

        builder.ConfigureServices(services =>
        {
            var old = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<JamWavDbContext>));
            if (old is not null) services.Remove(old);

            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();

            services.AddDbContext<JamWavDbContext>(o => o.UseSqlite(conn));

            using var scope = services.BuildServiceProvider().CreateScope();
            scope.ServiceProvider
                .GetRequiredService<JamWavDbContext>()
                .Database.EnsureCreated();
        });
    }
}