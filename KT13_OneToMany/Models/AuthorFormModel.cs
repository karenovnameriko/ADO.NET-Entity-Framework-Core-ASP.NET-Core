using System.ComponentModel.DataAnnotations;

namespace KT13_OneToMany.Models;

public class AuthorFormModel
{
    [Required(ErrorMessage = "Введите имя автора")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Country { get; set; }
}