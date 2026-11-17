using KT12_OneToOne.Data;
using KT12_OneToOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT12_OneToOne.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _db;

    public DetailsModel(AppDbContext db)
    {
        _db = db;
    }

    public User? SelectedUser { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        SelectedUser = await _db.Users
            .Include(user => user.Profile)
            .FirstOrDefaultAsync(user => user.Id == id);

        if (SelectedUser == null)
            return NotFound();

        return Page();
    }
}