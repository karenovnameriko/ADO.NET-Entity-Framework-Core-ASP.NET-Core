using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using KT19_CookieAuthSignalR.Data;
using KT19_CookieAuthSignalR.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT19_CookieAuthSignalR.Pages.Account;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LoginModel(
        AppDbContext db,
        IPasswordHasher<User> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public class LoginInput
    {
        [Required(ErrorMessage = "Введите логин")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var userName = Input.UserName.Trim();

        var user = await _db.Users
            .FirstOrDefaultAsync(user => user.UserName == userName);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return Page();
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            Input.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return Page();
        }

        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, Input.Password);
            await _db.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        return RedirectToPage("/Chat");
    }
}