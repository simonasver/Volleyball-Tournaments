using Backend.Data.Entities.Auth;
using Backend.Data.Entities.Tournament;

namespace Backend.Data.Entities.Game;

public enum GameStatus
{
    New,
    SingleTeam,
    Ready,
    Started,
    Finished
}

public class Game
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? PictureUrl { get; set; }
    public string? Description { get; set; }
    public bool Basic { get; set; }
    public int PointsToWin { get; set; }
    public int PointsToWinLastSet { get; set; }
    public int PointDifferenceToWin { get; set; }
    public int MaxSets { get; set; }
    public int PlayersPerTeam { get; set; }

    public GameTeam? FirstTeam { get; set; }
    public GameTeam? SecondTeam { get; set; }
    public ICollection<Set> Sets { get; set; }
    public int FirstTeamScore { get; set; }
    public int SecondTeamScore { get; set; }
    
    public bool IsPrivate { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastEditDate { get; set; }
    public GameStatus Status { get; set; }
    public DateTime? StartDate { get; set; }
    public GameTeam? Winner { get; set; }
    public DateTime? FinishDate { get; set; }
    
    public ICollection<Team.Team> RequestedTeams { get; set; }
    
    public TournamentMatch? TournamentMatch { get; set; }

    public string OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }
}

public class GameTeam
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Description { get; set; }
    public bool? Duplicate { get; set; }
    
    public ICollection<GameTeamPlayer> Players { get; set; }
    
    public Tournament.Tournament? Tournament { get; set; }

    public GameTeam Copy()
    {
        var newGameTeam = new GameTeam();
        List<GameTeamPlayer> newPlayers = new List<GameTeamPlayer>();
        foreach (var gameTeamPlayer in this.Players)
        {
            var newNewPlayer = new GameTeamPlayer()
            {
                Name = gameTeamPlayer.Name,
                GameTeam = newGameTeam
            };
            newPlayers.Add(newNewPlayer);
        }
        
        newGameTeam.Title = this.Title;
        newGameTeam.ProfilePicture = this.ProfilePicture;
        newGameTeam.Description = this.Description;
        newGameTeam.Duplicate = true;
        newGameTeam.Players = newPlayers;
        newGameTeam.Tournament = this.Tournament;
        
        return newGameTeam;
    }
}

public class GameTeamPlayer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public GameTeam GameTeam { get; set; }
}