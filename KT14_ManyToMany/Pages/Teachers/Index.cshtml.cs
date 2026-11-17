using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Teachers;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public IList<Teacher> Teachers { get; set; } = new List<Teacher>();

    public async Task OnGetAsync()
    {
        Teachers = await _db.Teachers
            .Include(teacher => teacher.Courses)
            .OrderBy(teacher => teacher.Name)
            .ToListAsync();
    }
}