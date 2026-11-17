using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Courses;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public Course? SelectedCourse { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        SelectedCourse = await _db.Courses
            .Include(course => course.Teacher)
            .Include(course => course.Enrollments)
            .ThenInclude(enrollment => enrollment.Student)
            .FirstOrDefaultAsync(course => course.Id == id);

        if (SelectedCourse == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var course = await _db.Courses.FindAsync(id);

        if (course == null)
            return NotFound();

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}