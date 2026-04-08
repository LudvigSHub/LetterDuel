// Generated from: features\game.feature
import { test } from "playwright-bdd";

test.describe('LetterDuel Game Flow', () => {

  test('Player 1 creates a game', async ({ Given, When, Then, page }) => { 
    await Given('player 1 is on the start page', null, { page }); 
    await When('player 1 clicks "Create Game"', null, { page }); 
    await Then('a Game ID should be displayed', null, { page }); 
  });

  test('Player 2 joins via GAME ID', async ({ Given, When, Then, page }) => { 
    await Given('player 1 has created a game', null, { page }); 
    await When('player 2 enters the GAME ID and clicks "Join Game"', null, { page }); 
    await Then('player 2 should be navigated to the game screen', null, { page }); 
  });

  test('Player 1 starts first', async ({ Given, Then, browser, page }) => { 
    await Given('a game has started with two players', null, { browser, page }); 
    await Then('player 1 should see "Your turn!"', null, { page }); 
  });

  test('Turn switches after a guess', async ({ Given, When, Then, browser, page }) => { 
    await Given('it is player 1s turn', null, { browser, page }); 
    await When('player 1 guesses a letter', null, { page }); 
    await Then('it should be player 2s turn', null, { page }); 
  });

  test('Correct letter is revealed', async ({ Given, When, Then, browser, page }) => { 
    await Given('it is player 1s turn', null, { browser, page }); 
    await When('player 1 guesses a letter that exists in the word', null, { page }); 
    await Then('the letter should be shown in the word', null, { page }); 
  });

  test('Already guessed letter is rejected', async ({ Given, When, Then, browser, page }) => { 
    await Given('the letter "A" has already been guessed', null, { browser, page }); 
    await When('player 1 tries to guess "A" again', null, { page }); 
    await Then('an error message should be displayed', null, { page }); 
  });

  test('Game ends when word is complete', async ({ Given, When, Then, browser, page }) => { 
    await Given('only one letter remains', null, { browser, page }); 
    await When('the correct letter is guessed', null, { page }); 
    await Then('the result page should be displayed', null, { page }); 
  });

  test('Player enters a number as guess', async ({ Given, When, Then, And, browser, page }) => { 
    await Given('a game is in progress', null, { browser, page }); 
    await And('it is the players turn', null, { page }); 
    await When('the player enters "7" as a guess', null, { page }); 
    await Then('a warning should be displayed', null, { page }); 
    await And('the guess should not be accepted', null, { page }); 
    await And('the game state should not change', null, { page }); 
  });

  test('Player enters a special character as guess', async ({ Given, When, Then, And, browser, page }) => { 
    await Given('a game is in progress', null, { browser, page }); 
    await And('it is the players turn', null, { page }); 
    await When('the player enters "@" as a guess', null, { page }); 
    await Then('a warning should be displayed', null, { page }); 
    await And('the guess should not be accepted', null, { page }); 
    await And('the game state should not change', null, { page }); 
  });

});

// == technical section ==

test.use({
  $test: [({}, use) => use(test), { scope: 'test', box: true }],
  $uri: [({}, use) => use('features\\game.feature'), { scope: 'test', box: true }],
  $bddFileData: [({}, use) => use(bddFileData), { scope: "test", box: true }],
});

