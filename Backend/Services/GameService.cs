using LetterDuel.Backend.Domain;
using Microsoft.AspNetCore.Authorization.Infrastructure;

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
            //kontroll om det är en engelsk bokstav
            if (!IsEnglishLetter(letter))
            {
                throw new InvalidOperationException("Only letters in the English alphabet A-Z");
            }
            //Om bokstaven redan gissats.
            if (game.GuessedLetters.Contains(letter))
            {
                throw new InvalidOperationException("Letter has already been guessed.");
            }
            
            game.GuessedLetters += letter;
            //om gissning är rätt
            if (game.SecretWord.Contains(letter))
            {
                var currentplayer = game.Players[game.CurrentPlayerIndex];
                currentplayer.Score += IsVowel(letter) ? 2 : 4;
            }
            //om alla bokstäver är gissade, avsluta Game
            if (IsWordFullyGuessed(Game))
            {
                game.State = GameState.GameFinished;
                return;
            }
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

        //hjälpmetod för engelska bokstäver
        private bool IsEnglishLetter(char letter)
        {
            return letter >= 'A' && letter <= 'Z';
        }
    }
}
