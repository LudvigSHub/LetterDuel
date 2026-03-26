namespace LetterDuel.Backend.Domain
{
    public class Game
    {

        public Guid Id { get; init; } = Guid.NewGuid();
        
        public string SecretWord { get; }
        public GameState State { get; private set; }
        public List<Player> Players { get; } = new();
        //sparar bokstäver som redan gissats
        public HashSet<char> GuessedLetters { get; } = new();

        public Game(string secretWord)
        {
            SecretWord = secretWord.ToUpperInvariant();
            State = GameState.WaitingForPlayers;
        }

        //hjälp-egenskap för att kunna visa ordets längd i UI
        public int WordLength => SecretWord.Length;
    }
}
