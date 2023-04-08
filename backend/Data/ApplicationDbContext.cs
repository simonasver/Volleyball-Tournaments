using Backend.Auth.Model;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IConfiguration _configuration;
    public DbSet<Tournament> Tournaments { get; set; }
    
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamPlayer> TeamPlayers { get; set; }
    
    public DbSet<Game> Games { get; set; }
    public DbSet<Set> Sets { get; set; }

    public ApplicationDbContext(IConfiguration configuration, DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
        _configuration = configuration;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Team>().HasMany(t => t.Players).WithOne(tp => tp.Team);

        builder.Entity<Game>().HasMany(g => g.Sets).WithOne(s => s.Game);
        builder.Entity<Game>().HasMany(g => g.RequestedTeams).WithMany(t => t.GamesRequestedTo).UsingEntity(j => j.ToTable("TeamsRequestedGames"));
        builder.Entity<Game>().HasMany(g => g.BlockedTeams).WithMany(t => t.GamesBlockedFrom).UsingEntity(j => j.ToTable("TeamsBlockedGames"));

        builder.Entity<Set>().HasMany(s => s.Players).WithOne(s => s.Set);
    }
}
