using Microsoft.EntityFrameworkCore;
using GameClubAPI.Models;

namespace GameClubAPI.Models;

public class GameClubContext : DbContext
{
    public GameClubContext(DbContextOptions<GameClubContext> options) : base(options)
    {
    }

    public DbSet<Club> Clubs { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
}