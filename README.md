# LetterDuel

LetterDuel is a turn-based two-player word game built with a separate UI and Backend.  
The project was developed with a strong focus on automated testing, including unit tests, API tests, and BDD/UI tests.

## Project Structure

- **LetterDuel.UI** – frontend application
- **LetterDuel.Backend** – Web API, game logic, and data access

## Game Overview

In LetterDuel, two players join the same game and take turns guessing letters in a hidden word.  
The game keeps track of player scores, guessed letters, turn order, and game state.

Example game states:
- `WaitingForPlayers`
- `InProgress`
- `GameFinished`

These states are part of the intended system behavior for the game.

## Requirements to Run the Project

To run the application normally, start both the UI and the Backend at the same time.
Make sure both projects use the DefaultProgram launch profile.
The DefaultProgram profile contains the larger seed data for playing the application as intended, with random seeded words.

### Important when doing Postman tests

Set **both** projects as **Startup Projects** in Visual Studio:

- `LetterDuel.UI`
- `LetterDuel.Backend`

Also make sure that **both projects use the `Testing` launch profile**, when testing with postman.

If you use another launch profile, the automated tests may fail because they expect the application to run with the testing-specific configuration.

The Postman collection and environment are located in "PostmanFiles" in the solution folder.
The postman tests are simulated to play a game with the word "APPLE"
Make sure the baseUrl in the environment is correct, the url for the API is http://localhost:5100.

## Database Setup

Before running the application, make sure the database is updated.

Run the following commands in the Package Manager Console:

update-database -Project LetterDuel.Backend -StartupProject LetterDuel.Backend

## BDD / E2E Testing

BDD / E2E tests are located in the `E2E` folder and are written with Playwright.

Do not start the application manually before running these tests.
The `run-tests.ps1` script handles that automatically.

### Run the tests

```powershell
cd E2E
npm install
npx playwright install
cd ..
.\run-tests.ps1
```

### What the script does

The script automatically:

* stops any already running instances of the application
* starts the backend with the `Testing` launch profile
* starts the UI with the `DefaultProgram` launch profile
* waits for both projects to become available
* runs the Playwright tests
* stops the started processes afterwards

This makes the test flow easier to use and ensures the projects are started with the correct profiles for the current E2E setup.

