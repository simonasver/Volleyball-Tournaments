using Backend.Data.Entities.Team;
using Backend.Data.Entities.Tournament;

namespace Backend.Interfaces.Services;

public interface ITournamentService
{
    public Tournament AddTeamToGame(Tournament tournament, Team team);
}