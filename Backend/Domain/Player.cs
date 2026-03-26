namespace LetterDuel.Backend.Domain
{
    public class Player
    {
        // Guid används för att ge varje spelare ett unikt id direkt vid skapandet,
        // utan att vara beroende av en databas eller en räknare som genererar löpnummer. 
        public Guid Id { get; init; } = Guid.NewGuid();
        //spelarens namn
        public string Name { get; set; }
        //spelarens nuvarande poäng under nuvarande spel
        public int Score { get; set; }
    }
}
