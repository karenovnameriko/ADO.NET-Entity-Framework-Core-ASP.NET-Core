using System.ComponentModel.DataAnnotations;

namespace KT12_OneToOne.Models;

public class UserFormModel
{
    [Required(ErrorMessage = "Введите имя")]
    [StringLength(80, MinimumLength = 2,
        ErrorMessage = "Имя должно содержать от 2 до 80 символов")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите почту")]
    [EmailAddress(ErrorMessage = "Некорректная почта")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите телефон")]
    [Phone(ErrorMessage = "Некорректный телефон")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите город")]
    public string City { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
    public string? About { get; set; }
}