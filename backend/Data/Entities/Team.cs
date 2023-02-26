using Backend.Auth.Model;

namespace Backend.Data.Entities
{
    public class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        // public List<TeamPlayer> Players { get; set; }
        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
