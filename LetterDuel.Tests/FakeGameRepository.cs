using LetterDuel.Backend.Domain;
using LetterDuel.Backend.Repositories;

namespace LetterDuel.Tests
{
    public class FakeGameRepository : IGameRepository
    {
        private readonly List<Game> _games = new();

        public List<GameWord> Words { get; } = new();

        public Task<Game?> GetAsync(Guid id)
        {
            return Task.FromResult(_games.FirstOrDefault(g => g.Id == id));
        }

        public Task<Game> AddAsync(Game game)
        {
            _games.Add(game);
            return Task.FromResult(game);
        }

        public Task UpdateAsync(Game game)
        {
            return Task.CompletedTask;
        }

        public Task<List<GameWord>> GetAllWordsAsync()
        {
            return Task.FromResult(Words.ToList());
        }

        public Task AddPlayerAsync(Player player)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}