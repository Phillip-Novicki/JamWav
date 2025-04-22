using JamWav.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace JamWav.Infrastructure.Tests.Utils;

public static class TestDbContextFactory
{
    public static JamWavDbContext CreateContext()
    {
        // Create a new in-memory SQLite connection
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        
        var options = new DbContextOptionsBuilder<JamWavDbContext>()
            .UseSqlite(connection)
            .Options;
        
        var context = new JamWavDbContext(options);
        
        // Ensure database is created and migrations are applied (if needed)
        context.Database.EnsureCreated();
        
        return context;
    }
}