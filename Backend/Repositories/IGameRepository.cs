using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.Repositories
{
    public interface IGameRepository
    {
        //hämtar ett game baserat på id
        Game? GetById(Guid id);

        //spara new game eller updatera ett befintligt game
        void Save(Game game);
    }
}
