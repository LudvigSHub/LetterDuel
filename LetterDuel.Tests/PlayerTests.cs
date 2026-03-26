using LetterDuel.Backend.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetterDuel.Tests
{
    public class PlayerTests
    {
        [Fact]
        //För spelarens score=0
        public void PlayerScore_0()
        {
            var player = new Player();

            Assert.Equal(0, player.Score);
        }

        [Fact]
        //spelarens namn = null
        public void PlayerName_Null()
        {
            var player = new Player();

            Assert.Null(player.Name);
        }

        [Fact]
        //spelarens namn = notnull
        public void PlayerName_NotNull()
        {
            var player = new Player();
            player.Name = "Isaac";

            Assert.NotNull(player.Name);
        }

        [Fact]
        //spelarens id = notnull
        public void PlayerId_NotNull()
        {
            var player = new Player();

            Assert.NotEqual(Guid.Empty, player.Id);
        }

        [Fact]
    }
}
