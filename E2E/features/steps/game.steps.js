import { createBdd, test } from "playwright-bdd";
import { expect } from "@playwright/test";

const { Given, When, Then, And } = createBdd(test);

const BASE_URL = "http://localhost:5239";

async function createGame(page) {
  await page.goto(BASE_URL);
  await page.waitForLoadState("networkidle");
  await page.click("text=Create Game");
  // vänta på att Game ID visas
  await page.waitForSelector(".alert-success");
  const gameId = await page.locator(".alert-success strong").textContent();
  return gameId.trim();
}

async function joinGame(page, gameId) {
  await page.goto(BASE_URL);
  await page.waitForLoadState("networkidle");
  await page.fill('input[placeholder="Enter Game ID"]', gameId);
  await page.click("text=Join Game");
  await page.waitForTimeout(3000);
  await page.waitForURL(/\/game\/.+/, { timeout: 20000 });
  await page.screenshot({ path: `debug-join-${Date.now()}.png` });
}

// Player 1 startar spel

Given("player 1 is on the start page", async ({ page }) => {
  await page.goto(BASE_URL);
});

When('player 1 clicks "Create Game"', async ({ page }) => {
  await page.click("text=Create Game");
  await page.waitForTimeout(2000); // vänta på API-svar
});

Then("a Game ID should be displayed", async ({ page }) => {
  await expect(page.locator(".alert-success")).toBeVisible({ timeout: 10000 });
  await expect(page.locator(".alert-success strong")).not.toBeEmpty();
});

/////////////////////////////////////////////////////////////////////////////////

// Player 2 joinar

Given("player 1 has created a game", async ({ page }) => {
  const gameId = await createGame(page);
});

When('player 2 enters the GAME ID and clicks "Join Game"', async ({ page }) => {
  const gameId = await createGame(page);
  await joinGame(page, gameId);
});

Then("player 2 should be navigated to the game screen", async ({ page }) => {
  await expect(page).toHaveURL(/\/game\/.+/);
});

//////////////////////////////////////////////////////////////////////////////

// Player 1 har första turen
Given("a game has started with two players", async ({ browser, page }) => {
  const gameId = await createGame(page);
  await page.click("text=Enter Game as Player 1");
  await page.waitForURL(/\/game\/.+/, { timeout: 10000 });

  const context2 = await browser.newContext();
  const page2 = await context2.newPage();
  await joinGame(page2, gameId);
  page["_page2"] = page2;

  await page.waitForTimeout(3000); // ökat från 2000
  await page.waitForSelector("text=Your turn!", { timeout: 15000 }); // ökat från 10000
});

Then(
  "player {int} should see {string}",
  async ({ page }, playerNumber, text) => {
    await expect(page.locator(`text=${text}`)).toBeVisible();
  },
);

//////////////////////////////////////////////////////////////////////////

// Spelet byter turn
Given("it is player 1s turn", async ({ browser, page }) => {
  const gameId = await createGame(page);
  await page.click("text=Enter Game as Player 1");
  await page.waitForURL(/\/game\/.+/, { timeout: 10000 });

  const context2 = await browser.newContext();
  const page2 = await context2.newPage();
  await joinGame(page2, gameId);

  // Spara både context2 och page2
  page["_context2"] = context2;
  page["_page2"] = page2;

  await page.waitForTimeout(3000);
  await page.waitForSelector("text=Your turn!", { timeout: 15000 });
});

When("player {int} guesses a letter", async ({ page }, playerNumber) => {
  await page.fill('input[placeholder="A-Z"]', "A");
  await page.click("text=Guess!");
  await page.waitForTimeout(1000);
});

Then("it should be player 2s turn", async ({ page }) => {
  const page2 = page["_page2"];
  await page2.waitForTimeout(1000);
  await expect(page2.locator("text=Your turn!")).toBeVisible();
});

/////////////////////////////////////////////////////////////////////////////

// Korrekt gissning
When(
  "player {int} guesses a letter that exists in the word",
  async ({ page }, playerNumber) => {
    // Gissar E pga vanligt ord
    await page.fill('input[placeholder="A-Z"]', "E");
    await page.click("text=Guess!");
    await page.waitForTimeout(1000);
  },
);

Then("the letter should be shown in the word", async ({ page }) => {
  // Kollar specifikt att en gissad bokstav badge visas
  await expect(page.locator(".badge.bg-success.me-1")).toBeVisible();
});

///////////////////////////////////////////////////////////////////////////////

// Gissa på redan gissad bokstav

