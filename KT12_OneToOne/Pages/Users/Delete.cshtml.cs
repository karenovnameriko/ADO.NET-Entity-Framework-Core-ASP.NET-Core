using KT12_OneToOne.Data;
using KT12_OneToOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT12_OneToOne.Pages.Users;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _db;

    public DeleteModel(AppDbContext db)
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

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(user => user.Id == id);

        if (user == null)
            return NotFound();

        _db.Users.Remove(user);

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}