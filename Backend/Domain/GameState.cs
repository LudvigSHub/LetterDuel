namespace LetterDuel.Backend.Domain
{
    //enum för spelets olika states
    //för att styra spelregler, turordning och när spelet är klart
    public enum GameState
    {
        //spel skapat men väntar på spelare
        WaitingForPlayers,
        //spelet startat och spelarna turas om
        InProgress,
        //en spelare har gissat och  turen går vidare
        TurnCompleted,
        //spel färdigt och vinnare utses
        GameFinished
    }
}
