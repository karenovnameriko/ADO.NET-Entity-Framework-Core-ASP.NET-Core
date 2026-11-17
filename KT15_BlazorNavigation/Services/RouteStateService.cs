namespace KT15_BlazorNavigation.Services;

public class RouteStateService
{
    public string CurrentPath { get; private set; } = "/";

    public string TitleKey { get; private set; } = "home";

    public string Notification { get; private set; } = string.Empty;

    public void Update(string absoluteUrl, bool showNotification = true)
    {
        var uri = new Uri(absoluteUrl);
        var path = uri.AbsolutePath;

        CurrentPath = path;

        TitleKey = path switch
        {
            "/" => "home",
            var p when p.StartsWith("/details/") => "details",
            var p when p.StartsWith("/products") => "products",
            "/services" => "services",
            _ => "notFound"
        };

        Notification = showNotification ? path : string.Empty;
    }
}