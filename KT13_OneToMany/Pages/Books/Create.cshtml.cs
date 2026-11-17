using KT13_OneToMany.Data;
using KT13_OneToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KT13_OneToMany.Pages.Books;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public BookFormModel Input { get; set; } = new();

    public List<SelectListItem> AuthorOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadAuthorsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadAuthorsAsync();
            return Page();
        }

        var authorExists = await _db.Authors
            .AnyAsync(author => author.Id == Input.AuthorId);

        if (!authorExists)
        {
            ModelState.AddModelError(
                "Input.AuthorId",
                "Выбранный автор не существует");

            await LoadAuthorsAsync();
            return Page();
        }

        var book = new Book
        {
            Title = Input.Title.Trim(),
            Year = Input.Year,
            Genre = Input.Genre?.Trim(),
            AuthorId = Input.AuthorId
        };

        _db.Books.Add(book);

        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadAuthorsAsync()
    {
        AuthorOptions = await _db.Authors
            .OrderBy(author => author.Name)
            .Select(author => new SelectListItem
            {
                Value = author.Id.ToString(),
                Text = author.Name
            })
            .ToListAsync();
    }
}