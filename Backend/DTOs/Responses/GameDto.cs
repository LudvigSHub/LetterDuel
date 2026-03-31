using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.DTOs.Responses
{
    public class GameDto
    {
        //spelets id
        public Guid Id { get; set; }
        public string MaskedWord { get; set; } = string.Empty;
        public List<PlayerDto> Players { get; set; } = new();
        public string State { get; set; } = "";
        //id på spelarens vars tur det är
        public List<char> GuessedLetters { get; set; } = new();
        public Guid? WinnerId { get; set; }
        public Guid CurrentPlayerId { get; set; }

        public static GameDto FromGame(Game game)
        {
            return new GameDto
            {
                //Kopierar spelets id
                Id = game.Id,
                //skapar en maskerad version av ordet
                MaskedWord = new string(
                    game.SecretWord.Select(letter =>
                        game.GuessedLetters.Contains(letter) ? letter : '_'
                    ).ToArray()
                ),
                //mappar varje player med playerdto
                Players = game.Players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Score = p.Score,
                }).ToList(),
                //hämtar id för spelarens tur det är
                //UI använder detta för att avgöra vem som får spela
                State = game.State.ToString(),
                CurrentPlayerId = game.Players[game.CurrentPlayerIndex].Id,

                GuessedLetters = game.GuessedLetters
                    .OrderBy(l => l)
                    .ToList(),
 
                WinnerId = game.State == GameState.GameFinished
                    ? game.Players
                    .OrderByDescending(p => p.Score)
                    .FirstOrDefault()?.Id
                    : null
            };
        }
    }
}
