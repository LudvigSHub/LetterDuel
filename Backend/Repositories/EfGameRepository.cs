using LetterDuel.Backend.Data;
using LetterDuel.Backend.Domain;
using Microsoft.EntityFrameworkCore;

namespace LetterDuel.Backend.Repositories
{
    public class EfGameRepository : IGameRepository
    {
        private readonly AppDbContext _context;

        public EfGameRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetAsync(Guid id)
        {
            return await _context.Games
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> AddAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public async Task UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }

        public async Task<List<GameWord>> GetAllWordsAsync()
        {
            return await _context.GameWords.ToListAsync();
        }
    }
}