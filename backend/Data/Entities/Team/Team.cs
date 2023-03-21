using Backend.Auth.Model;

namespace Backend.Data.Entities.Team
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? PictureUrl { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public ICollection<TeamPlayer> Players { get; set; }
        
        public ICollection<Game.Game> GamesRequestedTo { get; set; }
        public ICollection<Game.Game> GamesBlockedFrom { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
    }
    
    public class TeamPlayer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Team Team { get; set; }
    }
}
