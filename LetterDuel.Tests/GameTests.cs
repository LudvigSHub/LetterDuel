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
        //Game får unikt id
        public void NewGame_GetsUniqueId()
        {
            var game = new Game("apple");

            Assert.NotEqual(Guid.Empty, game.Id);
        }

        [Fact]
        //game väntar på spelare
        public void NewGameWaitingForPlayers()
        {
            var game = new Game("apple");

            Assert.Equal(GameState.WaitingForPlayers, game.State);

        }

        [Fact]
        //game gör ordet till stora bokstäver
        public void NewGame_WordInUpperCase()
        {
            var game = new Game("apple");

            Assert.Equal("APPLE", game.SecretWord);
        }

        [Fact]
        //game ordets längd i bokstäver
        public void NewGame_WordLength()
        {
            var game = new Game("apple");

            Assert.Equal(5, game.WordLength);
        }

        [Fact]
        //game startar med inga spelare
        public void NewGame_StartWithNoPlayers()
        {
            var game = new Game("apple");

            Assert.Empty(game.Players);
        }

        [Fact]
        //game startar med inga gissade bokstäver
        public void NewGame_StartWithNoGuessedLetters()
        {
            var game = new Game("apple");

            Assert.Empty(game.GuessedLetters);
        }
    }
}
