using LetterDuel.Backend.Domain;
using LetterDuel.Backend.Repositories;

public class FakeGameRepository : IGameRepository
{
    private readonly Dictionary<Guid, Game> _games = new();

    public Task<Game?> GetAsync(Guid id)
    {
        _games.TryGetValue(id, out var game);
        return Task.FromResult(game);
    }

    public Task<Game> AddAsync(Game game)
    {
        _games[game.Id] = game;
        return Task.FromResult(game);
    }

    public Task UpdateAsync(Game game)
    {
        _games[game.Id] = game;
        return Task.CompletedTask;
    }
}