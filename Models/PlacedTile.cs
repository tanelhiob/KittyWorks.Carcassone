using System.ComponentModel.DataAnnotations;

namespace KittyWorks.Carcassone.Models;

public class PlacedTile
{
    [Key]
    public int Id { get; set; }
    public TileType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Rotation { get; set; }
}
