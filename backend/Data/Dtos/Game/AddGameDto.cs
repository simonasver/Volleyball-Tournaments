using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Dtos.Game;

public class AddGameDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    
    [Range(1, 100, ErrorMessage = "Points to win must be between 1 and 100")]
    public int PointsToWin { get; set; }
    
    [Range(0, 10, ErrorMessage = "Point difference to win must be between 0 and 10")]
    public int PointDifferenceToWin { get; set; }
    
    [Range(1, 5, ErrorMessage = "Max sets must be between 1 and 5")]
    public int MaxSets { get; set; }
    
    [Range(0, 12, ErrorMessage = "Players per team must be between 1 and 12")]
    public int PlayersPerTeam { get; set; }
    public bool IsPrivate { get; set; }
}