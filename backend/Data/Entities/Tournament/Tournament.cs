using System.ComponentModel.DataAnnotations;
using Backend.Data.Entities.Auth;
using Backend.Data.Entities.Game;
using Backend.Interfaces.Services;

namespace Backend.Data.Entities.Tournament;

public enum TournamentStatus
{
    Open,
    Closed,
    Started,
    Finished
}

public class Tournament : IUserManagedResource
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? PictureUrl { get; set; }
    public string? Description { get; set; }
    public bool SingleThirdPlace { get; set; }
    [Range(minimum: 2, maximum: 128, ErrorMessage = "Team limit must be between 2 and 128")]
    public int MaxTeams { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastEditDate { get; set; }
    public TournamentStatus Status { get; set; }
    public ICollection<Team.Team> RequestedTeams { get; set; }
    public IList<GameTeam> AcceptedTeams { get; set; }
    public GameTeam? Winner { get; set; }
    public int FinalRound { get; set; }
    // TOURNAMENT GAME SETTINGS
    public bool Basic { get; set; }
    public int PointsToWin { get; set; }
    public int PointsToWinLastSet { get; set; }
    public int PointDifferenceToWin { get; set; }
    public int MaxSets { get; set; }
    public int PlayersPerTeam { get; set; }
    
    // TOURNAMENT MATCHUPS
    public ICollection<TournamentMatch> Matches { get; set; }
    
    public string OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }
    public ICollection<ApplicationUser> Managers { get; set; }
}