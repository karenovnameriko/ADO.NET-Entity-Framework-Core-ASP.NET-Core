using KT13_OneToMany.Data;
using KT13_OneToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KT13_OneToMany.Pages.Books;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public BookFormModel Input { get; set; } = new();

    public List<SelectListItem> AuthorOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var book = await _db.Books.FindAsync(id);

        if (book == null)
            return NotFound();

        Input = new BookFormModel
        {
            Title = book.Title,
            Year = book.Year,
            Genre = book.Genre,
            AuthorId = book.AuthorId
        };

        await LoadAuthorsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await LoadAuthorsAsync();
            return Page();
        }

        var book = await _db.Books.FindAsync(id);

        if (book == null)
            return NotFound();

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

        book.Title = Input.Title.Trim();
        book.Year = Input.Year;
        book.Genre = Input.Genre?.Trim();
        book.AuthorId = Input.AuthorId;

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