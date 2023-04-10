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
        return GenerateParentGame(new TournamentMatch(){ Tournament = tournament }, roundCount);
    }

    private TournamentMatch GenerateParentGame(TournamentMatch childMatch, int currentRound)
    {
        if (currentRound == 0)
        {
            return null;
        }

        childMatch.FirstParent =
            GenerateParentGame(new TournamentMatch() { Tournament = childMatch.Tournament }, currentRound - 1);

        childMatch.SecondParent =
            GenerateParentGame(new TournamentMatch() { Tournament = childMatch.Tournament }, currentRound - 1);

        return childMatch;
    }
}