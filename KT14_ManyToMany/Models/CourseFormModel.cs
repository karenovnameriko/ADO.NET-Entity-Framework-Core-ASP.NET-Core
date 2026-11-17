using System.ComponentModel.DataAnnotations;

namespace KT14_ManyToMany.Models;

public class CourseFormModel
{
    [Required(ErrorMessage = "Введите название курса")]
    [StringLength(150, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Выберите преподавателя")]
    public int TeacherId { get; set; }

    // Выбранные студенты для записи на курс.
    public List<int> SelectedStudentIds { get; set; } = new();
}