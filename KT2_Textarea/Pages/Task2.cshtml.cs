using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT2_Textarea.Pages;

public class Task2 : PageModel
{
    public string Text1 { get; set; }
    
    public void OnGet()
    {
        Text1 = System.IO.File.ReadAllText("Task2.txt");
        
    }
    public void OnPost(string text)
    {
        System.IO.File.WriteAllText("Task2.txt", text);

        Text1 = text;

    }
}