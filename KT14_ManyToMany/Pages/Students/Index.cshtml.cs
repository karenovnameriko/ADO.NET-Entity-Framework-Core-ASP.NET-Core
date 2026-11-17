using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Students;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public IList<Student> Students { get; set; } = new List<Student>();

    public async Task OnGetAsync()
    {
        Students = await _db.Students
            .Include(student => student.Enrollments)
            .ThenInclude(enrollment => enrollment.Course)
            .OrderBy(student => student.Name)
            .ToListAsync();
    }
}