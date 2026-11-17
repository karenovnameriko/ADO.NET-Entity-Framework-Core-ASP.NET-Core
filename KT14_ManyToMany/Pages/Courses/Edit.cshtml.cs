using KT14_ManyToMany.Data;
using KT14_ManyToMany.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KT14_ManyToMany.Pages.Courses;

public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public CourseFormModel Input { get; set; } = new();

    public List<SelectListItem> TeacherOptions { get; set; } = new();

    public List<Student> AvailableStudents { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var course = await _db.Courses
            .Include(course => course.Enrollments)
            .FirstOrDefaultAsync(course => course.Id == id);

        if (course == null)
            return NotFound();

        Input = new CourseFormModel
        {
            Title = course.Title,
            Description = course.Description,
            TeacherId = course.TeacherId,
            SelectedStudentIds = course.Enrollments
                .Select(enrollment => enrollment.StudentId)
                .ToList()
        };

        await LoadOptionsAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            return Page();
        }

        var course = await _db.Courses
            .Include(course => course.Enrollments)
            .FirstOrDefaultAsync(course => course.Id == id);

        if (course == null)
            return NotFound();

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

        course.Title = Input.Title.Trim();
        course.Description = Input.Description?.Trim();
        course.TeacherId = Input.TeacherId;

        // Определяем текущих и новых участников курса.
        var currentStudentIds = course.Enrollments
            .Select(enrollment => enrollment.StudentId)
            .ToHashSet();

        var selectedStudentIds = studentIds.ToHashSet();

        // Убираем студентов, которых сняли с курса.
        var enrollmentsToRemove = course.Enrollments
            .Where(enrollment =>
                !selectedStudentIds.Contains(enrollment.StudentId))
            .ToList();

        _db.Enrollments.RemoveRange(enrollmentsToRemove);

        // Добавляем новых студентов.
        var enrollmentsToAdd = selectedStudentIds
            .Except(currentStudentIds)
            .Select(studentId => new Enrollment
            {
                CourseId = course.Id,
                StudentId = studentId
            });

        _db.Enrollments.AddRange(enrollmentsToAdd);

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