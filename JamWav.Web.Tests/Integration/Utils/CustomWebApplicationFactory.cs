using System;
using System.Linq;
using JamWav.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JamWav.Web.Tests.Utils;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<JamWavDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<JamWavDbContext>(options => options.UseSqlite(connection));

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<JamWavDbContext>();
            db.Database.EnsureCreated();
        });
    }
}