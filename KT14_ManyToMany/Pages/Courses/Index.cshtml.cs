using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Courses;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public IList<Course> Courses { get; set; } = new List<Course>();

    public async Task OnGetAsync()
    {
        Courses = await _db.Courses
            .Include(course => course.Teacher)
            .Include(course => course.Enrollments)
            .ThenInclude(enrollment => enrollment.Student)
            .OrderBy(course => course.Title)
            .ToListAsync();
    }
}