using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT9_AdminPanel.Pages.Admin.Users;

public class DeleteModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public DeleteModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public string Id { get; set; } = "";

    public string Email { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        Id = user.Id;
        Email = user.Email ?? user.UserName ?? "";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.FindByIdAsync(Id);

        if (user == null)
            return NotFound();

        Email = user.Email ?? user.UserName ?? "";

        if (Id == _userManager.GetUserId(User))
        {
            ModelState.AddModelError(
                "", "Нельзя удалить собственную учётную запись.");

            return Page();
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }

        return RedirectToPage("./Index");
    }
}