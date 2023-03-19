namespace Backend.Data.Entities.Team
{
    public class TeamPlayer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Team Team { get; set; }
    }
}

