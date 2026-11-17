using KT13_OneToMany.Data;
using KT13_OneToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT13_OneToMany.Pages.Authors;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public AuthorFormModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var author = await _db.Authors.FindAsync(id);

        if (author == null)
            return NotFound();

        Input = new AuthorFormModel
        {
            Name = author.Name,
            Country = author.Country
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var author = await _db.Authors.FindAsync(id);

        if (author == null)
            return NotFound();

        author.Name = Input.Name.Trim();
        author.Country = Input.Country?.Trim();

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}