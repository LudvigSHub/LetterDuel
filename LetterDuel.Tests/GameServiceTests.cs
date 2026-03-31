using LetterDuel.Backend.Domain;
using LetterDuel.Backend.Services;

namespace LetterDuel.Tests
{
    public class GameServiceTests
    {
        private readonly GameService _service;

        public GameServiceTests()
        {
            var repo = new FakeGameRepository();
            _service = new GameService(repo);
        }

        [Fact]
        //new game ska ha WaitingForPlayers state 
        //creator ska vara player1 = [0]
        public async Task CreateGame_SetPlayer1_as_Creator()
        {
            var game = await _service.CreateGame("APPLE", "Player1");

            Assert.Single(game.Players);
            Assert.Equal("Player1", game.Players[0].Name);
            Assert.Equal(GameState.WaitingForPlayers, game.State);
            Assert.Equal(0, game.CurrentPlayerIndex);
        }

        [Fact]
        //AddPlayer ska lägga till en spelare i spelet
        public async Task AddPlayer_Should_Add_Player_To_Game()
        {
            var game = await _service.CreateGame("APPLE", "Player1");

            var updated = await _service.AddPlayer(game.Id, "Player2");

            Assert.Equal(2, updated!.Players.Count);
            Assert.Equal("Player2", updated.Players[1].Name);
            Assert.Equal(GameState.InProgress, updated.State);
        }

        [Fact]
        //efter player2 har joinat ska inga fler spelare kunna ansluta
        public async Task Max_2_players_allowed()
        {
            var game = await _service.CreateGame("APPLE", "P1");
            await _service.AddPlayer(game.Id, "P2");

            var action = async () => await _service.AddPlayer(game.Id, "P3");

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        //Game måste vara InProgress för att man ska kunna gissa
        public async Task Game_Has_To_Be_In_Progress_To_Guess()
        {
            var game = await _service.CreateGame("APPLE", "P1");

            var action = async () =>
                await _service.GuessLetter(game.Id, game.Players[0].Id, "A");

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        //Player2 ska inte kunna gissa när det är P1 tur
        public async Task P2_Cant_Guess_While_P1_Turn()
        {
            var (game, player1, player2) = await CreateStartedGame();

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
        public async Task Guess_Restrictionz(string input)
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

            Assert.Contains(input.ToUpperInvariant()[0], updated!.GuessedLetters);
            Assert.Equal(shouldExistInWord, updated.SecretWord.Contains(input.ToUpperInvariant()[0]));
            Assert.Equal(expectedScore, updated.Players[0].Score);
        }

        [Fact]
        public async Task SwitchTurns_After_Guess()
        {
            var (game, player1, _) = await CreateStartedGame();

            var updated = await _service.GuessLetter(game.Id, player1.Id, "A");

            Assert.Equal(1, updated!.CurrentPlayerIndex);
        }

        [Theory]
        [InlineData("WORD", "", "____")]
        [InlineData("WORD", "WO", "WO__")]
        [InlineData("WORD", "WORD", "WORD")]
        public async Task GetMaskedWord_Should_Return_Correct_Masked_Word(
            string secretWord,
            string guessedLetters,
            string expectedMaskedWord)
        {
            var game = await _service.CreateGame(secretWord, "P1");

            game.GuessedLetters = guessedLetters;

            var result = _service.GetMaskedWord(game);

            Assert.Equal(expectedMaskedWord, result);
        }

        //hjälpmetod för att skapa ett spel som är InProgress med 2 players
        private async Task<(Game game, Player player1, Player player2)> CreateStartedGame(string secretWord = "APPLE")
        {
            var game = await _service.CreateGame(secretWord, "P1");
            var updated = await _service.AddPlayer(game.Id, "P2");

            return (updated!, updated!.Players[0], updated.Players[1]);
        }

        [Fact]
        //Om bokstav redan gissats
        public async Task If_Letter_Is_Guessed_Already()
        {
            var (game, player1, _) = await CreateStartedGame();

            await _service.GuessLetter(game.Id, player1.Id, "A");

            var action = async () =>
                await _service.GuessLetter(game.Id, player1.Id, "A");

            await Assert.ThrowsAsync<InvalidOperationException>(action);
        }

        [Fact]
        public async Task Game_Ending_Test()
        {
            var (game, player1, _) = await CreateStartedGame("A");

            var updated = await _service.GuessLetter(game.Id, player1.Id, "A");

            Assert.Equal(GameState.GameFinished, updated!.State);
        }

        [Fact]
        //GetWinner ska returnera den spelare som har högst poäng
        public async Task GetWinner_Test()
        {
            var (game, player1, player2) = await CreateStartedGame("AB");

            await _service.GuessLetter(game.Id, player1.Id, "A");
            await _service.GuessLetter(game.Id, player2.Id, "B");

            game.State = GameState.GameFinished;

            var winner = _service.GetWinner(game);

            Assert.Equal(player2.Id, winner!.Id);
        }

        [Fact]
        //test att spelet tar emot stor bokstav oavsett
        public async Task GuessLetter_Should_Be_Case_Insensitive()
        {
            var (game, player1, _) = await CreateStartedGame("APPLE");

            var updated = await _service.GuessLetter(game.Id, player1.Id, "a");

            Assert.Contains('A', updated!.GuessedLetters);
            Assert.Equal(2, updated.Players[0].Score);
        }
    }
}