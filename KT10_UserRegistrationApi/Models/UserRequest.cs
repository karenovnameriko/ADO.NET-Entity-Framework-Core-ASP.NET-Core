using System.ComponentModel.DataAnnotations;

namespace KT10_UserRegistrationApi.Models;

public class UserRequest
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
}