using System.ComponentModel.DataAnnotations;
using KT19_CookieAuthSignalR.Data;
using KT19_CookieAuthSignalR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT19_CookieAuthSignalR.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterModel(
        AppDbContext db,
        IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public class RegisterInput
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(30, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Повторите пароль")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var userName = Input.UserName.Trim();

        if (await _db.Users.AnyAsync(user => user.UserName == userName))
        {
            ModelState.AddModelError("Input.UserName", "Такой логин уже занят");
            return Page();
        }

        var user = new User
        {
            UserName = userName
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, Input.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Account/Login");
    }
}