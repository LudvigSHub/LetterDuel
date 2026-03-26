namespace LetterDuel.Backend.Domain
{
    public class Game
    {

        public Guid Id { get; init; } = Guid.NewGuid();
        
        public string SecretWord { get; set; } = string.Empty;
        public GameState State { get; set; }
        public List<Player> Players { get; } = new();
        //sparar bokstäver som redan gissats
        public HashSet<char> GuessedLetters { get; } = new();
        public int CurrentPlayerIndex { get; set; }

        //hjälp-egenskap för att kunna visa ordets längd i UI
        public int WordLength => SecretWord.Length;
    }
}
