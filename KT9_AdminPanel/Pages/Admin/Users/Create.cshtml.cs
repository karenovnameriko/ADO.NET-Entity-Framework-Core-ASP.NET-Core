
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT9_AdminPanel.Pages.Admin.Users;

public class CreateModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public CreateModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (Input.Role != "Admin" && Input.Role != "User")
        {
            ModelState.AddModelError("Input.Role", "Недопустимая роль.");
            return Page();
        }

        if (await _userManager.FindByEmailAsync(Input.Email) != null)
        {
            ModelState.AddModelError(
                "Input.Email", "Пользователь с таким email уже существует.");

            return Page();
        }

        var user = new IdentityUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(
            user, Input.Password);

        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user, Input.Role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            foreach (var error in roleResult.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required]
        public string Role { get; set; } = "User";
    }
}
