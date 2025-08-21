using Microsoft.EntityFrameworkCore;
using KittyWorks.Carcassone.Models;

namespace KittyWorks.Carcassone.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
}
