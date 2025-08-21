using System.ComponentModel.DataAnnotations;

namespace KittyWorks.Carcassone.Models;

public class Game
{
    [Key]
    public int Id { get; set; }
    public List<Player> Players { get; set; } = new();
    public int CurrentPlayerIndex { get; set; }
    public List<Tile> Deck { get; set; } = new();
    public List<PlacedTile> PlacedTiles { get; set; } = new();
}
