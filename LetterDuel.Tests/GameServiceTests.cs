using LetterDuel.Backend.Domain;
using LetterDuel.Backend.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetterDuel.Tests
{
    public class GameServiceTests
    {
        private readonly GameService _service = new();

        [Fact]
        //new game ska ha WaitingForPlayers state 
        //creator ska vara player1 = [0]
        public void CreateGame_SetPlayer1_as_Creator()
        {
            var creator = new Player { Name = "Player1" };
            var game = _service.CreateGame("APPLE", creator);

            Assert.Single(game.Players);
            Assert.Equal(creator.Id, game.Players[0].Id);
            Assert.Equal(GameState.WaitingForPlayers, game.State);
            Assert.Equal(game.Id, creator.GameId);
            Assert.Equal(0, game.CurrentPlayerIndex);
        }

        [Fact]
        //AddPlayer ska lägga till en spelare i spelet
        public void AddPlayer_Should_Add_Player_To_Game()
        {
            var creator = new Player { Name = "Player1" };
            var player2 = new Player { Name = "Player2" };
            var game = _service.CreateGame("APPLE", creator);

            _service.AddPlayer(game, player2);

            Assert.Equal(2, game.Players.Count); //antal spelare sätts till 2
            Assert.Equal(player2.Id, game.Players[1].Id); //player2 id stämmer överens med play[1].Id
            Assert.Equal(game.Id, player2.GameId); //player2 has same gameId as game
            Assert.Equal(GameState.InProgress, game.State); //gamestate ska sättas till InProgress när player2 joinar
        }

        [Fact]
        //efter player2 har joinat ska inga fler spelare kunna ansluta
        public void Max_2_players_allowed()
        {
            var (game, player1, player2) = CreateStartedGame();
            var player3 = new Player { Name = "Player3" };
            var action = () => _service.AddPlayer(game, player3);

            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        //Game måste vara InProgress för att man ska kunna gissa
        public void Game_Has_To_Be_In_Progress_To_Guess()
        {
            var creator = new Player { Name = "Player1" };
            var game = _service.CreateGame("APPLE", creator);
            var action = () => _service.GuessLetter(game, creator.Id, "A");

            Assert.Throws<InvalidOperationException> (action);
        }

        [Fact]
        //Player2 ska inte kunna gissa när det är P1 tur
        public void P2_Cant_Guess_While_P1_Turn()
        {
            var (game, player1, player2) = CreateStartedGame();
            var action = () => _service.GuessLetter(game, player2.Id, "A");

            Assert.Throws<InvalidOperationException>(action);
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
        public void Guess_Restrictionz(string input)
        {
            var (game, player1, player2) = CreateStartedGame();
            var action = () => _service.GuessLetter(game, player1.Id, input);

            Assert.Throws<InvalidOperationException>(action);
        }

        [Theory]
        [InlineData("APPLE", "A", true, 2)]
        [InlineData("BANANA", "B", true, 4)]
        [InlineData("APPLE", "Z", false, 0)]
        [InlineData("LETTER", "T", true, 8)]
        //vokaler ger 2p, kononanter ger 4p, fel 0p
        public void GuessLetter_Should_Save_Letter_And_Update_Score_Correctly
           (string secretWord,
            string input,
            bool shouldExistInWord,
            int expectedScore)
        { 
            var (game, player1, player2) = CreateStartedGame(secretWord); 
            _service.GuessLetter(game,player1.Id, input);

            Assert.Contains(input.ToUpperInvariant()[0], game.GuessedLetters);
            Assert.Equal(shouldExistInWord, game.SecretWord.Contains(input.ToUpperInvariant()[0]));
            Assert.Equal(expectedScore, player1.Score);

        }

        [Fact]
        public void SwitchTurns_After_Guess()
        {
            var (game, player1, player2) = CreateStartedGame();
            _service.GuessLetter(game, player1.Id, "A");

            Assert.Equal(1, game.CurrentPlayerIndex);
        }

        [Theory]
        [InlineData("WORD", "", "____")]
        [InlineData("WORD", "WO", "WO__")]
        [InlineData("WORD", "WORD", "WORD")]
        public void GetMaskedWord_Should_Return_Correct_Masked_Word
        (string secretWord,
        string guessedLetters,
        string expectedMaskedWord)
        {
            var creator = new Player { Name = "Player1" };
            var game = _service.CreateGame(secretWord, creator);

            game.GuessedLetters = guessedLetters;

            var result = _service.GetMaskedWord(game);

            Assert.Equal(expectedMaskedWord, result);
        }




        //hjälpmetod för att skapa ett spel som är InProgress med 2 players
        private (Game game, Player player1, Player player2) CreateStartedGame(string secretWord = "APPLE")
        {
            var player1 = new Player
            {
                Name = "Player 1"
            };

            var player2 = new Player
            {
                Name = "Player 2"
            };

            var game = _service.CreateGame(secretWord, player1);
            _service.AddPlayer(game, player2);

            return (game, player1, player2);
        }
    }
}
