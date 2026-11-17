using KT12_OneToOne.Data;
using KT12_OneToOne.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT12_OneToOne.Pages.Users;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public IList<User> Users { get; set; } = new List<User>();

    public async Task OnGetAsync()
    {
        Users = await _db.Users
            .Include(user => user.Profile)
            .OrderBy(user => user.Id)
            .ToListAsync();
    }
}