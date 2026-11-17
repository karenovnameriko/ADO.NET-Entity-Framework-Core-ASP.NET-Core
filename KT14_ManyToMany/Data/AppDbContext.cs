using KT14_ManyToMany.Models;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Teacher> Teachers => Set<Teacher>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Составной первичный ключ промежуточной таблицы.
        modelBuilder.Entity<Enrollment>()
            .HasKey(e => new { e.StudentId, e.CourseId });

        // Связь один ко многим: Teacher -> Course.
        modelBuilder.Entity<Teacher>()
            .HasMany(teacher => teacher.Courses)
            .WithOne(course => course.Teacher)
            .HasForeignKey(course => course.TeacherId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Связь Student -> Enrollment.
        modelBuilder.Entity<Enrollment>()
            .HasOne(enrollment => enrollment.Student)
            .WithMany(student => student.Enrollments)
            .HasForeignKey(enrollment => enrollment.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь Course -> Enrollment.
        modelBuilder.Entity<Enrollment>()
            .HasOne(enrollment => enrollment.Course)
            .WithMany(course => course.Enrollments)
            .HasForeignKey(enrollment => enrollment.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}