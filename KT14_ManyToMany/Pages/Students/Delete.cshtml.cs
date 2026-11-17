using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Students;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public Student? SelectedStudent { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        SelectedStudent = await _db.Students
            .Include(student => student.Enrollments)
            .ThenInclude(enrollment => enrollment.Course)
            .FirstOrDefaultAsync(student => student.Id == id);

        if (SelectedStudent == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var student = await _db.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        _db.Students.Remove(student);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}