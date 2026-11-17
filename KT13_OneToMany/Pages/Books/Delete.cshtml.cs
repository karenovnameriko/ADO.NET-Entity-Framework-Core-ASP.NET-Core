using KT13_OneToMany.Data;
using KT13_OneToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT13_OneToMany.Pages.Books;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
    {
        _db = db;
    }

    public Book? SelectedBook { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        SelectedBook = await _db.Books
            .Include(book => book.Author)
            .FirstOrDefaultAsync(book => book.Id == id);

        if (SelectedBook == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var book = await _db.Books.FindAsync(id);

        if (book == null)
            return NotFound();

        _db.Books.Remove(book);

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}