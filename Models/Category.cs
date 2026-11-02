using System.ComponentModel.DataAnnotations;

namespace Lamsura.API.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Project>? Projects { get; set; }
}