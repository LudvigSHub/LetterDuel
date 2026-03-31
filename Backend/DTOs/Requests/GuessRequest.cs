namespace LetterDuel.Backend.DTOs.Requests
{
    public class GuessRequest
    {
        public Guid PlayerId { get; set; }
        public string Letter { get; set; } = string.Empty;
    }
}
