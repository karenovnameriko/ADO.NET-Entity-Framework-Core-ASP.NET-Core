using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT2_Textarea.Pages;

public class IndexModel : PageModel
{
    public string Text { get; set; }
    
    public void OnGet()
    {
        Text = System.IO.File.ReadAllText("Task1.txt");
        
    }
    public void OnPost(string text)
    {
        System.IO.File.WriteAllText("Task1.txt", text);

        Text = text;

    }
}
