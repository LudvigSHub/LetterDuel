using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.DTOs.Responses
{
    public class GameDto
    {
        public Guid Id { get; set; }
        public string MaskedWord { get; set; } = string.Empty;
        public List<PlayerDto> Players { get; set; } = new();
        public string State { get; set; } = "";
        public Guid CurrentPlayerId { get; set; }

        public static GameDto FromGame(Game game)
        {
            return new GameDto
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
                    Score = p.Score,
                }).ToList(),
                State = game.State.ToString(),
                CurrentPlayerId = game.Players[game.CurrentPlayerIndex].Id
            };
        }
    }
}
