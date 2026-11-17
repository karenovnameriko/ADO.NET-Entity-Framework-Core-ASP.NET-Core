using System.ComponentModel.DataAnnotations;

namespace KT13_OneToMany.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название книги")]
    [StringLength(150, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Range(1000, 2100, ErrorMessage = "Некорректный год издания")]
    public int Year { get; set; }

    [StringLength(60)]
    public string? Genre { get; set; }

    // Внешний ключ автора.
    [Range(1, int.MaxValue, ErrorMessage = "Выберите автора")]
    public int AuthorId { get; set; }

    // Навигационное свойство.
    public Author Author { get; set; } = null!;
}