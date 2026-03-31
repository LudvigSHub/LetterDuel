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

        public Game? GetById(Guid id)
        {
            //Include behövs för att Players ska laddas med spelet
            return _context.Games
                .Include(g => g.Players)
                .FirstOrDefault(g => g.Id == id);
        }

        public void Save(Game game)
        {
            //Om spelet inte redan finns i databasen läggs det till
            var existingGame = _context.Games
                .Include(g => g.Players)
                .FirstOrDefault(g => g.Id == game.Id);

            if (existingGame == null)
            {
                _context.Games.Add(game);
            }
            else
            {
                //Uppdatera enkla fält på Game
                existingGame.SecretWord = game.SecretWord;
                existingGame.State = game.State;
                existingGame.GuessedLetters = game.GuessedLetters;
                existingGame.CurrentPlayerIndex = game.CurrentPlayerIndex;

                //Om nya spelare lagts till, lägg till dem
                foreach (var player in game.Players)
                {
                    if (!existingGame.Players.Any(p => p.Id == player.Id))
                    {
                        existingGame.Players.Add(player);
                    }
                    else
                    {
                        var existingPlayer = existingGame.Players.First(p => p.Id == player.Id);
                        existingPlayer.Name = player.Name;
                        existingPlayer.Score = player.Score;
                        existingPlayer.GameId = player.GameId;
                    }
                }
            }

            _context.SaveChanges();
        }
    }
}