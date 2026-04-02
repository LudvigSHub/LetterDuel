using LetterDuel.Backend.Domain;
using Microsoft.EntityFrameworkCore;

namespace LetterDuel.Backend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
        {
            await context.Database.MigrateAsync();

            if (context.GameWords.Any())
                return;

            var words = configuration
                .GetSection("SeedWords")
                .Get<string[]>() ?? Array.Empty<string>();

            if (words.Length == 0)
                return;

            var gameWords = words
                .Select(word => new GameWord { Word = word })
                .ToList();

            context.GameWords.AddRange(gameWords);
            await context.SaveChangesAsync();
        }

    }
}
