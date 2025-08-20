using Microsoft.EntityFrameworkCore;
using KittyWorks.Carcassone.Data;
using KittyWorks.Carcassone.Models;

namespace KittyWorks.Carcassone.Services;

public class GameService
{
    private readonly GameDbContext _db;
    private static readonly Random _random = new();

    public GameService(GameDbContext db)
    {
        _db = db;
    }

    public Game CreateGame(string player1, string player2)
    {
        _db.Games.RemoveRange(_db.Games);
        var game = new Game
        {
            Players = new List<Player>
            {
                new() { Name = player1 },
                new() { Name = player2 }
            },
            Deck = CreateDeck(),
            CurrentPlayerIndex = 0
        };
        _db.Games.Add(game);
        _db.SaveChanges();
        return game;
    }

    private List<Tile> CreateDeck()
    {
        var tiles = new List<Tile>();
        for (int i = 0; i < 20; i++)
        {
            tiles.Add(new Tile { Type = (TileType)_random.Next(0, 3) });
        }
        return tiles;
    }

    public Game? GetGame()
    {
        return _db.Games
            .Include(g => g.Players)
            .Include(g => g.Deck)
            .Include(g => g.PlacedTiles)
            .FirstOrDefault();
    }

    public Tile? NextTile(Game? game) => game?.Deck.FirstOrDefault();

    public bool PlaceTile(int x, int y, int rotation)
    {
        var game = GetGame();
        var next = NextTile(game);
        if (game == null || next == null)
            return false;
        if (game.PlacedTiles.Any(t => t.X == x && t.Y == y))
            return false;
        if (game.PlacedTiles.Count > 0 &&
            !game.PlacedTiles.Any(t => Math.Abs(t.X - x) + Math.Abs(t.Y - y) == 1))
            return false;

        game.PlacedTiles.Add(new PlacedTile
        {
            Type = next.Type,
            X = x,
            Y = y,
            Rotation = rotation
        });
        game.Deck.RemoveAt(0);
        game.CurrentPlayerIndex = (game.CurrentPlayerIndex + 1) % game.Players.Count;
        _db.SaveChanges();
        return true;
    }
}
