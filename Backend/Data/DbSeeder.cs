using LetterDuel.Backend.Domain;
using Microsoft.EntityFrameworkCore;

namespace LetterDuel.Backend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IConfiguration configuration)
        {
            await context.Database.MigrateAsync();

            var words = configuration
                .GetSection("SeedWords")
                .Get<string[]>() ?? Array.Empty<string>();

            if (words.Length == 0)
                return;

            var existingWords = await context.GameWords.ToListAsync();

            if (existingWords.Any())
            {
                context.GameWords.RemoveRange(existingWords);
                await context.SaveChangesAsync();
            }

            var gameWords = words
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Select(word => new GameWord
                {
                    Word = word.Trim().ToUpperInvariant()
                })
                .ToList();

            context.GameWords.AddRange(gameWords);
            await context.SaveChangesAsync();
        }

    }
}
