using KT12_OneToOne.Data;
using KT12_OneToOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT12_OneToOne.Pages.Users;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public UserFormModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _db.Users
            .Include(user => user.Profile)
            .FirstOrDefaultAsync(user => user.Id == id);

        if (user == null)
            return NotFound();

        Input = new UserFormModel
        {
            Name = user.Name,
            Email = user.Email,
            Phone = user.Profile.Phone,
            City = user.Profile.City,
            About = user.Profile.About
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _db.Users
            .Include(user => user.Profile)
            .FirstOrDefaultAsync(user => user.Id == id);

        if (user == null)
            return NotFound();

        var email = Input.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email && u.Id != id))
        {
            ModelState.AddModelError(
                "Input.Email",
                "Эта почта уже используется");

            return Page();
        }

        user.Name = Input.Name.Trim();
        user.Email = email;

        user.Profile.Phone = Input.Phone.Trim();
        user.Profile.City = Input.City.Trim();
        user.Profile.About = Input.About?.Trim();

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}