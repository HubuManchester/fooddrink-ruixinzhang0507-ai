# Quick pre-demo checks for FoodMoment (run from repo root).
$ErrorActionPreference = "Stop"
$root = Split-Path $PSScriptRoot -Parent
$project = Join-Path $root "FoodMoment"
$recipesPath = Join-Path $project "Resources\Raw\recipes.json"
$imagesDir = Join-Path $project "Resources\Images"

Write-Host "=== FoodMoment demo verification ===" -ForegroundColor Cyan

# 1. Primary JSON + image references
$json = Get-Content $recipesPath -Raw | ConvertFrom-Json
$missing = @()
foreach ($r in $json.recipes) {
    if ($r.imageUrl -notmatch '^https?://') {
        $img = Join-Path $imagesDir $r.imageUrl
        if (-not (Test-Path $img)) { $missing += $r.imageUrl }
    }
}
if ($missing.Count -gt 0) {
    Write-Host "FAIL: Missing images:" ($missing -join ", ") -ForegroundColor Red
    exit 1
}
Write-Host "OK: $($json.recipes.Count) recipes, all local image files present."

# 2. Fallback JSON
$fallbackPath = Join-Path $project "Resources\Raw\recipes_fallback.json"
if (-not (Test-Path $fallbackPath)) {
    Write-Host "FAIL: recipes_fallback.json missing" -ForegroundColor Red
    exit 1
}
Write-Host "OK: recipes_fallback.json present."

# 3. Build Windows target
Push-Location $project
try {
    dotnet build -f net9.0-windows10.0.19041.0 -v q
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Write-Host "OK: Windows build succeeded."
}
finally {
    Pop-Location
}

Write-Host ""
Write-Host "Manual checks before recording video:" -ForegroundColor Yellow
Write-Host "  - Detail: Read aloud / Stop (TTS — enable system speech on Windows)"
Write-Host "  - Detail: Capture — photo appears above tabs"
Write-Host "  - Explore: Get GPS — lat/long update (enable Location in OS settings)"
Write-Host "  - List thumbnails use bundled JPGs (no network required)"
Write-Host "  - Profile -> Reload shows EmbeddedJson as data source"
