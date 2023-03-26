namespace Backend.Data.Dtos.Game;

public class AddGameDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int PointsToWin { get; set; }
    public int PointDifferenceToWin { get; set; }
    public int MaxSets { get; set; }
    public int PlayersPerTeam { get; set; }
    public bool IsPrivate { get; set; }
}