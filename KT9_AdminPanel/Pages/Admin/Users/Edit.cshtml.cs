
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT9_AdminPanel.Pages.Admin.Users;

public class EditModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public EditModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        Input = new InputModel
        {
            Id = user.Id,
            Email = user.Email ?? "",
            Role = roles.FirstOrDefault() ?? "User"
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByIdAsync(Input.Id);

        if (user == null)
            return NotFound();

        if (Input.Role != "Admin" && Input.Role != "User")
        {
            ModelState.AddModelError("Input.Role", "Недопустимая роль.");
            return Page();
        }

        // Нельзя снять роль администратора с самого себя.
        var currentUserId = _userManager.GetUserId(User);

        if (user.Id == currentUserId && Input.Role != "Admin")
        {
            ModelState.AddModelError(
                "", "Нельзя снять роль администратора с собственной учётной записи.");

            return Page();
        }

        var existingUser = await _userManager.FindByEmailAsync(Input.Email);

        if (existingUser != null && existingUser.Id != user.Id)
        {
            ModelState.AddModelError(
                "Input.Email", "Этот email уже используется.");

            return Page();
        }

        var emailResult = await _userManager.SetEmailAsync(user, Input.Email);

        if (!emailResult.Succeeded)
        {
            foreach (var error in emailResult.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        var nameResult = await _userManager.SetUserNameAsync(user, Input.Email);

        if (!nameResult.Succeeded)
        {
            foreach (var error in nameResult.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        var oldRoles = await _userManager.GetRolesAsync(user);

        var removeResult = await _userManager.RemoveFromRolesAsync(
            user, oldRoles);

        if (!removeResult.Succeeded)
        {
            foreach (var error in removeResult.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        var addResult = await _userManager.AddToRoleAsync(user, Input.Role);

        if (!addResult.Succeeded)
        {
            // Пытаемся восстановить прежние роли, если назначение не удалось.
            if (oldRoles.Count > 0)
                await _userManager.AddToRolesAsync(user, oldRoles);

            foreach (var error in addResult.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        [Required]
        public string Id { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Role { get; set; } = "User";
    }
}
