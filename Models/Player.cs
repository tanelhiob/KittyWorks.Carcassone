using System.ComponentModel.DataAnnotations;

namespace KittyWorks.Carcassone.Models;

public class Player
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
}
