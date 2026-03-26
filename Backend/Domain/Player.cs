namespace LetterDuel.Backend.Domain
{
    public class Player
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
