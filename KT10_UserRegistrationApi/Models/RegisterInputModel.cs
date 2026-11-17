using System.ComponentModel.DataAnnotations;

namespace KT10_UserRegistrationApi.Models;

public class RegisterInputModel
{
    [Required(ErrorMessage = "Введите имя пользователя")]
    [StringLength(50, MinimumLength = 3,
        ErrorMessage = "Имя должно содержать от 3 до 50 символов")]
    [RegularExpression(@"^\p{L}+$",
        ErrorMessage = "Имя должно содержать только буквы")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите электронную почту")]
    [EmailAddress(ErrorMessage = "Введите корректный адрес электронной почты")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [RegularExpression(@"^(?=.*\p{L})(?=.*\d).{8,}$",
        ErrorMessage = "Пароль должен содержать минимум 8 символов, буквы и цифры")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;
}