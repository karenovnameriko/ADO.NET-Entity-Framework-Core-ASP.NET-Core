using Microsoft.AspNetCore.Mvc;

namespace KT4_HttpRequest.Controllers;

public class EchoController : Controller
{
    [HttpGet]
    public async Task Get()
    {
        Response.ContentType = "text/plain";

        await Response.WriteAsync("GET request received");
    }

    [HttpPost]
    public async Task Post()
    {
        Response.ContentType = "text/plain";

        await Response.WriteAsync("POST request received");
    }

    public async Task Headers()
    {
        Response.ContentType = "application/json";

        var headers = Request.Headers.ToDictionary(
            x => x.Key,
            x => x.Value.ToString()
        );

        await Response.WriteAsJsonAsync(headers);
    }

    public async Task Query()
    {
        Response.ContentType = "application/json";

        var query = Request.Query.ToDictionary(
            x => x.Key,
            x => x.Value.ToString()
        );

        await Response.WriteAsJsonAsync(query);
    }

    public async Task Body()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        Response.ContentType = "text/plain";
        await Response.WriteAsync(body);
    }
}