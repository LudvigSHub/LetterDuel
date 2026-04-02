using LetterDuel.Backend.Domain;
using Microsoft.EntityFrameworkCore;

namespace LetterDuel.Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<GameWord> GameWords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
               .HasMany(g => g.Players)
               .WithOne(p => p.Game)
               .HasForeignKey(p => p.GameId)
               .OnDelete(DeleteBehavior.Cascade);

            
            
        }


    }
}
