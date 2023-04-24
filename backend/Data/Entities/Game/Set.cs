namespace Backend.Data.Entities.Game;

public class Set
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public GameTeam FirstTeam { get; set; }
    public GameTeam SecondTeam { get; set; }
    public IList<SetPlayer> Players { get; set; }
    public int FirstTeamScore { get; set; }
    public int SecondTeamScore { get; set; }
    public GameStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public GameTeam? Winner { get; set; }
    public DateTime? FinishDate { get; set; }
    public Game Game { get; set; }
}

public enum SetPlayerStats
{
    Kills = 0,
    Errors = 1,
    Attempts = 2,
    SuccessfulBlocks = 3,
    Blocks = 4,
    Touches = 5,
    BlockingErrors = 6,
    Aces = 7,
    ServingErrors = 8,
    TotalServes = 9,
    SuccessfulDigs = 10,
    BallTouches = 11,
    BallMisses = 12
}

public class SetPlayer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public uint Score { get; set; }
    public uint? Kills { get; set; }
    public uint? Errors { get; set; }
    public uint? Attempts { get; set; }
    public uint? SuccessfulBlocks { get; set; }
    public uint? Blocks { get; set; }
    public uint? Touches { get; set; }
    public uint? BlockingErrors { get; set; }
    public uint? Aces { get; set; }
    public uint? ServingErrors { get; set; }
    public uint? TotalServes { get; set; }
    public uint? SuccessfulDigs { get; set; }
    public uint? BallTouches { get; set; }
    public uint? BallMisses { get; set; }
    public bool Team { get; set; }
}