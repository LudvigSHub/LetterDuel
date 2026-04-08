# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: features\game.feature.spec.js >> LetterDuel Game Flow >> Game ends when word is complete
- Location: .features-gen\features\game.feature.spec.js:41:3

# Error details

```
Error: expect(page).toHaveURL(expected) failed

Expected pattern: /\/game\/.+\/result/
Received string:  "https://localhost:7051/game/8c35a3e8-3c9f-4635-8701-72e1adb6d55e"
Timeout: 15000ms

Call log:
  - Expect "toHaveURL" with timeout 15000ms
    18 × unexpected value "https://localhost:7051/game/8c35a3e8-3c9f-4635-8701-72e1adb6d55e"

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
        - generic [ref=e18]: You are Player 1
        - generic [ref=e19]:
          - generic [ref=e20]:
            - heading "Player 1" [level=5] [ref=e21]
            - generic [ref=e22]: 0 p
            - generic [ref=e24]: Your turn!
          - heading "VS" [level=5] [ref=e26]
          - generic [ref=e27]:
            - heading "Player 2" [level=5] [ref=e28]
            - generic [ref=e29]: 2 p
        - generic [ref=e38]: E
        - paragraph [ref=e42]:
          - strong [ref=e43]: "Guessed letters:"
          - generic [ref=e44]: A
          - generic [ref=e45]: E
          - generic [ref=e46]: L
          - generic [ref=e47]: P
        - generic [ref=e49]:
          - textbox "A-Z" [ref=e50]
          - button "Guess!" [ref=e51] [cursor=pointer]
        - generic [ref=e53]:
          - text: "Game ID:"
          - strong [ref=e54]: 8c35a3e8-3c9f-4635-8701-72e1adb6d55e
```

# Test source

