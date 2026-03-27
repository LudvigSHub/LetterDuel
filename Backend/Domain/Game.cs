namespace LetterDuel.Backend.Domain
{
    public class Game
    {

        public Guid Id { get; init; } = Guid.NewGuid();
        
        public string SecretWord { get; set; } = string.Empty;
        public GameState State { get; set; }
        public List<Player> Players { get; } = new();
        //sparar bokstäver som redan gissats
        public string GuessedLetters { get; set; } = string.Empty;
        public int CurrentPlayerIndex { get; set; }
        public Guid? CurrentTurnPlayerId { get; private set; }

        //hjälp-egenskap för att kunna visa ordets längd i UI
        public int WordLength => SecretWord.Length;
    }
}
