using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Courses;

public class CreateModel : PageModel
{
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public CourseFormModel Input { get; set; } = new();

    public List<SelectListItem> TeacherOptions { get; set; } = new();

    public List<Student> AvailableStudents { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var teacherExists = await _db.Teachers
            .AnyAsync(teacher => teacher.Id == Input.TeacherId);

        if (!teacherExists)
        {
            ModelState.AddModelError(
                "Input.TeacherId",
                "Выбранный преподаватель не существует");

            await LoadOptionsAsync();
            return Page();
        }

        var studentIds = Input.SelectedStudentIds.Distinct().ToList();

        var existingStudentsCount = await _db.Students
            .CountAsync(student => studentIds.Contains(student.Id));

        if (studentIds.Count != existingStudentsCount)
        {
            ModelState.AddModelError(
                "Input.SelectedStudentIds",
                "Один или несколько выбранных студентов не существуют");

            await LoadOptionsAsync();
            return Page();
        }

        var course = new Course
        {
            Title = Input.Title.Trim(),
            Description = Input.Description?.Trim(),
            TeacherId = Input.TeacherId
        };

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();

        // Создаём записи о зачислении студентов на курс.
        var enrollments = studentIds.Select(studentId => new Enrollment
        {
            CourseId = course.Id,
            StudentId = studentId
        });

        _db.Enrollments.AddRange(enrollments);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadOptionsAsync()
    {
        TeacherOptions = await _db.Teachers
            .OrderBy(teacher => teacher.Name)
            .Select(teacher => new SelectListItem
            {
                Value = teacher.Id.ToString(),
                Text = teacher.Name
            })
            .ToListAsync();

        AvailableStudents = await _db.Students
            .OrderBy(student => student.Name)
            .ToListAsync();
    }
}