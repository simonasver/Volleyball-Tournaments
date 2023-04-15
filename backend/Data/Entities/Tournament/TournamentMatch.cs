namespace Backend.Data.Entities.Tournament;

public class TournamentMatch
{
    public Guid Id { get; set; }
    public int Round { get; set; }
    public Game.Game Game { get; set; }
    public TournamentMatch? FirstParent { get; set; }
    public TournamentMatch? SecondParent { get; set; }
    public Tournament Tournament { get; set; }
}