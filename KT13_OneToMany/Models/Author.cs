using System.ComponentModel.DataAnnotations;

namespace KT13_OneToMany.Models;

public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите имя автора")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Country { get; set; }

    // Один автор может иметь несколько книг.
    public ICollection<Book> Books { get; set; } = new List<Book>();
}