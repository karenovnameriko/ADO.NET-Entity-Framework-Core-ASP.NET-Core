using KT13_OneToMany.Data;
using KT13_OneToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT13_OneToMany.Pages.Authors;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public AuthorFormModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var author = new Author
        {
            Name = Input.Name.Trim(),
            Country = Input.Country?.Trim()
        };

        _db.Authors.Add(author);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}