using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT14_ManyToMany.Pages.Students;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public StudentFormModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var student = new Student
        {
            Name = Input.Name.Trim(),
            Email = Input.Email.Trim()
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}