using LetterDuel.Backend.Domain;
using LetterDuel.Backend.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetterDuel.Tests
{
    public class GameServiceTests
    {
    //     private readonly GameService _service = new();

    //     [Fact]
    //     //new game ska ha WaitingForPlayers state 
    //    public void CreateGame_WaitingForPlayersState()
    //     {
    //         var game = _service.CreateGame("APPLE");

    //         Assert.Equal(GameState.WaitingForPlayers, game.State);
    //     }

    //     [Fact]
    //     //CurrentPlayerIndex ska vara 0 när spelet startar (Player 1 börjar)
    //     public void CurrentPlayerIndex_Starts0()
    //     {
    //         var game = _service.CreateGame("APPLE");

    //         Assert.Equal(0, game.CurrentPlayerIndex);
    //     }

    //     [Fact]
    //     //AddPlayer ska lägga till en spelare i spelet
    //     public void AddPlayer_Should_Add_Player_To_Game()
    //     {
    //         var game = _service.CreateGame("APPLE");
    //         var player = new Player { Name = "Angelos" };

    //         _service.AddPlayer(game, player);

    //         Assert.Single(game.Players);
    //         Assert.Equal("Angelos", game.Players[0].Name);
    //         Assert.Equal(game.Id, game.Players[0].GameId);
    //     }

    //     [Fact]
    //     //state ska sättas till InProgress när andra spelaren joinar
    //     public void AddPlayer_Should_Set_State_To_InProgress_When_Second_Player_Joins()
    //     {
    //         var game = _service.CreateGame("APPLE");

    //         _service.AddPlayer(game, new Player { Name = "Angelos" });
    //         _service.AddPlayer(game, new Player { Name = "Isaac" });

    //         Assert.Equal(2, game.Players.Count);
    //         Assert.Equal(GameState.InProgress, game.State);
    //     }

    }
}
