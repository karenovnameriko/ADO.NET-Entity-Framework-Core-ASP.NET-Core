using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT14_ManyToMany.Pages.Teachers;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public TeacherFormModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var teacher = new Teacher
        {
            Name = Input.Name.Trim(),
            Email = Input.Email.Trim()
        };

        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}