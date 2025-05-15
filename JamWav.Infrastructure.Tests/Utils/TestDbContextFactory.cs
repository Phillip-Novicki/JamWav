// JamWav.Infrastructure.Tests/Utils/TestDbContextFactory.cs
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using JamWav.Infrastructure.Persistence;

namespace JamWav.Infrastructure.Tests.Utils
{
    // public static class TestDbContextFactory
    // {
    //     public static JamWavDbContext CreateContext()
    //     {
    //         // build a clean, shared in‑memory SQLite connection
    //         var builder = new SqliteConnectionStringBuilder
    //         {
    //             DataSource = ":memory:",
    //             Mode       = SqliteOpenMode.Memory,
    //             Cache      = SqliteCacheMode.Shared
    //         };
    //         var connection = new SqliteConnection(builder.ToString());
    //         connection.Open();
    //
    //         // in TestDbContextFactory:
    //         var options = new DbContextOptionsBuilder<JamWavDbContext>()
    //             .UseInMemoryDatabase(Guid.NewGuid().ToString())
    //             .Options;
    //         var context = new JamWavDbContext(options);
    //         // ensure each test starts with a blank schema
    //         context.Database.EnsureDeleted();
    //         context.Database.EnsureCreated();
    //
    //         return context;
    //     }
    // }
    
    public static class TestDbContextFactory
    {
        public static JamWavDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<JamWavDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var ctx = new JamWavDbContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        }
    }
}