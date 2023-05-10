using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Dtos.Tournament;

public class RequestJoinTournamentDto
{
    public Guid TeamId { get; set; }
}