using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT12_OneToOne.Models;

[Table("UserProfiles")]
public class UserProfile
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Введите телефон")]
    [Phone(ErrorMessage = "Некорректный телефон")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите город")]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(500)]
    public string? About { get; set; }

    public User User { get; set; } = null!;
}