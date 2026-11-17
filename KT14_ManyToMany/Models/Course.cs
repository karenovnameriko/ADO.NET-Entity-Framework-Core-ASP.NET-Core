using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT14_ManyToMany.Models;

public class Course
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название курса")]
    [StringLength(150, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    // Внешний ключ преподавателя.
    [ForeignKey(nameof(Teacher))]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите преподавателя")]
    public int TeacherId { get; set; }

    // У каждого курса один преподаватель.
    public Teacher Teacher { get; set; } = null!;

    // На одном курсе может учиться несколько студентов.
    public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();
}