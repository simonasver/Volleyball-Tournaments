using Backend.Data.Entities;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Log;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;

namespace TestProject;

public class Helpers
{
    /// <summary>
    /// Returns data to test teams
    /// </summary>
    /// <returns>List of 3 teams: 1. OwnerId="first", 1 player; 2. OwnerId="first", 1 player; 3. OwnerId="second", 0 players;</returns>
    public static IList<Team> GetTestTeamData()
    {
        var _team1 = new Team()
        {
            Id = new Guid(),
            Title = "Team1",
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            OwnerId = "first",
            Players = new List<TeamPlayer>()
        };
        var _team2 = new Team(){
            Id = new Guid(),
            Title = "Team2",
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            OwnerId = "first",
            Players = new List<TeamPlayer>()
        };
        var _team3 = new Team(){
            Id = new Guid(),
            Title = "Team3",
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            OwnerId = "second",
            Players = new List<TeamPlayer>()
        };
        
        var _player1 = new TeamPlayer()
        {
            Id = new Guid(),
            Name = "Player1",
            Team = _team1
        };
        _team1.Players.Add(_player1);
        var _player2 = new TeamPlayer()
        {
            Id = new Guid(),
            Name = "Player2",
            Team = _team2
        };
        _team2.Players.Add(_player2);

        return new List<Team> { _team1, _team2, _team3 };
    }

    public static Team GetTeamWithNoPlayers()
    {
        return new Team()
        {
            Id = new Guid(),
            Title = "TeamWithNoPlayers",
            CreateDate = DateTime.Now,
            LastEditDate = DateTime.Now,
            OwnerId = "first",
            Players = new List<TeamPlayer>()
        };
    }

    public static IList<Game> GetTestGameData()
    {
        var _game1 = new Game
        {
            Id = new Guid(),
            Basic = false,
            CreateDate = DateTime.Now,
            FirstTeamScore = 0,
            SecondTeamScore = 0,
            IsPrivate = false,
            LastEditDate = DateTime.Now,
            OwnerId = "first",
            Status = GameStatus.New,
            RequestedTeams = new List<Team>()
        };
        var _game2 = new Game
        {
            Id = new Guid(),
            Basic = false,
            CreateDate = DateTime.Now,
            FirstTeamScore = 0,
            SecondTeamScore = 0,
            IsPrivate = false,
            LastEditDate = DateTime.Now,
            OwnerId = "first",
            Status = GameStatus.New,
            RequestedTeams = new List<Team>(),
            PlayersPerTeam = 2
        };
        var _game3 = new Game
        {
            Id = new Guid(),
            Basic = false,
            CreateDate = DateTime.Now,
            FirstTeamScore = 0,
            SecondTeamScore = 0,
            IsPrivate = false,
            LastEditDate = DateTime.Now,
            OwnerId = "second",
            Status = GameStatus.Finished,
            RequestedTeams = new List<Team>()
        };

        return new List<Game> { _game1, _game2, _game3 };
    }

    public static Game GetGameWithTournament()
    {
        TournamentMatch tournamentMatch = new TournamentMatch();
        return new Game
        {
            Id = new Guid(),
            Basic = false,
            CreateDate = DateTime.Now,
            FirstTeamScore = 0,
            SecondTeamScore = 0,
            IsPrivate = false,
            LastEditDate = DateTime.Now,
            OwnerId = "second",
            Status = GameStatus.Started,
            TournamentMatch = tournamentMatch
        };
    }
    
    public static Game GetGameWithTournamentAndRequestedTeam()
    {
        TournamentMatch tournamentMatch = new TournamentMatch();
        return new Game
        {
            Id = new Guid(),
            Basic = false,
            CreateDate = DateTime.Now,
            FirstTeamScore = 0,
            SecondTeamScore = 0,
            IsPrivate = false,
            LastEditDate = DateTime.Now,
            OwnerId = "second",
            Status = GameStatus.Started,
            TournamentMatch = tournamentMatch,
            RequestedTeams = new List<Team>()
            {
                GetTestTeamData()[0]
            }
        };
    }

    public static GameTeam GetGameTeam(int playerCount = 0)
    {
        var gameTeam = new GameTeam()
        {
            Id = new Guid(),
            Title = "GameTeam",
            Players = new List<GameTeamPlayer>()
        };
        for (int i = 0; i < playerCount; i++)
        {
            gameTeam.Players.Add(new GameTeamPlayer());
        }
        return gameTeam;
    }

    public static IList<Tournament> GetTestTournamentData()
    {
        var _tournament1 = new Tournament
        {
            AcceptedTeams = new List<GameTeam>(),
            Winner = null,
            Basic = false,
            PointsToWin = 0,
            PointsToWinLastSet = 0,
            PointDifferenceToWin = 0,
            MaxSets = 0,
            PlayersPerTeam = 0,
            CreateDate = DateTime.Now,
            FinalRound = 5,
            Id = new Guid(),
            Title = null,
            PictureUrl = null,
            Description = null,
            SingleThirdPlace = false,
            MaxTeams = 0,
            IsPrivate = false,
            LastEditDate = DateTime.Now,
            Status = TournamentStatus.Open,
            RequestedTeams = null,
            Matches = new List<TournamentMatch>(),
            OwnerId = null,
            Owner = null,
        };
        
        var match1 = new TournamentMatch
        {
            Id = Guid.NewGuid(),
            Round = 0,
            ThirdPlace = false,
            Game = null,
            FirstParentId = null,
            FirstParent = null,
            SecondParentId = null,
            SecondParent = null,
            TournamentId = default,
            Tournament = _tournament1
        };
        _tournament1.Matches.Add(match1);
        
        var match2 = new TournamentMatch
        {
            Id = Guid.NewGuid(),
            Round = 0,
            ThirdPlace = false,
            Game = null,
            FirstParentId = null,
            FirstParent = null,
            SecondParentId = null,
            SecondParent = null,
            TournamentId = default,
            Tournament = _tournament1
        };
        _tournament1.Matches.Add(match2);

        var match3 = new TournamentMatch
        {
            Id = Guid.NewGuid(),
            Round = 0,
            ThirdPlace = false,
            Game = null,
            FirstParentId = null,
            FirstParent = null,
            SecondParentId = null,
            SecondParent = null,
            TournamentId = default,
            Tournament = null
        };
        _tournament1.Matches.Add(match3);
        
        var match4 = new TournamentMatch
        {
            Id = Guid.NewGuid(),
            Round = 0,
            ThirdPlace = false,
            Game = null,
            FirstParentId = null,
            FirstParent = null,
            SecondParentId = null,
            SecondParent = null,
            TournamentId = default,
            Tournament = null
        };
        _tournament1.Matches.Add(match4);

        return new List<Tournament> { _tournament1 };
    }

    public static IList<Log> GetTestLogData()
    {
        var _log1 = new Log
        {
            Id = default,
            IsPrivate = false,
            Tournament = null,
            Game = null,
            Time = default,
            Message = null,
            OwnerId = null,
            Owner = null
        };
        return new List<Log> { _log1 };
    }
}