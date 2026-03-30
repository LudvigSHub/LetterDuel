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
        public void CreateGame_SetPlayer1_as_Creator()
        {
            var creator = new Player { Name = "Player1" };
            var game = _service.CreateGame("APPLE", creator);

            Assert.Single(game.Players);
            Assert.Equal(creator.Id, game.Players[0].Id);
            Assert.Equal(GameState.WaitingForPlayers, game.State);
            Assert.Equal(game.Id, creator.Game.Id);
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
            Assert.Equal(game.Id, creator.Game.Id);
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
