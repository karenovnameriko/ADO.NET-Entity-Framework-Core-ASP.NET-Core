using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT9_AdminPanel.Pages.Admin;

public class ReportsModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public ReportsModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public int TotalUsers { get; set; }
    public int AdminCount { get; set; }
    public int UserCount { get; set; }

    public async Task OnGetAsync()
    {
        TotalUsers = await _userManager.Users.CountAsync();

        AdminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;

        UserCount = (await _userManager.GetUsersInRoleAsync("User")).Count;
    }
}