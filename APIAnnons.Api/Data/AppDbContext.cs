using APIAnnons.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace APIAnnons.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ad> Ads => Set<Ad>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ad>(entity =>
        {
            entity.Property(a => a.Title).HasMaxLength(100);
            entity.Property(a => a.Description).HasMaxLength(2000);
            entity.Property(a => a.Category).HasMaxLength(50);
            entity.Property(a => a.Location).HasMaxLength(100);
            entity.Property(a => a.Price).HasPrecision(18, 2);
        });
    }
}