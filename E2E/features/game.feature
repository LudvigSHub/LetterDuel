Feature: LetterDuel Game Flow

Scenario: Player 1 creates a game
    Given player 1 is on the start page
    When player 1 clicks "Create Game"
    Then a Game ID should be displayed

Scenario: Player 2 joins via GAME ID
    Given player 1 has created a game
    When player 2 enters the GAME ID and clicks "Join Game"
    Then player 2 should be navigated to the game screen 

Scenario: Player 1 starts first
    Given a game has started with two players
    Then player 1 should see "Your turn!"

Scenario: Turn switches after a guess
    Given it is player 1s turn
    When player 1 guesses a letter
    Then it should be player 2s turn

Scenario: Correct letter is revealed
    Given it is player 1s turn
    When player 1 guesses a letter that exists in the word
    Then the letter should be shown in the word

Scenario: Already guessed letter is rejected
    Given the letter "A" has already been guessed
    When player 1 tries to guess "A" again
    Then an error message should be displayed

 Scenario: Game ends when word is complete
    Given only one letter remains
    When the correct letter is guessed
    Then the result page should be displayed

Scenario: Player enters a number as guess
    Given a game is in progress
    And it is the players turn
    When the player enters "7" as a guess
    Then a warning should be displayed
    And the guess should not be accepted
    And the game state should not change

Scenario: Player enters a special character as guess
    Given a game is in progress
    And it is the players turn
    When the player enters "@" as a guess
    Then a warning should be displayed
    And the guess should not be accepted
    And the game state should not change

