using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JamWav.Infrastructure.Persistence
{
    public class DesignTimeJamWavDbContextFactory : IDesignTimeDbContextFactory<JamWavDbContext>
    {
        public JamWavDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<JamWavDbContext>();

            // Look for environment variable first
            var connectionString = Environment.GetEnvironmentVariable("JAMWAV_MIGRATION_CONNECTION");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                // Fallback if not found (safe localhost dev only)
                connectionString = "Server=localhost,1434;Database=JamWavDb;User=sa;Password=YourStrong(!)Password;TrustServerCertificate=True;";
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new JamWavDbContext(optionsBuilder.Options);
        }
    }
}