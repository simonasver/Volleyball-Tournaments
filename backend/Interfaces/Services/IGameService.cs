using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;

namespace Backend.Interfaces.Services;

public interface IGameService
{
    Game AddTeamToGame(Game game, Team team);
    Game AddSetToGame(Game game, int number);
}