namespace LetterDuel.Backend.Domain
{
    public class GameWord
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
    }
}

//vi seedar en GameWord lista i vår db som service kan slumpa ett ord ifrån.