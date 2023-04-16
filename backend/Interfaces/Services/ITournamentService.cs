using System.Collections;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;

namespace Backend.Interfaces.Services;

public interface ITournamentService
{
    public Tournament AddTeamToTournament(Tournament tournament, Team team);
    public TournamentMatch GenerateEmptyBracket(Tournament tournament, int roundCount);
    public IList<TournamentMatch> PopulateEmptyBrackets(IList<TournamentMatch> tournamentMatches, IList<GameTeam> teams);
    public (Tournament, ICollection<TournamentMatch>) MatchesToUpdateInTournamentAfterWonMatch(Tournament tournament, TournamentMatch tournamentMatch);

    public IEnumerable<TournamentMatch> MoveMatchTeamDown(ICollection<TournamentMatch> tournamentMatches,
        TournamentMatch tournamentMatch);
}