using Backend.Data.Entities.Auth;

namespace Backend.Data.Entities.Log;

public class Log
{
    public Guid Id { get; set; }
    public bool IsPrivate { get; set; }
    public Tournament.Tournament? Tournament { get; set; }
    public Game.Game? Game { get; set; }
    public DateTime Time { get; set; }
    public string Message { get; set; }
    public string OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }
}