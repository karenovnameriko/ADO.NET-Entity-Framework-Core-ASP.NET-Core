using KT13_OneToMany.Data;
using KT13_OneToMany.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT13_OneToMany.Pages.Books;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public IList<Book> Books { get; set; } = new List<Book>();

    public async Task OnGetAsync()
    {
        Books = await _db.Books
            .Include(book => book.Author)
            .OrderBy(book => book.Title)
            .ToListAsync();
    }
}