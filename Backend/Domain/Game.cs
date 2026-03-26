namespace LetterDuel.Backend.Domain
{
    public class Game
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string SecretWord { get; }
        public GameState State { get; private set; }
        public List<Player> Players { get; } = new();
        public HashSet<char> GuessedLetters { get; } = new();

        public Game(string secretWord)
        {
            SecretWord = secretWord.ToUpperInvariant();
            State = GameState.WaitingForPlayers;
        }

        public int WordLength => SecretWord.Length;
    }
}
