using System.ComponentModel.DataAnnotations;

namespace KT14_ManyToMany.Models;

public class TeacherFormModel
{
    [Required(ErrorMessage = "Введите имя преподавателя")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;
}