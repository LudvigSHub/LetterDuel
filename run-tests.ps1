# Stoppa eventuella körande instanser
Write-Host "Stopping any running instances..."
taskkill /F /IM LetterDuel.Backend.exe 2>$null
taskkill /F /IM LetterDuel.UI.exe 2>$null

# Vänta lite
Start-Sleep -Seconds 2

# Spara solution-mappen
$solutionDir = $PSScriptRoot

# Starta Backend i Testing-läge i bakgrunden
Write-Host "Starting Backend in Testing mode..."
$backend = Start-Process -FilePath "dotnet" -ArgumentList "run --project Backend --launch-profile Testing" -WorkingDirectory $solutionDir -PassThru -WindowStyle Minimized

# Vänta på att Backend startar
Write-Host "Waiting for Backend to start..."
Start-Sleep -Seconds 45

# Starta Blazor i bakgrunden
Write-Host "Starting Blazor..."
$blazor = Start-Process -FilePath "dotnet" -ArgumentList "run --project BlazorApp1" -WorkingDirectory $solutionDir -PassThru -WindowStyle Minimized

# Vänta på att Blazor startar
Write-Host "Waiting for Blazor to start..."
Start-Sleep -Seconds 45

# Kör E2E-tester
Write-Host "Running E2E tests..."
Set-Location "$solutionDir\E2E"
npm test

# Stoppa processer när testerna är klara
Write-Host "Stopping processes..."
Stop-Process -Id $backend.Id -Force 2>$null
Stop-Process -Id $blazor.Id -Force 2>$null

Write-Host "Done!"