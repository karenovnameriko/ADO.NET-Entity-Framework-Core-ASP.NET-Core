using Microsoft.AspNetCore.Mvc;

namespace KT3_HttpResponse.Controllers;

public class TestController : Controller
{
    public async Task Text()
    {
        Response.ContentType = "text/plain";

        await Response.WriteAsync("Hello, world!");
    }

    public async Task Html()
    {
        Response.ContentType = "text/html";

        await Response.WriteAsync("""
                                  <h1>Hello, world!</h1>
                                  <p>This is an HTML response.</p>
                                  """);
    }

    public async Task Json()
    {
        Response.ContentType = "application/json";

        var person = new
        {
            Name = "Mery",
            Age = 18
        };

        await Response.WriteAsJsonAsync(person);
    }

    public async Task File()
    {
        Response.ContentType = "text/plain";

        var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");

        System.IO.File.WriteAllText(path, "This is a test file");

        await Response.SendFileAsync(path);
    }

    public async Task Status()
    {
        Response.StatusCode = 404;

        Response.ContentType = "text/plain";

        await Response.WriteAsync("Resource not found");
    }

    public async Task Cookie()
    {
        Response.Cookies.Append("user", "Answer");

        Response.ContentType = "text/plain";

        await Response.WriteAsync("Cookie 'user' has been set.");
    }
}