Given('the letter "A" has already been guessed', async ({ browser, page }) => {
  // Skapa spel och navigera spelare 1 till spelvyn
  const gameId = await createGame(page);
  await page.click("text=Enter Game as Player 1");
  await page.waitForURL(/\/game\/.+/, { timeout: 10000 });

  // Spelare 2 joinar i separat kontext
  const context2 = await browser.newContext();
  const page2 = await context2.newPage();
  await joinGame(page2, gameId);
  page["_page2"] = page2;

  // Vänta på polling så spelet startar
  await page.waitForTimeout(2000);
  await page.waitForSelector("text=Your turn!", { timeout: 10000 });

  // Spelare 1 gissar A
  await page.fill('input[placeholder="A-Z"]', "A");
  await page.click("text=Guess!");
  await page.waitForTimeout(1000);

  // Spelare 2 gissar B så turen går tillbaka till spelare 1
  await page2.waitForSelector("text=Your turn!", { timeout: 10000 });
  await page2.fill('input[placeholder="A-Z"]', "B");
  await page2.click("text=Guess!");
  await page2.waitForTimeout(1000);

  // Vänta på att det är spelare 1s tur igen
  await page.waitForSelector("text=Your turn!", { timeout: 10000 });
});

When('player 1 tries to guess "A" again', async ({ page }) => {
  await page.fill('input[placeholder="A-Z"]', "A");
  await page.click("text=Guess!");
});

Then("an error message should be displayed", async ({ page }) => {
  await expect(page.locator(".alert-danger")).toBeVisible();
});

////////////////////////////////////////////////////////////////////////////////

Given("only one letter remains", async ({ browser, page }) => {
  const gameId = await createGame(page);
  await page.click("text=Enter Game as Player 1");
  await page.waitForURL(/\/game\/.+/, { timeout: 10000 });

  const context2 = await browser.newContext();
  const page2 = await context2.newPage();
  await joinGame(page2, gameId);
  page["_page2"] = page2;

  await page.waitForTimeout(3000);
  await page.waitForSelector("text=Your turn!", { timeout: 15000 });

  // Gissa alla bokstäver i APPLE utom E
  const letters = ["A", "P", "L"];
  for (let i = 0; i < letters.length; i++) {
    const currentPage = i % 2 === 0 ? page : page2;
    await currentPage.waitForSelector("text=Your turn!", { timeout: 10000 });
    await currentPage.fill('input[placeholder="A-Z"]', letters[i]);
    await currentPage.click("text=Guess!");
    await currentPage.waitForTimeout(1000);
  }
});

When("the correct letter is guessed", async ({ page }) => {
  const page2 = page["_page2"];
  // Det är spelare 2s tur att gissa E
  await page2.waitForSelector("text=Your turn!", { timeout: 10000 });
  await page2.fill('input[placeholder="A-Z"]', "E");
  await page2.click("text=Guess!");
  await page2.waitForTimeout(3000);
});

Then("the result page should be displayed", async ({ page }) => {
  const page2 = page["_page2"];
  await expect(page2).toHaveURL(/\/game\/.+\/result/, { timeout: 15000 });
});

Given("a game is in progress", async ({ browser, page }) => {
  const gameId = await createGame(page);
  await page.click("text=Enter Game as Player 1");
  await page.waitForURL(/\/game\/.+/, { timeout: 10000 });

  const context2 = await browser.newContext();
  const page2 = await context2.newPage();
  await joinGame(page2, gameId);

  page["_context2"] = context2;
  page["_page2"] = page2;

  await page.waitForTimeout(3000);
  await page.waitForSelector("text=Your turn!", { timeout: 15000 });
});

Given("it is the players turn", async ({ page }) => {
  await expect(page.locator('input[placeholder="A-Z"]')).toBeVisible();
});

When('the player enters "7" as a guess', async ({ page }) => {
  await page.fill('input[placeholder="A-Z"]', "7");
  await page.click("text=Guess!");
  await page.waitForTimeout(2000); // vänta på API-svar
});

When('the player enters "@" as a guess', async ({ page }) => {
  await page.fill('input[placeholder="A-Z"]', "@");
  await page.click("text=Guess!");
  await page.waitForTimeout(2000);
});

// When('the player enters "AB" as a guess', async ({ page }) => {
//   await page.fill('input[placeholder="A-Z"]', "AB");
//   await page.click("text=Guess!");
//   await page.waitForTimeout(2000);
// });

Then("a warning should be displayed", async ({ page }) => {
  await expect(page.locator(".alert-danger")).toBeVisible({ timeout: 10000 });
});

Then("the guess should not be accepted", async ({ page }) => {
  await expect(page.locator(".alert-danger")).toContainText("Only letters");
});

Then("the game state should not change", async ({ page }) => {
  // Kontrollera att det fortfarande är spelarens tur
  await expect(page.locator('input[placeholder="A-Z"]')).toBeVisible();
});