```ts
  113 | 
  114 | /////////////////////////////////////////////////////////////////////////////
  115 | 
  116 | // Korrekt gissning
  117 | When(
  118 |   "player {int} guesses a letter that exists in the word",
  119 |   async ({ page }, playerNumber) => {
  120 |     // Gissar E pga vanligt ord
  121 |     await page.fill('input[placeholder="A-Z"]', "E");
  122 |     await page.click("text=Guess!");
  123 |     await page.waitForTimeout(1000);
  124 |   },
  125 | );
  126 | 
  127 | Then("the letter should be shown in the word", async ({ page }) => {
  128 |   // Kollar specifikt att en gissad bokstav badge visas
  129 |   await expect(page.locator(".badge.bg-success.me-1")).toBeVisible();
  130 | });
  131 | 
  132 | ///////////////////////////////////////////////////////////////////////////////
  133 | 
  134 | // Gissa på redan gissad bokstav
  135 | 
  136 | Given('the letter "A" has already been guessed', async ({ browser, page }) => {
  137 |   // Skapa spel och navigera spelare 1 till spelvyn
  138 |   const gameId = await createGame(page);
  139 |   await page.click("text=Enter Game as Player 1");
  140 |   await page.waitForURL(/\/game\/.+/, { timeout: 10000 });
  141 | 
  142 |   // Spelare 2 joinar i separat kontext
  143 |   const context2 = await browser.newContext();
  144 |   const page2 = await context2.newPage();
  145 |   await joinGame(page2, gameId);
  146 |   page["_page2"] = page2;
  147 | 
  148 |   // Vänta på polling så spelet startar
  149 |   await page.waitForTimeout(2000);
  150 |   await page.waitForSelector("text=Your turn!", { timeout: 10000 });
  151 | 
  152 |   // Spelare 1 gissar A
  153 |   await page.fill('input[placeholder="A-Z"]', "A");
  154 |   await page.click("text=Guess!");
  155 |   await page.waitForTimeout(1000);
  156 | 
  157 |   // Spelare 2 gissar B så turen går tillbaka till spelare 1
  158 |   await page2.waitForSelector("text=Your turn!", { timeout: 10000 });
  159 |   await page2.fill('input[placeholder="A-Z"]', "B");
  160 |   await page2.click("text=Guess!");
  161 |   await page2.waitForTimeout(1000);
  162 | 
  163 |   // Vänta på att det är spelare 1s tur igen
  164 |   await page.waitForSelector("text=Your turn!", { timeout: 10000 });
  165 | });
  166 | 
  167 | When('player 1 tries to guess "A" again', async ({ page }) => {
  168 |   await page.fill('input[placeholder="A-Z"]', "A");
  169 |   await page.click("text=Guess!");
  170 | });
  171 | 
  172 | Then("an error message should be displayed", async ({ page }) => {
  173 |   await expect(page.locator(".alert-danger")).toBeVisible();
  174 | });
  175 | 
  176 | ////////////////////////////////////////////////////////////////////////////////
  177 | 
  178 | Given("only one letter remains", async ({ browser, page }) => {
  179 |   const gameId = await createGame(page);
  180 |   await page.click("text=Enter Game as Player 1");
  181 |   await page.waitForURL(/\/game\/.+/, { timeout: 10000 });
  182 | 
  183 |   const context2 = await browser.newContext();
  184 |   const page2 = await context2.newPage();
  185 |   await joinGame(page2, gameId);
  186 |   page["_page2"] = page2;
  187 | 
  188 |   await page.waitForTimeout(3000);
  189 |   await page.waitForSelector("text=Your turn!", { timeout: 15000 });
  190 | 
  191 |   // Gissa alla bokstäver i APPLE utom E
  192 |   const letters = ["A", "P", "L"];
  193 |   for (let i = 0; i < letters.length; i++) {
  194 |     const currentPage = i % 2 === 0 ? page : page2;
  195 |     await currentPage.waitForSelector("text=Your turn!", { timeout: 10000 });
  196 |     await currentPage.fill('input[placeholder="A-Z"]', letters[i]);
  197 |     await currentPage.click("text=Guess!");
  198 |     await currentPage.waitForTimeout(1000);
  199 |   }
  200 | });
  201 | 
  202 | When("the correct letter is guessed", async ({ page }) => {
  203 |   const page2 = page["_page2"];
  204 |   // Det är spelare 2s tur att gissa E
  205 |   await page2.waitForSelector("text=Your turn!", { timeout: 10000 });
  206 |   await page2.fill('input[placeholder="A-Z"]', "E");
  207 |   await page2.click("text=Guess!");
  208 |   await page2.waitForTimeout(3000);
  209 | });
  210 | 
  211 | Then("the result page should be displayed", async ({ page }) => {
  212 |   const page2 = page["_page2"];
> 213 |   await expect(page2).toHaveURL(/\/game\/.+\/result/, { timeout: 15000 });
      |                       ^ Error: expect(page).toHaveURL(expected) failed
  214 | });
  215 | 
  216 | Given("a game is in progress", async ({ browser, page }) => {
  217 |   const gameId = await createGame(page);
  218 |   await page.click("text=Enter Game as Player 1");
  219 |   await page.waitForURL(/\/game\/.+/, { timeout: 10000 });
  220 | 
  221 |   const context2 = await browser.newContext();
  222 |   const page2 = await context2.newPage();
  223 |   await joinGame(page2, gameId);
  224 | 
  225 |   page["_context2"] = context2;
  226 |   page["_page2"] = page2;
  227 | 
  228 |   await page.waitForTimeout(3000);
  229 |   await page.waitForSelector("text=Your turn!", { timeout: 15000 });
  230 | });
  231 | 
  232 | Given("it is the players turn", async ({ page }) => {
  233 |   await expect(page.locator('input[placeholder="A-Z"]')).toBeVisible();
  234 | });
  235 | 
  236 | When('the player enters "7" as a guess', async ({ page }) => {
  237 |   await page.fill('input[placeholder="A-Z"]', "7");
  238 |   await page.click("text=Guess!");
  239 |   await page.waitForTimeout(2000); // vänta på API-svar
  240 | });
  241 | 
  242 | When('the player enters "@" as a guess', async ({ page }) => {
  243 |   await page.fill('input[placeholder="A-Z"]', "@");
  244 |   await page.click("text=Guess!");
  245 |   await page.waitForTimeout(2000);
  246 | });
  247 | 
  248 | When('the player enters "AB" as a guess', async ({ page }) => {
  249 |   await page.fill('input[placeholder="A-Z"]', "AB");
  250 |   await page.click("text=Guess!");
  251 |   await page.waitForTimeout(2000);
  252 | });
  253 | 
  254 | Then("a warning should be displayed", async ({ page }) => {
  255 |   await expect(page.locator(".alert-danger")).toBeVisible({ timeout: 10000 });
  256 | });
  257 | 
  258 | Then("the guess should not be accepted", async ({ page }) => {
  259 |   await expect(page.locator(".alert-danger")).toContainText("Only letters");
  260 | });
  261 | 
  262 | Then("the game state should not change", async ({ page }) => {
  263 |   // Kontrollera att det fortfarande är spelarens tur
  264 |   await expect(page.locator('input[placeholder="A-Z"]')).toBeVisible();
  265 | });
  266 | 
```