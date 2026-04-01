using LetterDuel.Backend.Domain;
using LetterDuel.Backend.DTOs.Requests;
using LetterDuel.Backend.DTOs.Responses;
using LetterDuel.Backend.Services;
using Microsoft.AspNetCore.Mvc;


namespace LetterDuel.Backend.Controllers
{
    [ApiController]
    [Route("api/games")]
    [IgnoreAntiforgeryToken]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;
        
        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        //Skapa nytt spel
        [HttpPost]
        public async Task<ActionResult<CreateGameResponse>> CreateGame([FromBody] CreateGameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PlayerName))
                return BadRequest("PlayerName is required");

            var response = await _gameService.CreateGame(request.PlayerName);

            return Ok(response);
        }

        
        //gå med i spel (player2)
        [HttpPost("{gameId}/join")]
        public async Task<ActionResult<JoinGameResponse>> JoinGame(Guid gameId, JoinGameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PlayerName))
                return BadRequest("PlayerName is required");

            try
            {
                var response = await _gameService.AddPlayer(gameId, request.PlayerName);

                if (response == null)
                    return NotFound("Game not found");

                return Ok(response);
            }
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
