namespace Backend.Data.Dtos.Game;

public class EditGameDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? PointsToWin { get; set; }
    public int? PointDifferenceToWin { get; set; }
    public int? MaxSets { get; set; }
    public bool? IsPrivate { get; set; }
}