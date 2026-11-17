using System.ComponentModel.DataAnnotations;

namespace KT13_OneToMany.Models;

public class BookFormModel
{
    [Required(ErrorMessage = "Введите название книги")]
    [StringLength(150, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Range(1000, 2100, ErrorMessage = "Некорректный год издания")]
    public int Year { get; set; }

    [StringLength(60)]
    public string? Genre { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Выберите автора")]
    public int AuthorId { get; set; }
}