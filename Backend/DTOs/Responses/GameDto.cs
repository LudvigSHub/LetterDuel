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
    }
}
