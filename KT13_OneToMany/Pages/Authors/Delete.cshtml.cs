using KT13_OneToMany.Data;
using KT13_OneToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT13_OneToMany.Pages.Authors;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public Author? SelectedAuthor { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        SelectedAuthor = await _db.Authors
            .Include(author => author.Books)
            .FirstOrDefaultAsync(author => author.Id == id);

        if (SelectedAuthor == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var author = await _db.Authors.FindAsync(id);

        if (author == null)
            return NotFound();

        _db.Authors.Remove(author);

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}