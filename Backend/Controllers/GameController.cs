using LetterDuel.Backend.Domain;
using LetterDuel.Backend.DTOs.Requests;
using LetterDuel.Backend.DTOs.Responses;
using LetterDuel.Backend.Repositories;
using LetterDuel.Backend.Services;
using Microsoft.AspNetCore.Mvc;


namespace LetterDuel.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly GameService _gameService;
        private readonly IGameRepository _gameRepository;

        //Temporär lagring av spel i minnet ersätts senare med en databas
        //private static List<Game> _games = new();

        //Dependency injection av GameService som hanterar spelets logik och regler
        public GameController(GameService gameService, IGameRepository gameRepository)
        {
            _gameService = gameService;
            _gameRepository = gameRepository;
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
            _gameRepository.Save(game);

            return Ok(new GameDto
            {
                Id = game.Id,
                MaskedWord = new string(
                game.SecretWord.Select(letter =>
                    game.GuessedLetters.Contains(letter) ? letter : '_'
                ).ToArray()
            ),
                Players = game.Players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Score = p.Score
                }).ToList(),
                State = game.State.ToString(),
                CurrentPlayerId = game.Players[game.CurrentPlayerIndex].Id,
                GuessedLetters = game.GuessedLetters.OrderBy(l => l).ToList(),
                WinnerId = game.State == GameState.GameFinished
                ? game.Players.OrderByDescending(p => p.Score).First().Id
                : null
                    });
        }

        //gå med i spel (player2)
        [HttpPost("{gameId}/join")]
        public ActionResult JoinGame(Guid gameId, JoinGameRequest request)
        {
            //hämtar spel från databas
            var game = _gameRepository.GetById(gameId);

            if (game == null)
            {
                return NotFound("Game not found");
            }

            try
            {
                //lägger till player2 i spelet
                _gameService.AddPlayer(game, request.PlayerName);
                //sparar uppdaterat spel i db
                _gameRepository.Save(game);
            }
            //om spel tex har 2 spelare redan
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new GameDto
            {
                Id = game.Id,
                MaskedWord = new string(
                game.SecretWord.Select(letter =>
                game.GuessedLetters.Contains(letter) ? letter : '_'
                ).ToArray()
                ),
                Players = game.Players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Score = p.Score
                }).ToList(),
                State = game.State.ToString(),
                CurrentPlayerId = game.Players[game.CurrentPlayerIndex].Id,
                GuessedLetters = game.GuessedLetters.OrderBy(l => l).ToList(),
                WinnerId = game.State == GameState.GameFinished
                ? game.Players.OrderByDescending(p => p.Score).First().Id
                : null
                    });
            }

        //gissning av bokstav
        [HttpPost("{gameId}/guess")] 
        public ActionResult Guess(Guid gameId, GuessRequest request)
        {
            //hämtar spel
            var game = _gameRepository.GetById(gameId);


            if (game == null)
                return NotFound("Game not found");
            //spelservice hanterar gissning av bokstav och uppdaterar spelets state
            try
            {
                _gameService.GuessLetter(game, request.PlayerId, request.Letter);
                _gameRepository.Save(game); //sparar game i db
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new GameDto
            {
                Id = game.Id,
                MaskedWord = new string(
                game.SecretWord.Select(letter =>
                game.GuessedLetters.Contains(letter) ? letter : '_'
                ).ToArray()
                ),
                Players = game.Players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Score = p.Score
                }).ToList(),
                State = game.State.ToString(),
                CurrentPlayerId = game.Players[game.CurrentPlayerIndex].Id,
                GuessedLetters = game.GuessedLetters.OrderBy(l => l).ToList(),
                WinnerId = game.State == GameState.GameFinished
                ? game.Players.OrderByDescending(p => p.Score).First().Id
                : null
                });
        }

        //hämtar spelstatus
        [HttpGet("{gameId}")] 
        public ActionResult<GameDto> GetGame(Guid gameId)
        {
            //hämtar spel
            var game = _gameRepository.GetById(gameId);

            if (game == null)
                return NotFound("Game not found");

            //returnerar spelet med aktuell status
            return Ok(new GameDto
            {
                Id = game.Id,
                MaskedWord = new string(
                game.SecretWord.Select(letter =>
                    game.GuessedLetters.Contains(letter) ? letter : '_'
                ).ToArray()
                ),
                Players = game.Players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Score = p.Score
                }).ToList(),
                State = game.State.ToString(),
                CurrentPlayerId = game.Players[game.CurrentPlayerIndex].Id,
                GuessedLetters = game.GuessedLetters.OrderBy(l => l).ToList(),
                WinnerId = game.State == GameState.GameFinished
                ? game.Players.OrderByDescending(p => p.Score).First().Id
                : null
            });
        }   
    }
}
