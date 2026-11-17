using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT19_CookieAuthSignalR.Pages;

[Authorize]
public class ChatModel : PageModel
{
    public void OnGet()
    {
    }
}