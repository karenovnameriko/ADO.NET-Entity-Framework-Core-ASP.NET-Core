using KT12_OneToOne.Data;
using KT12_OneToOne.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT12_OneToOne.Pages.Users;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public UserFormModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var email = Input.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(user => user.Email == email))
        {
            ModelState.AddModelError(
                "Input.Email",
                "Пользователь с такой почтой уже существует");

            return Page();
        }

        var user = new User
        {
            Name = Input.Name.Trim(),
            Email = email,

            Profile = new UserProfile
            {
                Phone = Input.Phone.Trim(),
                City = Input.City.Trim(),
                About = Input.About?.Trim()
            }
        };

        _db.Users.Add(user);

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}