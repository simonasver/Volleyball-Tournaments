using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;
using Backend.Interfaces.Services;

namespace Backend.Services;

public class GameService : IGameService
{
    public Game AddTeamToGame(Game game, Team team)
    {
        game.RequestedTeams.Remove(team);
        var gameTeam = new GameTeam()
        {
            Title = team.Title,
            ProfilePicture = team.PictureUrl,
            Description = team.Description
        };
        foreach (var teamPlayer in team.Players)
        {
            var gameTeamPlayer = new GameTeamPlayer()
            {
                Name = teamPlayer.Name,
            };
            gameTeam.Players.Add(gameTeamPlayer);
        }
        if (game.FirstTeam == null)
        {
            game.FirstTeam = gameTeam;
        }
        else if (game.SecondTeam == null)
        {
            game.SecondTeam = gameTeam;
        }
        else throw new InvalidOperationException("Game already has two teams");

        if ((game.FirstTeam != null && game.SecondTeam == null) || (game.FirstTeam == null && game.SecondTeam != null))
        {
            game.Status = GameStatus.SingleTeam;
        }
        else if (game.FirstTeam != null && game.SecondTeam != null)
        {
            game.Status = GameStatus.Ready;
        }
        
        return game;
    }
    public Game AddSetToGame(Game game)
    {
        var newSet = new Set()
        {
            FirstTeam = game.FirstTeam,
            SecondTeam = game.SecondTeam
        };
        foreach (var firstTeamPlayer in game.FirstTeam.Players)
        {
            var newSetPlayer = new SetPlayer()
            {
                Name = firstTeamPlayer.Name,
                Score = 0,
                Team = false
            };
            newSet.Players.Add(newSetPlayer);
        }
        foreach (var secondTeamPlayer in game.SecondTeam.Players)
        {
            var newSetPlayer = new SetPlayer()
            {
                Name = secondTeamPlayer.Name,
                Score = 0,
                Team = false
            };
            newSet.Players.Add(newSetPlayer);
        }
        game.Sets.Add(newSet);
        return game;
    }
}