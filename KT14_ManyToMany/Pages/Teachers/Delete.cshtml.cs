using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Teachers;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public Teacher? SelectedTeacher { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        SelectedTeacher = await _db.Teachers
            .Include(teacher => teacher.Courses)
            .FirstOrDefaultAsync(teacher => teacher.Id == id);

        if (SelectedTeacher == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var teacher = await _db.Teachers.FindAsync(id);

        if (teacher == null)
            return NotFound();

        _db.Teachers.Remove(teacher);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}