using LetterDuel.Backend.Domain;
using LetterDuel.Backend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;


namespace LetterDuel.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;

        //Temporär lagring av spel i minnet ersätts senare med en databas
        private static List<Game> _games = new();

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public ActionResult<Game> CreateGame(CreateGameRequest request)
        {
            var player = new Player { Name = request.PlayerName };

            var game = _gameService.CreateGame(request.SecretWord, player);

            _games.Add(game);

            return Ok(game);
        }

        [HttpPost("{gameId}/join")]
        public ActionResult JoinGame(Guid gameId, JoinGameRequest request)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            if (game == null)
            {
                return NotFound("Game not found");
            }
            var player = new Player { Name = request.PlayerName };
            try
            {
                _gameService.AddPlayer(game, player);
                return Ok(game);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{gameId}/guess")] 
        public ActionResult Guess(Guid gameId, GuessRequest request)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);


            if (game == null)
                return NotFound();

            _gameService.GuessLetter(game, request.PlayerId, request.Letter);

            return Ok(game);
        }

        [HttpGet("{gameId}")] 
        public ActionResult<Game> GetGame(Guid gameId)
        {
            var game = _games.FirstOrDefault(g => g.Id == gameId);

            if (game == null)
                return NotFound();

            return Ok(game);
        }   
    }
}
