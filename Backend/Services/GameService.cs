using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.Services
{
    public class GameService
    {
        public Game CreateGame(string secretWord)
        {
            return new Game
            {
                SecretWord = secretWord.ToUpperInvariant(),
                State = GameState.WaitingForPlayers,
                CurrentPlayerIndex = 0
            };
        }

        public void AddPlayer(Game game, Player player)
        {
            if (game.Players.Count >= 2)
            {
                throw new InvalidOperationException("Game already has two players.");
            }

            player.GameId = game.Id;
            game.Players.Add(player);

            if (game.Players.Count == 2)
            {
                game.State = GameState.InProgress;
            }
        }

        public void GuessLetter(Game game, Guid playerId, char letter)
        {
            if (game.State != GameState.InProgress)
            {
                throw new InvalidOperationException("Game is not in progress");
            }

            if (game.Players[game.CurrentPlayerIndex].Id != playerId)
            {
                throw new InvalidOperationException("It is not this players turn");
            }
            letter = char.ToUpperInvariant(letter);

            if (game.GuessedLetters.Contains(letter))
            {
                throw new InvalidOperationException("Letter has already been guessed.");
            }
            game.GuessedLetters.Add(letter);

            if (game.SecretWord.Contains(letter))
            {
                var currentPlayer = game.Players[game.CurrentPlayerIndex];
                currentPlayer.Score += IsVowel(letter) ? 2 : 4;
            }

            if (IsWordFullyGuessed(game))
            {
                game.State = GameState.GameFinished;
                return;
            }
            game.CurrentPlayerIndex = (game.CurrentPlayerIndex + 1) % game.Players.Count;
        }

        //returnerar ordet i maskerad form, där ogissade bokstäver visas som _.
        public string GetMaskedWord(Game game)
        {
            return new string(
                game.SecretWord
                .Select(letter => game.GuessedLetters.Contains(letter) ? letter : '_')
                .ToArray());
        }
    }
}
