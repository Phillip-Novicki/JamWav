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
    public DbSet<Event> Events { get; set; }
    public DbSet<Friend> Friends { get; set; }
   
    

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

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);      // optional length constraint

            entity.Property(e => e.Date)
                .IsRequired();

            entity.Property(e => e.Venue)
                .IsRequired()
                .HasMaxLength(200);      // optional

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });
        
        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.UserId).IsRequired();
            entity.Property(f => f.FriendUserId).IsRequired();
            entity.Property(f => f.CreatedAt).IsRequired();
        });
    }
}