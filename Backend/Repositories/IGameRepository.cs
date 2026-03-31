using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.Repositories
{
    public interface IGameRepository
    {
        //hämtar ett game baserat på id
        Task<Game?> GetByIdAsync(Guid id);

        //spara new game eller updatera ett befintligt game
        Task SaveAsync(Game game);
    }
}
