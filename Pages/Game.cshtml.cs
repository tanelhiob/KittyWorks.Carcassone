using KittyWorks.Carcassone.Models;
using KittyWorks.Carcassone.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KittyWorks.Carcassone.Pages;

public class GameModel : PageModel
{
    private readonly GameService _gameService;

    public GameModel(GameService gameService)
    {
        _gameService = gameService;
    }

    public Game? CurrentGame { get; set; }
    public Tile? NextTile { get; set; }

    [BindProperty]
    public int X { get; set; }
    [BindProperty]
    public int Y { get; set; }
    [BindProperty]
    public int Rotation { get; set; }

    public void OnGet()
    {
        CurrentGame = _gameService.GetGame();
        NextTile = _gameService.NextTile(CurrentGame);
    }

    public IActionResult OnPost()
    {
        _gameService.PlaceTile(X, Y, Rotation);
        CurrentGame = _gameService.GetGame();
        NextTile = _gameService.NextTile(CurrentGame);
        return Page();
    }
}
