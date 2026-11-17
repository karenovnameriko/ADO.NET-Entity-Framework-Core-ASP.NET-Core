using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace KT19_CookieAuthSignalR.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        var userName = Context.User?.Identity?.Name ?? "Пользователь";

        await Clients.All.SendAsync(
            "ReceiveMessage",
            userName,
            message.Trim());
    }
}