using Backend.Data.Entities.Game;

namespace Backend.Data.Dtos.Tournament;

public class ReorderTeamsDto
{
    public Dictionary<Guid, int> UpdatedNumbers { get; set; }
}