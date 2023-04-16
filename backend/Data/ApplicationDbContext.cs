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
    public DbSet<GameTeam> GameTeams { get; set; }
    public DbSet<GameTeamPlayer> GameTeamPlayers { get; set; }
    public DbSet<Set> Sets { get; set; }
    public DbSet<SetPlayer> SetPlayers { get; set; }

    public ApplicationDbContext(IConfiguration configuration, DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
        _configuration = configuration;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Team>().HasMany(t => t.Players).WithOne(tp => tp.Team).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Game>().HasMany(g => g.Sets).WithOne(s => s.Game).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Game>().HasOne(g => g.FirstTeam).WithOne().HasForeignKey<GameTeam>("GameWhereFirstId").OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Game>().HasOne(g => g.SecondTeam).WithOne().HasForeignKey<GameTeam>("GameWhereSecondId").OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Game>().HasOne(g => g.Winner).WithOne().HasForeignKey<GameTeam>("GameWhereWinnerId").OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GameTeam>().HasMany(gt => gt.Players).WithOne(g => g.GameTeam).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Set>().HasMany(s => s.Players).WithOne().HasForeignKey("SetId").OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<Tournament>().HasMany(t => t.RequestedTeams);
        builder.Entity<Tournament>().HasMany(t => t.AcceptedTeams).WithOne(at => at.Tournament)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Tournament>().HasMany(t => t.Matches).WithOne(m => m.Tournament)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Tournament>().HasOne(t => t.Winner).WithOne().HasForeignKey<GameTeam>("TournamentWhereWinner").OnDelete(DeleteBehavior.Restrict).IsRequired(false);
        
        builder.Entity<TournamentMatch>().HasOne(tm => tm.Game).WithOne(g => g.TournamentMatch).HasForeignKey<Game>("TournamentMatchId").OnDelete(DeleteBehavior.Cascade);
        builder.Entity<TournamentMatch>().HasMany(tm => tm.Parents).WithOne(tm => tm.Child).OnDelete(DeleteBehavior.Cascade).IsRequired(false);
        builder.Entity<TournamentMatch>().HasOne(tm => tm.Child).WithMany(tm => tm.Parents).OnDelete(DeleteBehavior.Cascade).IsRequired(false);

    }
}
