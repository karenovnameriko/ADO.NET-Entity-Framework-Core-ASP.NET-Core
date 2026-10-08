using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT2_Textarea.Pages;

public class Task3 : PageModel
{
    public string Text { get; set; } = "";

    public void OnGet()
    {
        Text = System.IO.File.ReadAllText("Task3.txt");
    }

    public void OnPost(string text)
    {
        System.IO.File.WriteAllText("Task3.txt", text);

        Text = text;
    }
}