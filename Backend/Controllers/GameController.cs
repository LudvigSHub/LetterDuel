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

        //Temporär lagring av spel i minnet ersätts senare med en databas
        private static List<Game> _games = new();

        //Dependency injection av GameService som hanterar spelets logik och regler
        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        //Skapa nytt spel
        [HttpPost]
        public ActionResult<Game> CreateGame(CreateGameRequest request)
        {
            //anropar service för att skapa spelet
            var game = _gameService.CreateGame(
                request.SecretWord, 
                request.PlayerName
                );
            //lägger till spel i minnet
            _games.Add(game);

            return Ok(GameDto.FromGame(game));
        }

        //gå med i spel (player2)
        [HttpPost("{gameId}/join")]
        public ActionResult JoinGame(Guid gameId, JoinGameRequest request)
        {
            //hämtar spel från databas
            var game = _games.FirstOrDefault(g => g.Id == gameId);
            
            if (game == null)
            {
                return NotFound("Game not found");
            }

            try
            {
                //lägger till player2 i spelet
                _gameService.AddPlayer(game, request.PlayerName);
            }
            //om spel tex har 2 spelare redan
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(GameDto.FromGame(game));
        }

        //gissning av bokstav
        [HttpPost("{gameId}/guess")] 
        public ActionResult Guess(Guid gameId, GuessRequest request)
        {
            //hämtar spel
            var game = _games.FirstOrDefault(g => g.Id == gameId);


            if (game == null)
                return NotFound("Game not found");
            //spelservice hanterar gissning av bokstav och uppdaterar spelets state
            try
            {
                _gameService.GuessLetter(game, request.PlayerId, request.Letter);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(GameDto.FromGame(game));
        }

        //hämtar spelstatus
        [HttpGet("{gameId}")] 
        public ActionResult<GameDto> GetGame(Guid gameId)
        {
            //hämtar spel
            var game = _games.FirstOrDefault(g => g.Id == gameId);

            if (game == null)
                return NotFound("Game not found");

            //returnerar spelet med aktuell status
            return Ok(GameDto.FromGame(game));
        }   
    }
}
