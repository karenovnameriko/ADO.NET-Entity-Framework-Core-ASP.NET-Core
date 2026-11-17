using KT10_UserRegistrationApi.Data;
using KT10_UserRegistrationApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT10_UserRegistrationApi.Pages;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<AppUser> _passwordHasher;

    public RegisterModel(
        AppDbContext db,
        IPasswordHasher<AppUser> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; } = new();

    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var email = Input.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError("Input.Email",
                "Пользователь с такой почтой уже существует");

            return Page();
        }

        var user = new AppUser
        {
            Username = Input.Username.Trim(),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, Input.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        ModelState.Clear();
        Input = new RegisterInputModel();
        SuccessMessage = "Регистрация успешно завершена!";

        return Page();
    }
}