using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.Repositories
{
    public class InMemoryGameRepository : IGameRepository
    {
        //lagrar spel med id som nyckel i dictionary
        private readonly Dictionary<Guid, Game> _games = new();

        public Game? GetById(Guid id)
        {
            //försöker hitta spelet i dictionaryn (id) 
            if (_games.TryGetValue(id, out var game)) //om game med det id finns
            {
                return game; //returnera
            }

            return null; //annars null
        }

        public void Save(Game game)
        {
            //uppdatera spelet om det redan finns annars lägg till det i dictionary
            _games[game.Id] = game;
        }
    }
}
