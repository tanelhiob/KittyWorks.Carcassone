using System.ComponentModel.DataAnnotations;

namespace KittyWorks.Carcassone.Models;

public class PlacedTile
{
    [Key]
    public int Id { get; set; }
    public Tile Tile { get; set; } = new();
    public int X { get; set; }
    public int Y { get; set; }
    public int Rotation { get; set; }
    public PieceType? Piece { get; set; }
    public TileType? PieceFeature { get; set; }
    public int? PiecePlayerIndex { get; set; }
}
