using LetterDuel.Backend.Domain;
using LetterDuel.Backend.Services;

namespace LetterDuel.Tests
{
    public class GameServiceTests
    {
        private readonly FakeGameRepository _repo;
        private readonly GameService _service;

        public GameServiceTests()
        {
            _repo = new FakeGameRepository();
            _service = new GameService(_repo);
        }

        [Fact]
        // new game ska ha WaitingForPlayers state
        //creator ska vara player1 = [0]
        public async Task CreateGame_SetPlayer1_As_Creator()
        {
            SetSingleWord("APPLE");

            var response = await _service.CreateGame("Player1");
            var game = await _repo.GetAsync(response.GameId);

            Assert.NotNull(game);
            Assert.Single(game!.Players);
            Assert.Equal("Player1", game.Players[0].Name);
            Assert.Equal(GameState.WaitingForPlayers, game.State);
            Assert.Equal(0, game.CurrentPlayerIndex);
            Assert.Equal("APPLE", game.SecretWord);
        }

        [Fact]
        // Om inga ord finns i repositoryt så ska det inte gå att skapa ett spel
        public async Task CreateGame_Should_Throw_When_No_Words_Available()
        {
            _repo.Words.Clear();

            var action = async () => await _service.CreateGame("Player1");

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        //AddPlayer ska lägga till en spelare i spelet
        public async Task AddPlayer_Should_Add_Player_To_Game()
        {
            SetSingleWord("APPLE");

            var createResponse = await _service.CreateGame("Player1");
            var joinResponse = await _service.AddPlayer(createResponse.GameId, "Player2");
            var game = await _repo.GetAsync(createResponse.GameId);

            Assert.NotNull(joinResponse);
            Assert.NotNull(game);
            Assert.Equal(2, game!.Players.Count);
            Assert.Equal("Player2", game.Players[1].Name);
            Assert.Equal(GameState.InProgress, game.State);
        }

        [Fact]
        // Om spelet inte finns så ska AddPlayer returnera null
        public async Task AddPlayer_Should_Return_Null_If_Game_Not_Found()
        {
            var response = await _service.AddPlayer(Guid.NewGuid(), "Player2");

            Assert.Null(response);
        }

        [Fact]
        //efter player2 har joinat ska inga fler spelare kunna ansluta
        public async Task Max_2_Players_Allowed()
        {
            SetSingleWord("APPLE");

            var createResponse = await _service.CreateGame("P1");
            await _service.AddPlayer(createResponse.GameId, "P2");

            var action = async () => await _service.AddPlayer(createResponse.GameId, "P3");

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        //Game måste vara InProgress för att man ska kunna gissa
        public async Task Game_Has_To_Be_In_Progress_To_Guess()
        {
            SetSingleWord("APPLE");

            var createResponse = await _service.CreateGame("P1");
            var game = await _repo.GetAsync(createResponse.GameId);

            Assert.NotNull(game);

            var action = async () =>
                await _service.GuessLetter(game!.Id, game.Players[0].Id, "A");

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        //Player2 ska inte kunna gissa när det är P1 tur
        public async Task P2_Cant_Guess_While_P1_Turn()
        {
            var (game, _, player2) = await CreateStartedGame();

            var action = async () =>
                await _service.GuessLetter(game.Id, player2.Id, "A");

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Theory]
        [InlineData("")]
        [InlineData("AB")]
        [InlineData("1")]
        [InlineData("@")]
        [InlineData("Å")]
        [InlineData("Ä")]
        [InlineData("Ö")]
        //restrictions, input får ej vara special chars
        public async Task Guess_Restrictions(string input)
        {
            var (game, player1, _) = await CreateStartedGame();

            var action = async () =>
                await _service.GuessLetter(game.Id, player1.Id, input);

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Theory]
        [InlineData("APPLE", "A", true, 2)]
        [InlineData("BANANA", "B", true, 4)]
        [InlineData("APPLE", "Z", false, 0)]
        [InlineData("LETTER", "T", true, 8)]
        //vokaler ger 2p, kononanter ger 4p, fel 0p
        public async Task GuessLetter_Should_Save_Letter_And_Update_Score_Correctly(
            string secretWord,
            string input,
            bool shouldExistInWord,
            int expectedScore)
        {
            var (game, player1, _) = await CreateStartedGame(secretWord);

            var updated = await _service.GuessLetter(game.Id, player1.Id, input);

            Assert.NotNull(updated);
            Assert.Contains(input.ToUpperInvariant()[0], updated!.GuessedLetters);
            Assert.Equal(shouldExistInWord, updated.SecretWord.Contains(input.ToUpperInvariant()[0]));
            Assert.Equal(expectedScore, updated.Players[0].Score);
        }

        [Fact]
        public async Task SwitchTurns_After_Guess()
        {
            var (game, player1, _) = await CreateStartedGame();

            var updated = await _service.GuessLetter(game.Id, player1.Id, "A");

            Assert.NotNull(updated);
            Assert.Equal(1, updated!.CurrentPlayerIndex);
        }

        [Theory]
        [InlineData("WORD", "", "____")]
        [InlineData("WORD", "WO", "WO__")]
        [InlineData("WORD", "WORD", "WORD")]
        public void GetMaskedWord_Should_Return_Correct_Masked_Word(
            string secretWord,
            string guessedLetters,
            string expectedMaskedWord)
        {
            var game = new Game
            {
                SecretWord = secretWord,
                GuessedLetters = guessedLetters
            };

            var result = _service.GetMaskedWord(game);

            Assert.Equal(expectedMaskedWord, result);
        }

        [Fact]
        public async Task If_Letter_Is_Guessed_Already()
        {
            var (game, player1, player2) = await CreateStartedGame();

            await _service.GuessLetter(game.Id, player1.Id, "A");

            var action = async () =>
                await _service.GuessLetter(game.Id, player2.Id, "A");

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        public async Task Game_Ending_Test()
        {
            var (game, player1, _) = await CreateStartedGame("A");

            var updated = await _service.GuessLetter(game.Id, player1.Id, "A");

            Assert.NotNull(updated);
            Assert.Equal(GameState.GameFinished, updated!.State);
        }

        [Fact]
        public async Task GetWinner_Test()
        {
            var (game, player1, player2) = await CreateStartedGame("AB");

            await _service.GuessLetter(game.Id, player1.Id, "A");
            var updated = await _service.GuessLetter(game.Id, player2.Id, "B");

            Assert.NotNull(updated);

            var winner = _service.GetWinner(updated!);

            Assert.NotNull(winner);
            Assert.Equal(player2.Id, winner!.Id);
        }

        [Fact]
        public async Task GuessLetter_Should_Be_Case_Insensitive()
        {
            var (game, player1, _) = await CreateStartedGame("APPLE");

            var updated = await _service.GuessLetter(game.Id, player1.Id, "a");

            Assert.NotNull(updated);
            Assert.Contains('A', updated!.GuessedLetters);
            Assert.Equal(2, updated.Players[0].Score);
        }

        [Fact]
        public async Task GetGuessedLetters_Should_Return_Sorted_List()
        {
            var game = new Game
            {
                SecretWord = "APPLE",
                GuessedLetters = "PAE"
            };

            var result = _service.GetGuessedLetters(game);

            Assert.Equal(new List<char> { 'A', 'E', 'P' }, result);
        }

        [Fact]
        public void IsWordFullyGuessed_Should_Return_True_When_All_Letters_Are_Guessed()
        {
            var game = new Game
            {
                SecretWord = "APPLE",
                GuessedLetters = "APLE"
            };

            var result = _service.IsWordFullyGuessed(game);

            Assert.True(result);
        }

        [Fact]
        public void GetWinner_Should_Return_Null_If_Game_Is_Not_Finished()
        {
            var game = new Game
            {
                State = GameState.InProgress
            };

            var winner = _service.GetWinner(game);

            Assert.Null(winner);
        }

        //Om lika ska winner vara null
        [Fact]
        public void GetWinner_Should_Return_Null_If_Draw()
        {
            var game = new Game
            {
                State = GameState.GameFinished
            };

            game.Players.Add(new Player { Score = 10 });
            game.Players.Add(new Player { Score = 10 });

            var winner = _service.GetWinner(game);

            Assert.Null(winner);
        }

        private void SetSingleWord(string word)
        {
            _repo.Words.Clear();
            _repo.Words.Add(new GameWord
            {
                Id = 1,
                Word = word
            });
        }

        private async Task<(Game game, Player player1, Player player2)> CreateStartedGame(string secretWord = "APPLE")
        {
            SetSingleWord(secretWord);

            var createResponse = await _service.CreateGame("P1");
            await _service.AddPlayer(createResponse.GameId, "P2");

            var game = await _repo.GetAsync(createResponse.GameId);

            Assert.NotNull(game);
            return (game!, game!.Players[0], game.Players[1]);
        }
    }
}