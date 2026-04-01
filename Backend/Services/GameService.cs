using LetterDuel.Backend.Domain;
using LetterDuel.Backend.DTOs.Responses;
using LetterDuel.Backend.Repositories;

namespace LetterDuel.Backend.Services
{
    // Ansvarar för spelets regler och flöde
    public class GameService
    {
        private readonly IGameRepository _repo;

        public GameService(IGameRepository repo)
        {
            _repo = repo;
        }

        // Skapar nytt spel
        public async Task<CreateGameResponse> CreateGame(string playerName)
        {
            var words = await _repo.GetAllWordsAsync();

            if (words == null || !words.Any())
            {
                throw new InvalidOperationException("No words available");
            }

            var randomWord = words[Random.Shared.Next(words.Count)].Word;

            var game = new Game
            {
                SecretWord = randomWord.ToUpperInvariant(),
                State = GameState.WaitingForPlayers,
                CurrentPlayerIndex = 0
            };

            var player = new Player
            {
                Name = playerName,
                GameId = game.Id
            };

            game.Players.Add(player);

            var savedGame = await _repo.AddAsync(game);

            return new CreateGameResponse
            {
                GameId = savedGame.Id,
                PlayerId = player.Id,
                PlayerName = player.Name
            };
        }

        // Lägger till spelare
            public async Task<JoinGameResponse?> AddPlayer(Guid gameId, string playerName)
        {
            var game = await _repo.GetAsync(gameId);

            if (game == null)
                return null;

            if (game.Players.Count >= 2)
                throw new InvalidOperationException("Game already has two players.");

            var player = new Player
            {
                Name = playerName,
                GameId = game.Id
            };
            await _repo.AddPlayerAsync(player);



            if (game.Players.Count == 2)
                game.State = GameState.InProgress;

            await _repo.SaveChangesAsync();

            return new JoinGameResponse
            {
                GameId = game.Id,
                PlayerId = player.Id,
                PlayerName = player.Name
            };
        }
        

        // Hanterar gissning
        public async Task<Game?> GuessLetter(Guid gameId, Guid playerId, string input)
        {
            var game = await _repo.GetAsync(gameId);

            if (game == null)
                return null;

            if (game.State != GameState.InProgress)
                throw new InvalidOperationException("Game is not in progress");

            if (game.Players[game.CurrentPlayerIndex].Id != playerId)
                throw new InvalidOperationException("It is not this players turn");

            if (!IsSingleEnglishLetter(input))
                throw new InvalidOperationException("Only letters A-Z are allowed");

            char letter = char.ToUpperInvariant(input[0]);

            if (game.GuessedLetters.Contains(letter))
                throw new InvalidOperationException("Letter has already been guessed.");

            game.GuessedLetters += letter;

            if (game.SecretWord.Contains(letter))
            {
                var player = game.Players[game.CurrentPlayerIndex];
                var count = game.SecretWord.Count(c => c == letter);
                var points = IsVowel(letter) ? 2 : 4;

                player.Score += count * points;
            }

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

        // Hämta spel
        public async Task<Game?> GetGame(Guid gameId)
        {
            return await _repo.GetAsync(gameId);
        }

        // Maskerat ord
        public string GetMaskedWord(Game game)
        {
            return new string(
                game.SecretWord
                .Select(letter => game.GuessedLetters.Contains(letter) ? letter : '_')
                .ToArray());
        }

        // Gissade bokstäver
        public List<char> GetGuessedLetters(Game game)
        {
            return game.GuessedLetters
                .OrderBy(letter => letter)
                .ToList();
        }

        // Kolla om hela ordet är gissat
        public bool IsWordFullyGuessed(Game game)
        {
            return game.SecretWord
                .Distinct()
                .All(letter => game.GuessedLetters.Contains(letter));
        }

        // Hämta vinnare
        public Player? GetWinner(Game game)
        {
            if (game.State != GameState.GameFinished)
                return null;

            return game.Players
                .OrderByDescending(p => p.Score)
                .FirstOrDefault();
        }

        // Hjälpmetoder
        private bool IsVowel(char letter)
        {
            return "AEIOUY".Contains(letter);
        }

        private bool IsSingleEnglishLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length != 1)
                return false;

            char letter = char.ToUpperInvariant(input[0]);
            return letter >= 'A' && letter <= 'Z';
        }
    }
}