using LetterDuel.Backend.Domain;

namespace LetterDuel.Backend.Services
{
    //ansvarar för spelets regler och flöde
    // här hanteras skapande av spel, spelare, bokstavsgissningar, poäng och vinnare
    public class GameService
    {
        //skapar ett nytt game och ersätter startvärden för spelets state
        public Game CreateGame(string secretWord)
        {
            return new Game
            {
                //ordet sparas i versaler för att förenkla jämförelser av bokstäver
                SecretWord = secretWord.ToUpperInvariant(),
                //ett nytt spel börjar altid med att vänta på spelare
                State = GameState.WaitingForPlayers,
                //första spelaren i listan får första turen när spelet startar
                CurrentPlayerIndex = 0
            };
        }

        //Lägger till en spelare i spelet
        public void AddPlayer(Game game, Player player)
        {
            //spelet får bara innehålla två spelare.
            if (game.Players.Count >= 2)
            {
                throw new InvalidOperationException("Game already has two players.");
            }
            //kopplar spelaren till aktuella spelet
            player.GameId = game.Id;
            //lägger till spelare i listan av deltagare
            game.Players.Add(player);
            // när två spelare har gått med startar spelet
            if (game.Players.Count == 2)
            {
                game.State = GameState.InProgress;
            }
        }

        //hanterar en spelares bokstavsgissning
        public void GuessLetter(Game game, Guid playerId, char letter)
        {
            //det går bara att gissa när spel är igång
            if (game.State != GameState.InProgress)
            {
                throw new InvalidOperationException("Game is not in progress");
            }

            //kontrollera att det är rätt spelares tur
            if (game.Players[game.CurrentPlayerIndex].Id != playerId)
            {
                throw new InvalidOperationException("It is not this players turn");
            }
            //bokstaven görs om till versal för att matcha sparade ordet
            letter = char.ToUpperInvariant(letter);

            //samma bokstav får inte gissas flera gånger
            if (game.GuessedLetters.Contains(letter))
            {
                throw new InvalidOperationException("Letter has already been guessed.");
            }
            //sparar bokstaven som "Guessed"
            game.GuessedLetters.Add(letter);

            //om bokstaven finns i ordet får spelaren poäng
            if (game.SecretWord.Contains(letter))
            {
                var currentPlayer = game.Players[game.CurrentPlayerIndex];
                //vokaler ger 2 poäng, konsonanter ger 4, modellen nedan innehåller alla vokaler
                currentPlayer.Score += IsVowel(letter) ? 2 : 4;
            }

            //om alla bokstaver i ordet är gissade avslutas spelet
            if (IsWordFullyGuessed(game))
            {
                game.State = GameState.GameFinished;
                return;
            }
            //byter tur till nästa spelare
            game.CurrentPlayerIndex = (game.CurrentPlayerIndex + 1) % game.Players.Count;
        }

        //returnerar ordet i maskerad form, där ogissade bokstäver visas som _.
        public string GetMaskedWord(Game game)
        {
            return new string(
                game.SecretWord
                .Select(letter => game.GuessedLetters.Contains(letter) ? letter : '_')
                .ToArray());
        }

        //Kollar om alla unika bokstäver i ordet har blivit gissade
        public bool IsWordFullyGuessed(Game game)
        {
            return game.SecretWord
                .Distinct()
                .All(letter => game.GuessedLetters.Contains(letter));
        }

        //returnerar vinnaren när spelet är färdigt
        public Player? GetWinner(Game game)
        {
            //Ingen vinnare kan utses innan spel är färdigt
            if (game.State != GameState.GameFinished)
            {
                return null;
            }
            //spelaren med högst poäng vinner
            return game.Players
                .OrderByDescending(player => player.Score)
                .FirstOrDefault();
        }

        //hjälpmetod som avngör om bokstav = vokal.. engelska
        private bool IsVowel(char letter)
        {
            return "AEIOUY".Contains(letter);
        }
    }
}
