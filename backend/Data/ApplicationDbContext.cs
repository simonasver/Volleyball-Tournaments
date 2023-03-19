using Backend.Auth.Model;
using Backend.Data.Entities.Team;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamPlayer> TeamPlayers { get; set; }

        public ApplicationDbContext(IConfiguration configuration, DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {
            _configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Team>().HasMany(t => t.Players).WithOne(tp => tp.Team);
        }
    }
}
