using Microsoft.AspNetCore.Identity;

namespace Backend.Data.Entities.Auth;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string FullName { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public bool Banned { get; set; }
    public ICollection<Team.Team> OwnedTeams { get; set; }
    public ICollection<Game.Game> OwnedGames { get; set; }
    public ICollection<Tournament.Tournament> OwnedTournaments { get; set; }
    public ICollection<Team.Team> ManagedTeams { get; set; }
    public ICollection<Game.Game> ManagedGames { get; set; }
    public ICollection<Tournament.Tournament> ManagedTournaments { get; set; }
}

