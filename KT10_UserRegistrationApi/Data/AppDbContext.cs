using KT10_UserRegistrationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KT10_UserRegistrationApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
}