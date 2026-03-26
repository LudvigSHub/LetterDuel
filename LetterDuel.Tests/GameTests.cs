using LetterDuel.Backend.Domain;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetterDuel.Tests
{
    public class GameTests
    {
        [Fact]
        public void NewGameWaitingForPlayers()
        {
            var game = new Game("apple");

            Assert.Equal(GameState.WaitingForPlayers, game.State);

        }

        [Fact]
        public void NewGame_WordInUpperCase()
        {
            var game = new Game("apple");

            Assert.Equal("APPLE", game.SecretWord);
        }

        [Fact] 
        public void NewGame_WordLength()
        {
            var game = new Game("apple");

            Assert.Equal(5, game.WordLength);
        }

        [Fact]
        public void NewGame_StartWithNoPlayers()
        {
            var game = new Game("apple");

            Assert.Empty(game.Players);
        }

        [Fact]
        public void NewGame_StartWithNoGuessedLetters()
        {
            var game = new Game("apple");

            Assert.Empty(game.GuessedLetters);
        }
    }
}
