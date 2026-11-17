using KT19_CookieAuthSignalR.Models;
using Microsoft.EntityFrameworkCore;

namespace KT19_CookieAuthSignalR.Data;

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