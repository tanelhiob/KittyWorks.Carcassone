using Microsoft.EntityFrameworkCore;
using KittyWorks.Carcassone.Data;
using KittyWorks.Carcassone.Models;
using System.Linq;

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
            var tile = new Tile();
            for (int e = 0; e < 4; e++)
            {
                tile.Edges[e] = (TileType)_random.Next(0, 3); // city, road, field
            }
            tile.HasChurch = _random.Next(0, 5) == 0; // occasional church
            tiles.Add(tile);
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

    public bool PlaceTile(int x, int y, int rotation, PieceType? piece, TileType? feature)
    {
        var game = GetGame();
        var next = NextTile(game);
        if (game == null || next == null)
            return false;
        if (game.PlacedTiles.Any(t => t.X == x && t.Y == y))
            return false;
        bool hasAdjacent = game.PlacedTiles.Count == 0;
        for (int dir = 0; dir < 4; dir++)
        {
            var (nx, ny) = Neighbor(x, y, dir);
            var neighbor = game.PlacedTiles.FirstOrDefault(t => t.X == nx && t.Y == ny);
            var edge = GetEdge(next, rotation, dir);
            if (neighbor != null)
            {
                hasAdjacent = true;
                var opposite = GetEdge(neighbor, (dir + 2) % 4);
                if (edge != opposite)
                    return false;
            }
        }
        if (!hasAdjacent)
            return false;

        var placed = new PlacedTile
        {
            Tile = next,
            X = x,
            Y = y,
            Rotation = rotation
        };

        if (piece != null && feature != null && ValidPiecePlacement(next, piece.Value, feature.Value))
        {
            placed.Piece = piece;
            placed.PieceFeature = feature;
            placed.PiecePlayerIndex = game.CurrentPlayerIndex;
        }

        game.PlacedTiles.Add(placed);
        game.Deck.RemoveAt(0);

        ResolveCompletions(game, placed);

        game.CurrentPlayerIndex = (game.CurrentPlayerIndex + 1) % game.Players.Count;
        _db.SaveChanges();
        return true;
    }

    private static TileType GetEdge(Tile tile, int rotation, int dir)
    {
        int index = (dir - rotation / 90 + 4) % 4;
        return tile.Edges[index];
    }

    private static TileType GetEdge(PlacedTile tile, int dir)
    {
        int index = (dir - tile.Rotation / 90 + 4) % 4;
        return tile.Tile.Edges[index];
    }

    private static (int x, int y) Neighbor(int x, int y, int dir) => dir switch
    {
        0 => (x, y + 1),
        1 => (x + 1, y),
        2 => (x, y - 1),
        3 => (x - 1, y),
        _ => (x, y)
    };

    private bool ValidPiecePlacement(Tile tile, PieceType piece, TileType feature)
    {
        if (piece == PieceType.Farmer && feature == TileType.Field)
            return false;
        if (piece == PieceType.Bishop && feature == TileType.Road)
            return false;
        if (feature == TileType.Church)
            return tile.HasChurch;
        return tile.Edges.Contains(feature);
    }

    private void ResolveCompletions(Game game, PlacedTile placed)
    {
        var visited = new HashSet<(int, int, int)>();
        for (int dir = 0; dir < 4; dir++)
        {
            var feature = GetEdge(placed, dir);
            if (feature != TileType.Road && feature != TileType.City)
                continue;
            if (visited.Contains((placed.X, placed.Y, dir)))
                continue;
            var region = new HashSet<PlacedTile>();
            if (ExploreFeature(game, placed, dir, feature, visited, region))
            {
                int points = feature == TileType.Road ? region.Count : region.Count * 2;
                foreach (var t in region)
                {
                    if (t.PieceFeature == feature && t.PiecePlayerIndex.HasValue)
                    {
                        game.Players[t.PiecePlayerIndex.Value].Score += points;
                        t.Piece = null;
                        t.PieceFeature = null;
                        t.PiecePlayerIndex = null;
                    }
                }
            }
        }
    }

    private bool ExploreFeature(Game game, PlacedTile start, int startDir, TileType feature,
        HashSet<(int, int, int)> visited, HashSet<PlacedTile> region)
    {
        var queue = new Queue<(PlacedTile tile, int dir)>();
        queue.Enqueue((start, startDir));
        bool closed = true;
        while (queue.Count > 0)
        {
            var (tile, dir) = queue.Dequeue();
            if (!visited.Add((tile.X, tile.Y, dir)))
                continue;
            region.Add(tile);
            var (nx, ny) = Neighbor(tile.X, tile.Y, dir);
            var neighbor = game.PlacedTiles.FirstOrDefault(t => t.X == nx && t.Y == ny);
            if (neighbor == null)
            {
                closed = false;
                continue;
            }
            int opposite = (dir + 2) % 4;
            if (GetEdge(neighbor, opposite) != feature)
            {
                closed = false;
                continue;
            }
            for (int i = 0; i < 4; i++)
            {
                if (i == opposite) continue;
                if (GetEdge(neighbor, i) == feature)
                    queue.Enqueue((neighbor, i));
            }
        }
        return closed;
    }
}
