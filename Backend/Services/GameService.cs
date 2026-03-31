using System.ComponentModel.Design;
using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.Services
{
    //ansvarar för spelets regler och flöde
    // här hanteras skapande av spel, spelare, bokstavsgissningar, poäng och vinnare
    public class GameService
    {

        private readonly IGameRepository _repo;

        public GameService(IGameRepository repo)
        {
            _repo = repo;
        }
        //skapar ett nytt game och ersätter startvärden för spelets state
        public async Task<Game> CreateGame(string secretWord, string playerName)
        {
            var game = new Game
            {
                //ordet sparas i versaler för att förenkla jämförelser av bokstäver
                SecretWord = secretWord.ToUpperInvariant(),
                //ett nytt spel börjar altid med att vänta på spelare
                State = GameState.WaitingForPlayers,
                //första spelaren i listan får första turen när spelet startar
                CurrentPlayerIndex = 0
            };

            //player1 skapas med spelet
            var player = new Player
            {
                Name = playerName,
                GameId = game.Id
            };

            game.Players.Add(player);

            return await _repo.AddAsync(game);
        }

        //Lägger till en spelare i spelet
        public async Task<Game?> AddPlayer(Guid gameId, string playerName)
        {
            var game = await _repo.GetAsync(gameId);

            if (game == null)
                return null;
            //spelet får bara innehålla två spelare.
            if (game.Players.Count >= 2)
            {
                throw new InvalidOperationException("Game already has two players.");
            }
            //kopplar spelaren till aktuella spelet
            var player = new Player
            {
                Name = playerName,
                GameId = game.Id
            };

            game.Players.Add(player);

            if (game.Players.Count == 2)
                game.State = GameState.InProgress;

            await _repo.UpdateAsync(game);

            return game;
        }

        //hanterar en spelares bokstavsgissning
        public async Task<Game?> GuessLetter(Guid gameId, Guid playerId, string input)
        {
            var game = await _repo.GetAsync(gameId);

            if (game == null)
                return null;
            //det går bara att gissa när spel är igång
            if (game.State != GameState.InProgress)
            {
                throw new InvalidOperationException("Game is not in progress");
            }

            //kontrollera att det är rätt spelares tur
            if (game.Players[game.CurrentPlayerIndex].Id != playerId)
            {
                throw new InvalidOperationException("It is not this players turn");
            }

            //kontroll om input är exakt 1 bokstav från det engelska alfabetet
            if (!IsSingleEnglishLetter(input))
            {
                throw new InvalidOperationException("Only letters in the English alphabet A-Z are allowed.");
            }

            //bokstaven görs om till versal för att matcha sparade ordet
            char letter = char.ToUpperInvariant(input[0]);

            //Om bokstaven redan gissats.
            if (game.GuessedLetters.Contains(letter))
            {
                throw new InvalidOperationException("Letter has already been guessed.");
            }

            game.GuessedLetters += letter;

            //om gissning är rätt
            if (game.SecretWord.Contains(letter))
            {
                var player = game.Players[game.CurrentPlayerIndex];
                var count = game.SecretWord.Count(c => c == letter);
                var points = IsVowel(letter) ? 2 : 4;

                player.Score += count * points;
            }

            //om alla bokstäver är gissade, avsluta Game
            if (IsWordFullyGuessed(game))
            {
                game.State = GameState.GameFinished;
                await _repo.UpdateAsync(game);
                return game;
            }

            game.CurrentPlayerIndex = 
                (game.CurrentPlayerIndex + 1) % game.Players.Count;

            await _repo.UpdateAsync(game);

            return game;
        }

        // Get game
        public async Task<Game?> GetGame(Guid gameId)
        {
            return await _repo.GetAsync(gameId);
        }

        //returnerar ordet i maskerad form, där ogissade bokstäver visas som _.
        public string GetMaskedWord(Game game)
        {
            return new string(
                game.SecretWord
                .Select(letter => game.GuessedLetters.Contains(letter) ? letter : '_')
                .ToArray());
        }

        //lista med gissade bokstäver
        public List<char> GetGuessedLetters(Game game)
        {
            return game.GuessedLetters
                .OrderBy(letter => letter)
                .ToList();
        }

        //Kollar om alla unika bokstäver i ordet har blivit gissade
        public bool IsWordFullyGuessed(Game game)
        {
            return game.SecretWord
                .Distinct()
                .All(letter => game.GuessedLetters.Contains(letter));
        }

        //returnerar vinnaren när spelet är färdigt
        public Player? GetWinner(Game game)
        {
            //Ingen vinnare kan utses innan spel är färdigt
            if (game.State != GameState.GameFinished)
            {
                return null;
            }
            //spelaren med högst poäng vinner
            return game.Players
                .OrderByDescending(p => p.Score)
                .FirstOrDefault();
        }

        //hjälpmetod som avngör om bokstav = vokal.. engelska
        private bool IsVowel(char letter)
        {
            return "AEIOUY".Contains(letter);
        }

        //hjälpmetod för engelska bokstäver + att input = 1 karaktär
        private bool IsSingleEnglishLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length != 1)
            {
                return false;
            }

            char letter = char.ToUpperInvariant(input[0]);
            return letter >= 'A' && letter <= 'Z';
        }
    }
}
