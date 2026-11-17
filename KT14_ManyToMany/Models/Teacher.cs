using System.ComponentModel.DataAnnotations;

namespace KT14_ManyToMany.Models;

public class Teacher
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите имя преподавателя")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    // Один преподаватель может вести несколько курсов.
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}