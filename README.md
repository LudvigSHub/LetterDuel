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

These states are part of the intended system behavior for the game. :contentReference[oaicite:0]{index=0} :contentReference[oaicite:1]{index=1}

## Requirements to Run the Project

To run the application correctly, you must start **both the UI and the Backend at the same time**.

### Important

Set **both** projects as **Startup Projects** in Visual Studio:

- `LetterDuel.UI`
- `LetterDuel.Backend`

Also make sure that **both projects use the `Testing` launch profile**.

This is required because the **BDD tests** and **Postman tests** are written and configured to run against the application using the `Testing` profile for both the UI and the Backend.

If you use another launch profile, the automated tests may fail because they expect the application to run with the testing-specific configuration.

## Database Setup

Before running the application, make sure the database is updated.

Run the following commands in the Package Manager Console:

update-database -Project LetterDuel.Backend -StartupProject LetterDuel.Backend

## BDD / E2E Testing

The BDD / E2E tests must be run from **Visual Studio Code** (or another terminal environment), not from Visual Studio.

These tests are located in the `E2E` folder and use Playwright.

### Important

Before running the BDD tests:

- Make sure **both UI and Backend are running**
- Make sure **both projects use the `Testing` launch profile**
- Open the project in **VS Code**
- Run the test setup and script from the terminal

### Commands

```powershell
cd E2E
npm install
npx playwright install
cd ..
.\run-tests.ps1
