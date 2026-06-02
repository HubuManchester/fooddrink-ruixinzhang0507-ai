# FoodMoment

A **food and drink** companion app built with **.NET MAUI** (.NET 9). Browse recipes offline, manage your own entries in SQLite, and demonstrate mobile hardware features (TTS, camera, GPS, haptics).

**Author:** Ruixin Zhang  
**Repository:** [HubuManchester/fooddrink-ruixinzhang0507-ai](https://github.com/HubuManchester/fooddrink-ruixinzhang0507-ai)

## Tech stack

| Layer | Technology |
|--------|------------|
| UI | .NET MAUI (XAML), Shell `TabBar` and navigation routes |
| Pattern | MVVM (CommunityToolkit.Mvvm) |
| Database | SQLite (`sqlite-net-pcl`) |
| Seed data | `Resources/Raw/recipes.json` + `recipes_fallback.json` |
| Platforms | **Windows** and **Android** |

## Features

| Area | Capability |
|------|------------|
| Browse | 14 bundled recipes, search, category filter, pull-to-refresh |
| Detail | Ingredients / steps, nutrition panel, favorites |
| Hardware | Text-to-speech (read step + stop), camera capture, GPS coordinates, haptic feedback |
| Data | JSON → fallback JSON → hardcoded catalog; SQLite merge on launch |
| Profile | Theme (Light / Dark / System), reload data, cook history, recipe CRUD |
| Images | Local JPG assets under `Resources/Images/` (no network required for list thumbnails) |

## Data loading

`RecipeDataLoaderService` uses a try/catch fallback chain (no remote API):

1. Embedded `recipes.json`
2. Embedded `recipes_fallback.json`
3. `StaticRecipeCatalog` (built-in list)

Configuration: `DataConfig.cs`. Reload from **Profile → Reload data** to see the active source (`EmbeddedJson`, `LocalStaticJson`, or `HardcodedFallback`).

## Project structure

```
FoodMoment/
├── DataConfig.cs
├── Resources/
│   ├── Raw/recipes.json
│   ├── Raw/recipes_fallback.json
│   └── Images/*.jpg
├── Services/
│   ├── RecipeDataLoaderService.cs
│   ├── RecipeRepository.cs
│   ├── CameraPhotoService.cs
│   └── ThemePreferenceService.cs
├── ViewModels/
├── Views/
└── AppShell.xaml
```

## Build and run

Open **`FoodMoment.sln`** in Visual Studio. Set the startup profile to **Windows Machine** (not Android emulator unless you intend to test on Android).

```powershell
cd FoodMoment
dotnet build -f net9.0-windows10.0.19041.0
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

Build output may be redirected to `C:\MauiBuild\FoodMoment\` (see `Directory.Build.props` in the repo root).

### Pre-demo check

From the repository root:

```powershell
.\scripts\verify-demo.ps1
```

Manual hardware checks: see `docs/DEMO_VERIFICATION.md`.

## Video demo (suggested flow)

1. **Home** — scroll recipes (offline thumbnails), search, filter by category, toggle favorite.
2. **Detail** — **Read aloud** / **Stop**, **Capture** (photo appears on the page), view nutrition.
3. **Explore** — **Get GPS** (allow location); latitude and longitude update.
4. **Profile** — switch theme, **Reload data**, open **Manage recipes** for CRUD.
5. Optionally show **Android** or **Windows** dark mode.

## Hardware used in this app

| # | Feature | Where to demo |
|---|---------|----------------|
| 1 | Text-to-speech | Recipe detail → Read aloud / Stop |
| 2 | Camera | Recipe detail → Capture |
| 3 | GPS / geolocation | Explore → Get GPS |
| 4 | Haptic feedback | Favorite, capture success, location update |

Cooking photos are stored under `FileSystem.AppDataDirectory/records/`.

## Not implemented

- Interactive map (GPS shown as text only)
- Shake / accelerometer
- Remote REST API (mockapi.io replaced by local JSON chain)
- iOS target
- Automated unit/UI tests

## License

Academic coursework submission — University of Manchester (HubuManchester classroom repository).
