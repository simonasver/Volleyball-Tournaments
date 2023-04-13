using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class TournamentService : ITournamentService
{
    public Tournament AddTeamToTournament(Tournament tournament, Team team)
    {
        tournament.RequestedTeams.Remove(team);
        var gameTeam = new GameTeam()
        {
            Title = team.Title,
            ProfilePicture = team.PictureUrl,
            Description = team.Description,
            Players = new List<GameTeamPlayer>()
        };
        foreach (var teamPlayer in team.Players)
        {
            var gameTeamPlayer = new GameTeamPlayer()
            {
                Name = teamPlayer.Name,
            };
            gameTeam.Players.Add(gameTeamPlayer);
        }

        if (tournament.AcceptedTeams.Count >= tournament.MaxTeams)
        {
            throw new InvalidOperationException("Tournament is already full");
        }
        else
        {
            tournament.AcceptedTeams.Add(gameTeam);
        }

        return tournament;
    }

    public TournamentMatch GenerateEmptyBracket(Tournament tournament, int roundCount)
    {
        var games = GenerateParentGames(
            tournament,
            new TournamentMatch()
            {
                Tournament = tournament,
                Round = roundCount + 1,
                Game = new Game()
                {
                    Title = tournament.Title + " game " + roundCount + 1,
                    PointsToWin = tournament.PointsToWin,
                    PointDifferenceToWin = tournament.PointDifferenceToWin,
                    MaxSets = tournament.MaxSets,
                    PlayersPerTeam = tournament.PlayersPerTeam,
                    FirstTeamScore = 0,
                    SecondTeamScore = 0,
                    IsPrivate = tournament.IsPrivate,
                    CreateDate = DateTime.Now,
                    LastEditDate = DateTime.Now,
                    Status = GameStatus.New,
                    OwnerId = tournament.OwnerId
                }
            }, roundCount);
        return games;
    }

    private TournamentMatch GenerateParentGames(Tournament tournament, TournamentMatch childMatch, int currentRound)
    {
        if (currentRound < 0)
        {
            return null;
        }

        childMatch.FirstParent =
            GenerateParentGames(
                tournament,
                new TournamentMatch()
                {
                    Tournament = childMatch.Tournament,
                    Round = currentRound,
                    Game = new Game()
                    {
                        Title = tournament.Title + " game " + currentRound,
                        PointsToWin = tournament.PointsToWin,
                        PointDifferenceToWin = tournament.PointDifferenceToWin,
                        MaxSets = tournament.MaxSets,
                        PlayersPerTeam = tournament.PlayersPerTeam,
                        FirstTeamScore = 0,
                        SecondTeamScore = 0,
                        IsPrivate = tournament.IsPrivate,
                        CreateDate = DateTime.Now,
                        LastEditDate = DateTime.Now,
                        Status = GameStatus.New,
                        OwnerId = tournament.OwnerId
                    }
                }, currentRound - 1);

        childMatch.SecondParent =
            GenerateParentGames(
                tournament,
                new TournamentMatch()
                {
                    Tournament = childMatch.Tournament,
                    Round = currentRound,
                    Game = new Game()
                    {
                        Title = tournament.Title + " game " + currentRound,
                        PointsToWin = tournament.PointsToWin,
                        PointDifferenceToWin = tournament.PointDifferenceToWin,
                        MaxSets = tournament.MaxSets,
                        PlayersPerTeam = tournament.PlayersPerTeam,
                        FirstTeamScore = 0,
                        SecondTeamScore = 0,
                        IsPrivate = tournament.IsPrivate,
                        CreateDate = DateTime.Now,
                        LastEditDate = DateTime.Now,
                        Status = GameStatus.New,
                        OwnerId = tournament.OwnerId
                    }
                }, currentRound - 1);
        return childMatch;
    }
}