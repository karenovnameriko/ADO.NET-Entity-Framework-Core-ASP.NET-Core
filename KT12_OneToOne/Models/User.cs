using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT12_OneToOne.Models;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите имя")]
    [StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите электронную почту")]
    [EmailAddress(ErrorMessage = "Некорректная почта")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [InverseProperty(nameof(UserProfile.User))]
    public UserProfile Profile { get; set; } = null!;
}