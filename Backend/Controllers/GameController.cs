using LetterDuel.Backend.Domain;
using LetterDuel.Backend.DTOs.Requests;
using LetterDuel.Backend.DTOs.Responses;
using LetterDuel.Backend.Services;
using Microsoft.AspNetCore.Mvc;


namespace LetterDuel.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;
        
        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        //Skapa nytt spel
        [HttpPost]
        public async Task<ActionResult<GameDto>> CreateGame(CreateGameRequest request)
        {
            //anropar service för att skapa spelet
            var game = await _gameService.CreateGame(
                request.SecretWord, 
                request.PlayerName
                );
            //lägger till spel i minnet
                return Ok(MapToDto(game));
        }

        //gå med i spel (player2)
        [HttpPost("{gameId}/join")]
        public async Task<ActionResult<GameDto>> JoinGame(Guid gameId, JoinGameRequest request)
        {

            try
            {
                var game = await _gameService.AddPlayer(gameId, request.PlayerName);

                if (game == null)
                    return NotFound("Game not found");

                return Ok(MapToDto(game));
            }
            //om spel tex har 2 spelare redan
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //gissning av bokstav
        [HttpPost("{gameId}/guess")] 
        public async Task<ActionResult<GameDto>> Guess(Guid gameId, GuessRequest request)
        {
            try
            {
                var game = await _gameService.GuessLetter(
                    gameId,
                    request.PlayerId,
                    request.Letter
                    );
                
                if (game == null)
                    return NotFound("Game not found");

                return Ok(MapToDto(game));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //hämtar spelstatus
        [HttpGet("{gameId}")] 
        public async Task<ActionResult<GameDto>> GetGame(Guid gameId)
        {
            //hämtar spel
            var game = await _gameService.GetGame(gameId);

            if (game == null)
                return NotFound("Game not found");

            return Ok(MapToDto(game));
        }

        // DTO mapping
        private GameDto MapToDto(Domain.Game game)
        {
            return new GameDto
            {
                Id = game.Id,
                MaskedWord = _gameService.GetMaskedWord(game),
                Players = game.Players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Score = p.Score
                }).ToList(),
                State = game.State.ToString(),
                CurrentPlayerId = game.Players[game.CurrentPlayerIndex].Id,
                GuessedLetters = _gameService.GetGuessedLetters(game),
                WinnerId = _gameService.GetWinner(game)?.Id
            };
        }
    }
}
