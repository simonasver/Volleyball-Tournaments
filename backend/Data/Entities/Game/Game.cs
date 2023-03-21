using Backend.Auth.Model;

namespace Backend.Data.Entities.Game;

public class Game
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int PointsToWin { get; set; }
    public int PointDifferenceToWin { get; set; }
    public int SetsToWin { get; set; }
    
    public GameTeam FirstTeam { get; set; }
    public GameTeam Secondteam { get; set; }
    public ICollection<Set> Sets { get; set; }
    
    public bool IsPrivate { get; set; }
    public bool CreationDate { get; set; }
    public bool LastEditDate { get; set; }
    public bool IsStarted { get; set; }
    public DateTime StartDate { get; set; }
    public GameTeam? Winner { get; set; }
    public DateTime FinishDate { get; set; }
    
    public ICollection<Team.Team> RequestedTeams { get; set; }
    public ICollection<Team.Team> BlockedTeams { get; set; }
    
    public Guid OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }
}

public class GameTeam
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string ProfilePicture { get; set; }
    public string Description { get; set; }
    
    public ICollection<GameTeamPlayer> Players { get; set; }
}

public class GameTeamPlayer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public GameTeam GameTeam { get; set; }
}