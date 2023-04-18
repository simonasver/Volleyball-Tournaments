using Backend.Data.Entities.Game;

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
}