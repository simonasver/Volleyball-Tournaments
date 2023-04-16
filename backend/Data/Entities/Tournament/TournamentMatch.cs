namespace Backend.Data.Entities.Tournament;

public class TournamentMatch
{
    public Guid Id { get; set; }
    public int Round { get; set; }
    public Game.Game Game { get; set; }
    public IList<TournamentMatch>? Parents { get; set; }
    public TournamentMatch? Child { get; set; }
    public Tournament Tournament { get; set; }
}