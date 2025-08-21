using System.ComponentModel.DataAnnotations;

namespace KittyWorks.Carcassone.Models;

public class Tile
{
    [Key]
    public int Id { get; set; }
    public TileType[] Edges { get; set; } = new TileType[4]; // N,E,S,W
    public bool HasChurch { get; set; }
}
