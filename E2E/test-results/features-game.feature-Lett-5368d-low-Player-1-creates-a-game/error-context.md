# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: features\game.feature.spec.js >> LetterDuel Game Flow >> Player 1 creates a game
- Location: .features-gen\features\game.feature.spec.js:6:3

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: locator('.alert-success')
Expected: visible
Timeout: 10000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" with timeout 10000ms
  - waiting for locator('.alert-success')

```

# Page snapshot

```yaml
- generic [ref=e2]:
  - generic [ref=e3]:
    - link "⚔️ LetterDuel" [ref=e6] [cursor=pointer]:
      - /url: ""
    - navigation [ref=e8]:
      - link "Home" [ref=e10] [cursor=pointer]:
        - /url: ""
        - text: Home
  - main [ref=e12]:
    - link "About" [ref=e14] [cursor=pointer]:
      - /url: https://learn.microsoft.com/aspnet/core/
    - article [ref=e15]:
      - generic [ref=e16]:
        - heading "⚔️ LetterDuel" [level=1] [ref=e17]
        - generic [ref=e19]:
          - button "Create Game" [active] [ref=e20] [cursor=pointer]
          - separator [ref=e21]
          - textbox "Enter Game ID" [ref=e22]
          - button "Join Game" [ref=e23] [cursor=pointer]
```

# Test source

```ts
  1   | import { createBdd, test } from "playwright-bdd";
  2   | import { expect } from "@playwright/test";
  3   | 
  4   | const { Given, When, Then, And } = createBdd(test);
  5   | 
  6   | const BASE_URL = "http://localhost:5239";
  7   | 
  8   | async function createGame(page) {
  9   |   await page.goto(BASE_URL);
  10  |   await page.waitForLoadState("networkidle");
  11  |   await page.click("text=Create Game");
  12  |   // vänta på att Game ID visas
  13  |   await page.waitForSelector(".alert-success");
  14  |   const gameId = await page.locator(".alert-success strong").textContent();
  15  |   return gameId.trim();
  16  | }
  17  | 
  18  | async function joinGame(page, gameId) {
  19  |   await page.goto(BASE_URL);
  20  |   await page.waitForLoadState("networkidle");
  21  |   await page.fill('input[placeholder="Enter Game ID"]', gameId);
  22  |   await page.click("text=Join Game");
  23  |   await page.waitForTimeout(3000);
  24  |   await page.waitForURL(/\/game\/.+/, { timeout: 20000 });
  25  |   await page.screenshot({ path: `debug-join-${Date.now()}.png` });
  26  | }
  27  | 
  28  | // Player 1 startar spel
  29  | 
  30  | Given("player 1 is on the start page", async ({ page }) => {
  31  |   await page.goto(BASE_URL);
  32  |   await page.waitForLoadState("networkidle");
  33  | });
  34  | 
  35  | When('player 1 clicks "Create Game"', async ({ page }) => {
  36  |   await page.click("text=Create Game");
  37  |   await page.waitForTimeout(2000); // vänta på API-svar
  38  | });
  39  | 
  40  | Then("a Game ID should be displayed", async ({ page }) => {
> 41  |   await expect(page.locator(".alert-success")).toBeVisible({ timeout: 10000 });
      |                                                ^ Error: expect(locator).toBeVisible() failed
  42  |   await expect(page.locator(".alert-success strong")).not.toBeEmpty();
  43  | });
  44  | 
  45  | /////////////////////////////////////////////////////////////////////////////////
  46  | 
  47  | // Player 2 joinar
  48  | 
  49  | Given("player 1 has created a game", async ({ page }) => {
  50  |   const gameId = await createGame(page);
  51  | });
  52  | 
  53  | When('player 2 enters the GAME ID and clicks "Join Game"', async ({ page }) => {
  54  |   const gameId = await createGame(page);
  55  |   await joinGame(page, gameId);
  56  | });
  57  | 
  58  | Then("player 2 should be navigated to the game screen", async ({ page }) => {
  59  |   await expect(page).toHaveURL(/\/game\/.+/);
  60  | });
  61  | 
  62  | //////////////////////////////////////////////////////////////////////////////
  63  | 
  64  | // Player 1 har första turen
  65  | Given("a game has started with two players", async ({ browser, page }) => {
  66  |   const gameId = await createGame(page);
  67  |   await page.click("text=Enter Game as Player 1");
  68  |   await page.waitForURL(/\/game\/.+/, { timeout: 10000 });
  69  | 
  70  |   const context2 = await browser.newContext();
  71  |   const page2 = await context2.newPage();
  72  |   await joinGame(page2, gameId);
  73  |   page["_page2"] = page2;
  74  | 
  75  |   await page.waitForTimeout(3000); // ökat från 2000
  76  |   await page.waitForSelector("text=Your turn!", { timeout: 15000 }); // ökat från 10000
  77  | });
  78  | 
  79  | Then(
  80  |   "player {int} should see {string}",
  81  |   async ({ page }, playerNumber, text) => {
  82  |     await expect(page.locator(`text=${text}`)).toBeVisible();
  83  |   },
  84  | );
  85  | 
  86  | //////////////////////////////////////////////////////////////////////////
  87  | 
  88  | // Spelet byter turn
  89  | Given("it is player 1s turn", async ({ browser, page }) => {
  90  |   const gameId = await createGame(page);
  91  |   await page.click("text=Enter Game as Player 1");
  92  |   await page.waitForURL(/\/game\/.+/, { timeout: 10000 });
  93  | 
  94  |   const context2 = await browser.newContext();
  95  |   const page2 = await context2.newPage();
  96  |   await joinGame(page2, gameId);
  97  | 
  98  |   // Spara både context2 och page2
  99  |   page["_context2"] = context2;
  100 |   page["_page2"] = page2;
  101 | 
  102 |   await page.waitForTimeout(3000);
  103 |   await page.waitForSelector("text=Your turn!", { timeout: 15000 });
  104 | });
  105 | 
  106 | When("player {int} guesses a letter", async ({ page }, playerNumber) => {
  107 |   await page.fill('input[placeholder="A-Z"]', "A");
  108 |   await page.click("text=Guess!");
  109 |   await page.waitForTimeout(1000);
  110 | });
  111 | 
  112 | Then("it should be player 2s turn", async ({ page }) => {
  113 |   const page2 = page["_page2"];
  114 |   await page2.waitForTimeout(1000);
  115 |   await expect(page2.locator("text=Your turn!")).toBeVisible();
  116 | });
  117 | 
  118 | /////////////////////////////////////////////////////////////////////////////
  119 | 
  120 | // Korrekt gissning
  121 | When(
  122 |   "player {int} guesses a letter that exists in the word",
  123 |   async ({ page }, playerNumber) => {
  124 |     // Gissar E pga vanligt ord
  125 |     await page.fill('input[placeholder="A-Z"]', "E");
  126 |     await page.click("text=Guess!");
  127 |     await page.waitForTimeout(1000);
  128 |   },
  129 | );
  130 | 
  131 | Then("the letter should be shown in the word", async ({ page }) => {
  132 |   // Kollar specifikt att en gissad bokstav badge visas
  133 |   await expect(page.locator(".badge.bg-success.me-1")).toBeVisible();
  134 | });
  135 | 
  136 | ///////////////////////////////////////////////////////////////////////////////
  137 | 
  138 | // Gissa på redan gissad bokstav
  139 | 
  140 | Given('the letter "A" has already been guessed', async ({ browser, page }) => {
  141 |   // Skapa spel och navigera spelare 1 till spelvyn
```