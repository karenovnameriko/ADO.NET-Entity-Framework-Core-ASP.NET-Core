using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Teachers;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public TeacherFormModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var teacher = await _db.Teachers.FindAsync(id);

        if (teacher == null)
            return NotFound();

        Input = new TeacherFormModel
        {
            Name = teacher.Name,
            Email = teacher.Email
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var teacher = await _db.Teachers.FindAsync(id);

        if (teacher == null)
            return NotFound();

        teacher.Name = Input.Name.Trim();
        teacher.Email = Input.Email.Trim();

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}