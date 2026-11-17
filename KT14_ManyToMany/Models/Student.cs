using System.ComponentModel.DataAnnotations;

namespace KT14_ManyToMany.Models;

public class Student
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите имя студента")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    // Один студент может быть записан на несколько курсов.
    public ICollection<Enrollment> Enrollments { get; set; }
        = new List<Enrollment>();
}