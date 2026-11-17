using KT13_OneToMany.Data;
using KT13_OneToMany.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT13_OneToMany.Pages.Authors;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public IList<Author> Authors { get; set; } = new List<Author>();

    public async Task OnGetAsync()
    {
        Authors = await _db.Authors
            .Include(author => author.Books)
            .OrderBy(author => author.Name)
            .ToListAsync();
    }
}