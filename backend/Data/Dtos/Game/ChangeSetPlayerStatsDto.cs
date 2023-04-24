using Backend.Data.Entities.Game;

namespace Backend.Data.Dtos.Game;

public class ChangeSetPlayerStatsDto
{
    public SetPlayerStats Type { get; set; }
    public bool Change { get; set; }
}