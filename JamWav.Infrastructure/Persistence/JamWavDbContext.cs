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
    public DbSet<Band> Bands { get; set; }

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

        modelBuilder.Entity<Band>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.HasIndex(b => b.Name).IsUnique();
            entity.Property(b => b.Name).IsRequired();
            entity.Property(b => b.Genre).IsRequired();
            entity.Property(b => b.Origin).IsRequired();
            entity.Property(b => b.CreatedAt).IsRequired();
        });
    }
}