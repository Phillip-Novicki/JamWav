using JamWav.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JamWav.Infrastructure.Persistence;

public class JamWavDbContext : DbContext
{
    public JamWavDbContext(DbContextOptions<JamWavDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).IsRequired();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.DisplayName).IsRequired();
            entity.Property(u => u.CreatedAt).IsRequired();
        });
    }
}