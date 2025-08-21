using System.ComponentModel.DataAnnotations;

namespace KittyWorks.Carcassone.Models;

public class Tile
{
    [Key]
    public int Id { get; set; }
    public TileType Type { get; set; }
}
