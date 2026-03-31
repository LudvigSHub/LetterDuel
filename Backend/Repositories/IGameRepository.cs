using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.Repositories
{
    public interface IGameRepository
    {
        //hämtar ett game baserat på id
        Task<Game?> GetAsync(Guid id);

        Task<Game> AddAsync(Game game);
        //spara new game eller updatera ett befintligt game
        Task UpdateAsync(Game game);
    }
}
