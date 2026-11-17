using KT20_TokenAuthSignalR.Models;
using Microsoft.EntityFrameworkCore;

namespace KT20_TokenAuthSignalR.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.UserName)
            .IsUnique();
    }
}