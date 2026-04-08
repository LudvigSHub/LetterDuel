# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: features\game.feature.spec.js >> LetterDuel Game Flow >> Player 1 creates a game
- Location: .features-gen\features\game.feature.spec.js:6:3

# Error details

```
Error: page.goto: net::ERR_CONNECTION_REFUSED at http://localhost:5239/
Call log:
  - navigating to "http://localhost:5239/", waiting until "load"

```

# Page snapshot

```yaml
- generic [ref=e3]:
  - generic [ref=e6]:
    - heading "Webbplatsen kan inte nås" [level=1] [ref=e7]
    - paragraph [ref=e8]:
      - strong [ref=e9]: localhost
      - text: avvisade anslutningen.
    - generic [ref=e10]:
      - paragraph [ref=e11]: Testa att
      - list [ref=e12]:
        - listitem [ref=e13]: kontrollera anslutningen
        - listitem [ref=e14]:
          - link "kontrollera proxyn och brandväggen" [ref=e15] [cursor=pointer]:
            - /url: "#buttons"
    - generic [ref=e16]: ERR_CONNECTION_REFUSED
  - generic [ref=e17]:
    - button "Hämta igen" [ref=e19] [cursor=pointer]
    - button "Detaljer" [ref=e20] [cursor=pointer]
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
  20  |   await page.fill('input[placeholder="Enter Game ID"]', gameId);
  21  | 
  22  |   // Starta navigation och klicka simultaneously
  23  |   await Promise.all([
  24  |     page.waitForURL(/\/game\/.+/, { timeout: 20000 }),
  25  |     page.click("text=Join Game"),
  26  |   ]);
  27  | 
  28  |   await page.screenshot({ path: `debug-join-${Date.now()}.png` });
  29  | }
  30  | 
  31  | // Player 1 startar spel
  32  | 
  33  | Given("player 1 is on the start page", async ({ page }) => {
> 34  |   await page.goto(BASE_URL);
      |              ^ Error: page.goto: net::ERR_CONNECTION_REFUSED at http://localhost:5239/
  35  | });
  36  | 
  37  | When('player 1 clicks "Create Game"', async ({ page }) => {
  38  |   await page.click("text=Create Game");
  39  |   await page.waitForTimeout(2000); // vänta på API-svar
  40  | });
  41  | 
  42  | Then("a Game ID should be displayed", async ({ page }) => {
  43  |   await expect(page.locator(".alert-success")).toBeVisible({ timeout: 10000 });
  44  |   await expect(page.locator(".alert-success strong")).not.toBeEmpty();
  45  | });
  46  | 
  47  | /////////////////////////////////////////////////////////////////////////////////
  48  | 
  49  | // Player 2 joinar
  50  | 
  51  | Given("player 1 has created a game", async ({ page }) => {
  52  |   const gameId = await createGame(page);
  53  | });
  54  | 
  55  | When('player 2 enters the GAME ID and clicks "Join Game"', async ({ page }) => {
  56  |   const gameId = await createGame(page);
  57  |   await joinGame(page, gameId);
  58  | });
  59  | 
  60  | Then("player 2 should be navigated to the game screen", async ({ page }) => {
  61  |   await expect(page).toHaveURL(/\/game\/.+/);
  62  | });
  63  | 
  64  | //////////////////////////////////////////////////////////////////////////////
  65  | 
  66  | // Player 1 har första turen
  67  | Given("a game has started with two players", async ({ browser, page }) => {
  68  |   const gameId = await createGame(page);
  69  |   await page.click("text=Enter Game as Player 1");
  70  |   await page.waitForURL(/\/game\/.+/, { timeout: 10000 });
  71  | 
  72  |   const context2 = await browser.newContext();
  73  |   const page2 = await context2.newPage();
  74  |   await joinGame(page2, gameId);
  75  |   page["_page2"] = page2;
  76  | 
  77  |   await page.waitForTimeout(3000); // ökat från 2000
  78  |   await page.waitForSelector("text=Your turn!", { timeout: 15000 }); // ökat från 10000
  79  | });
  80  | 
  81  | Then(
  82  |   "player {int} should see {string}",
  83  |   async ({ page }, playerNumber, text) => {
  84  |     await expect(page.locator(`text=${text}`)).toBeVisible();
  85  |   },
  86  | );
  87  | 
  88  | //////////////////////////////////////////////////////////////////////////
  89  | 
  90  | // Spelet byter turn
  91  | Given("it is player 1s turn", async ({ browser, page }) => {
  92  |   const gameId = await createGame(page);
  93  |   await page.click("text=Enter Game as Player 1");
  94  |   await page.waitForURL(/\/game\/.+/, { timeout: 10000 });
  95  | 
  96  |   const context2 = await browser.newContext();
  97  |   const page2 = await context2.newPage();
  98  |   await joinGame(page2, gameId);
  99  | 
  100 |   // Spara både context2 och page2
  101 |   page["_context2"] = context2;
  102 |   page["_page2"] = page2;
  103 | 
  104 |   await page.waitForTimeout(3000);
  105 |   await page.waitForSelector("text=Your turn!", { timeout: 15000 });
  106 | });
  107 | 
  108 | When("player {int} guesses a letter", async ({ page }, playerNumber) => {
  109 |   await page.fill('input[placeholder="A-Z"]', "A");
  110 |   await page.click("text=Guess!");
  111 |   await page.waitForTimeout(1000);
  112 | });
  113 | 
  114 | Then("it should be player 2s turn", async ({ page }) => {
  115 |   const page2 = page["_page2"];
  116 |   await page2.waitForTimeout(1000);
  117 |   await expect(page2.locator("text=Your turn!")).toBeVisible();
  118 | });
  119 | 
  120 | /////////////////////////////////////////////////////////////////////////////
  121 | 
  122 | // Korrekt gissning
  123 | When(
  124 |   "player {int} guesses a letter that exists in the word",
  125 |   async ({ page }, playerNumber) => {
  126 |     // Gissar E pga vanligt ord
  127 |     await page.fill('input[placeholder="A-Z"]', "E");
  128 |     await page.click("text=Guess!");
  129 |     await page.waitForTimeout(1000);
  130 |   },
  131 | );
  132 | 
  133 | Then("the letter should be shown in the word", async ({ page }) => {
  134 |   // Kollar specifikt att en gissad bokstav badge visas
```