using LetterDuel.Backend.Domain;
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
    }
}
