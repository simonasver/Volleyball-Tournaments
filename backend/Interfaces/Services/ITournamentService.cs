using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;

namespace Backend.Interfaces.Services;

public interface ITournamentService
{
    public Tournament AddTeamToTournament(Tournament tournament, Team team);
    public TournamentMatch GenerateEmptyBracket(Tournament tournament, int roundCount);
}