const bddFileData = [ // bdd-data-start
  {"pwTestLine":6,"pickleLine":3,"tags":[],"steps":[{"pwStepLine":7,"gherkinStepLine":4,"keywordType":"Context","textWithKeyword":"Given player 1 is on the start page","stepMatchArguments":[]},{"pwStepLine":8,"gherkinStepLine":5,"keywordType":"Action","textWithKeyword":"When player 1 clicks \"Create Game\"","stepMatchArguments":[]},{"pwStepLine":9,"gherkinStepLine":6,"keywordType":"Outcome","textWithKeyword":"Then a Game ID should be displayed","stepMatchArguments":[]}]},
  {"pwTestLine":12,"pickleLine":8,"tags":[],"steps":[{"pwStepLine":13,"gherkinStepLine":9,"keywordType":"Context","textWithKeyword":"Given player 1 has created a game","stepMatchArguments":[]},{"pwStepLine":14,"gherkinStepLine":10,"keywordType":"Action","textWithKeyword":"When player 2 enters the GAME ID and clicks \"Join Game\"","stepMatchArguments":[]},{"pwStepLine":15,"gherkinStepLine":11,"keywordType":"Outcome","textWithKeyword":"Then player 2 should be navigated to the game screen","stepMatchArguments":[]}]},
  {"pwTestLine":18,"pickleLine":13,"tags":[],"steps":[{"pwStepLine":19,"gherkinStepLine":14,"keywordType":"Context","textWithKeyword":"Given a game has started with two players","stepMatchArguments":[]},{"pwStepLine":20,"gherkinStepLine":15,"keywordType":"Outcome","textWithKeyword":"Then player 1 should see \"Your turn!\"","stepMatchArguments":[{"group":{"start":7,"value":"1","children":[]},"parameterTypeName":"int"},{"group":{"start":20,"value":"\"Your turn!\"","children":[{"start":21,"value":"Your turn!","children":[{"children":[]}]},{"children":[{"children":[]}]}]},"parameterTypeName":"string"}]}]},
  {"pwTestLine":23,"pickleLine":17,"tags":[],"steps":[{"pwStepLine":24,"gherkinStepLine":18,"keywordType":"Context","textWithKeyword":"Given it is player 1s turn","stepMatchArguments":[]},{"pwStepLine":25,"gherkinStepLine":19,"keywordType":"Action","textWithKeyword":"When player 1 guesses a letter","stepMatchArguments":[{"group":{"start":7,"value":"1","children":[]},"parameterTypeName":"int"}]},{"pwStepLine":26,"gherkinStepLine":20,"keywordType":"Outcome","textWithKeyword":"Then it should be player 2s turn","stepMatchArguments":[]}]},
  {"pwTestLine":29,"pickleLine":22,"tags":[],"steps":[{"pwStepLine":30,"gherkinStepLine":23,"keywordType":"Context","textWithKeyword":"Given it is player 1s turn","stepMatchArguments":[]},{"pwStepLine":31,"gherkinStepLine":24,"keywordType":"Action","textWithKeyword":"When player 1 guesses a letter that exists in the word","stepMatchArguments":[{"group":{"start":7,"value":"1","children":[]},"parameterTypeName":"int"}]},{"pwStepLine":32,"gherkinStepLine":25,"keywordType":"Outcome","textWithKeyword":"Then the letter should be shown in the word","stepMatchArguments":[]}]},
  {"pwTestLine":35,"pickleLine":27,"tags":[],"steps":[{"pwStepLine":36,"gherkinStepLine":28,"keywordType":"Context","textWithKeyword":"Given the letter \"A\" has already been guessed","stepMatchArguments":[]},{"pwStepLine":37,"gherkinStepLine":29,"keywordType":"Action","textWithKeyword":"When player 1 tries to guess \"A\" again","stepMatchArguments":[]},{"pwStepLine":38,"gherkinStepLine":30,"keywordType":"Outcome","textWithKeyword":"Then an error message should be displayed","stepMatchArguments":[]}]},
  {"pwTestLine":41,"pickleLine":32,"tags":[],"steps":[{"pwStepLine":42,"gherkinStepLine":33,"keywordType":"Context","textWithKeyword":"Given only one letter remains","stepMatchArguments":[]},{"pwStepLine":43,"gherkinStepLine":34,"keywordType":"Action","textWithKeyword":"When the correct letter is guessed","stepMatchArguments":[]},{"pwStepLine":44,"gherkinStepLine":35,"keywordType":"Outcome","textWithKeyword":"Then the result page should be displayed","stepMatchArguments":[]}]},
  {"pwTestLine":47,"pickleLine":37,"tags":[],"steps":[{"pwStepLine":48,"gherkinStepLine":38,"keywordType":"Context","textWithKeyword":"Given a game is in progress","stepMatchArguments":[]},{"pwStepLine":49,"gherkinStepLine":39,"keywordType":"Context","textWithKeyword":"And it is the players turn","stepMatchArguments":[]},{"pwStepLine":50,"gherkinStepLine":40,"keywordType":"Action","textWithKeyword":"When the player enters \"7\" as a guess","stepMatchArguments":[]},{"pwStepLine":51,"gherkinStepLine":41,"keywordType":"Outcome","textWithKeyword":"Then a warning should be displayed","stepMatchArguments":[]},{"pwStepLine":52,"gherkinStepLine":42,"keywordType":"Outcome","textWithKeyword":"And the guess should not be accepted","stepMatchArguments":[]},{"pwStepLine":53,"gherkinStepLine":43,"keywordType":"Outcome","textWithKeyword":"And the game state should not change","stepMatchArguments":[]}]},
  {"pwTestLine":56,"pickleLine":45,"tags":[],"steps":[{"pwStepLine":57,"gherkinStepLine":46,"keywordType":"Context","textWithKeyword":"Given a game is in progress","stepMatchArguments":[]},{"pwStepLine":58,"gherkinStepLine":47,"keywordType":"Context","textWithKeyword":"And it is the players turn","stepMatchArguments":[]},{"pwStepLine":59,"gherkinStepLine":48,"keywordType":"Action","textWithKeyword":"When the player enters \"@\" as a guess","stepMatchArguments":[]},{"pwStepLine":60,"gherkinStepLine":49,"keywordType":"Outcome","textWithKeyword":"Then a warning should be displayed","stepMatchArguments":[]},{"pwStepLine":61,"gherkinStepLine":50,"keywordType":"Outcome","textWithKeyword":"And the guess should not be accepted","stepMatchArguments":[]},{"pwStepLine":62,"gherkinStepLine":51,"keywordType":"Outcome","textWithKeyword":"And the game state should not change","stepMatchArguments":[]}]},
]; // bdd-data-end