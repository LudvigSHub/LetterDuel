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

            modelBuilder.Entity<GameWord>().HasData(
                new GameWord { Id = 1, Word = "Knowledge" },
                new GameWord { Id = 2, Word = "Dangerous" },
                new GameWord { Id = 3, Word = "Discovery" }
            );
        }


    }
}
