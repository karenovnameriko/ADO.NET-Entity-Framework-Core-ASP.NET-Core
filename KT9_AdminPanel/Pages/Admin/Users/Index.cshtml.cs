using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT9_AdminPanel.Pages.Admin.Users;

public class IndexModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public IndexModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public List<UserItem> Users { get; set; } = new();

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            Users.Add(new UserItem
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Roles = string.Join(", ", roles)
            });
        }
    }

    public class UserItem
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string Roles { get; set; } = "";
    }
}