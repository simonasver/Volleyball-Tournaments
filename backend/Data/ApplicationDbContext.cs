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
    public DbSet<TournamentMatch> TournamentMatches { get; set; }
    
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

        builder.Entity<Set>().HasMany(s => s.Players).WithOne(s => s.Set);
        
        builder.Entity<Tournament>().HasMany(t => t.RequestedTeams);
        builder.Entity<Tournament>().HasMany(t => t.Matches).WithOne(m => m.Tournament);

        builder.Entity<TournamentMatch>().HasOne(tm => tm.FirstParent);
        builder.Entity<TournamentMatch>().HasOne(tm => tm.SecondParent);

        builder.Entity<TournamentMatch>().HasOne(tm => tm.Game).WithOne(g => g.TournamentMatch).HasForeignKey<TournamentMatch>(tm => tm.Id);
    }
}
