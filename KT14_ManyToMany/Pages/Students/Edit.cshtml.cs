using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT14_ManyToMany.Pages.Students;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public StudentFormModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var student = await _db.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        Input = new StudentFormModel
        {
            Name = student.Name,
            Email = student.Email
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var student = await _db.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        student.Name = Input.Name.Trim();
        student.Email = Input.Email.Trim();

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}