namespace Backend.Data.Entities.Tournament;

public class TournamentMatch
{
    public Guid Id { get; set; }
    public int Round { get; set; }
    public bool ThirdPlace { get; set; }
    public Game.Game Game { get; set; }
    public Guid? FirstParentId { get; set; }
    public TournamentMatch? FirstParent { get; set; }
    public Guid? SecondParentId { get; set; }
    public TournamentMatch? SecondParent { get; set; }
    public Guid TournamentId { get; set; }
    public Tournament Tournament { get; set; }
}