namespace LetterDuel.Backend.DTOs.Requests
{
    public class CreateGameRequest
    {
        public string SecretWord { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
    }
}
