namespace LetterDuel.Backend.DTOs.Responses
{
    public class CreateGameResponse
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int PlayerNumber { get; set; }
    }
}
