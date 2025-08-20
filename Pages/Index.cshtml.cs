using KittyWorks.Carcassone.Services;
using KittyWorks.Carcassone.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KittyWorks.Carcassone.Pages;

public class IndexModel : PageModel
{
    private readonly GameService _gameService;
    private readonly GameDbContext _db;

    public IndexModel(GameService gameService, GameDbContext db)
    {
        _gameService = gameService;
        _db = db;
    }

    [BindProperty]
    public string Player1 { get; set; } = string.Empty;
    [BindProperty]
    public string Player2 { get; set; } = string.Empty;

    public bool HasGame { get; set; }

    public void OnGet()
    {
        HasGame = _db.Games.Any();
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Player1) || string.IsNullOrWhiteSpace(Player2))
        {
            ModelState.AddModelError(string.Empty, "Please enter player names.");
            HasGame = _db.Games.Any();
            return Page();
        }
        _gameService.CreateGame(Player1, Player2);
        return RedirectToPage("/Game");
    }
}
