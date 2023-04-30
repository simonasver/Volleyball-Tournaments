using Backend.Data.Dtos.Game;
using Backend.Data.Entities.Game;
using Backend.Data.Entities.Team;

namespace Backend.Helpers.Utils;

public class GameUtils
{
    /// <summary>
    /// Determines, whether the set could be last for the game
    /// </summary>
    /// <param name="game"></param>
    /// <param name="set"></param>
    public static bool IsFinalSet(Game game, Set set)
    {
        return (game.FirstTeamScore == (((game.MaxSets + 1) / 2) - 1) ||
               game.SecondTeamScore == (((game.MaxSets + 1) / 2) - 1));
    }
    public static Game AddTeamToGame(Game game, Team team)
    {
        game.RequestedTeams.Remove(team);
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
    public static Game AddSetToGame(Game game, int number = 0)
    {
        var newSet = new Set()
        {
            Number = number + 1,
            FirstTeam = game.FirstTeam,
            SecondTeam = game.SecondTeam,
            Players = new List<SetPlayer>()
        };
        foreach (var firstTeamPlayer in game.FirstTeam.Players)
        {
            var newSetPlayer = new SetPlayer()
            {
                Name = firstTeamPlayer.Name,
                Score = 0,
                Kills = 0,
                Errors = 0,
                Attempts = 0,
                SuccessfulBlocks = 0,
                Blocks = 0,
                Touches = 0,
                BlockingErrors = 0,
                Aces = 0,
                ServingErrors = 0,
                TotalServes = 0,
                SuccessfulDigs = 0,
                BallTouches = 0,
                BallMisses = 0,
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
                Kills = 0,
                Errors = 0,
                Attempts = 0,
                SuccessfulBlocks = 0,
                Blocks = 0,
                Touches = 0,
                BlockingErrors = 0,
                Aces = 0,
                ServingErrors = 0,
                TotalServes = 0,
                SuccessfulDigs = 0,
                BallTouches = 0,
                BallMisses = 0,
                Team = true
            };
            newSet.Players.Add(newSetPlayer);
        }
        game.Sets.Add(newSet);
        return game;
    }
    
    public static GameTeam FindGameLoser(Game game) 
    {
        if (game.Winner == null)
        {
            throw new InvalidOperationException("Game does not have a winner");
        }

        if (game.FirstTeam == game.Winner)
        {
            return game.SecondTeam;
        }
        else
        {
            return game.FirstTeam;
        }
    }

    public static GameTeam FindOtherTeam(Game game, GameTeam team)
    {
        if (game.FirstTeam == team && game.SecondTeam != null)
        {
            return game.SecondTeam;
        }
        if (game.SecondTeam == team && game.FirstTeam != null)
        {
            return game.FirstTeam;
        }

        throw new Exception("Not found");
    }
}