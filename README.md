# FoodMoment

A **food & drink** companion app built with **.NET MAUI** (.NET 9). Browse meals and beverages, manage recipes locally, and demo required device features (TTS, camera, GPS).

## Tech stack

| Layer | Choice |
|--------|--------|
| UI | .NET MAUI (XAML), Shell `TabBar` + routes |
| Pattern | MVVM (CommunityToolkit.Mvvm) |
| Local DB | SQLite (`sqlite-net-pcl`) |
| Embedded data | `Resources/Raw/recipes.json` + `recipes_fallback.json` |
| Platforms | Windows + Android |

## Data loading (dual insurance — local static only)

`RecipeDataLoaderService` implements the required try/catch chain **without mockapi.io**:

1. **Try** embedded `recipes.json` (`EmbeddedRecipeDataService`)
2. **Catch** → embedded `recipes_fallback.json` (`LocalFallbackRecipeDataService`)
3. **Catch** → built-in list (`StaticRecipeCatalog`)

Asset names are in **`DataConfig.cs`**. Used on first launch and when pulling to refresh / **Reload** on Profile.

## Images

All seed recipes use **bundled JPG files** under `Resources/Images/` (offline-friendly for demo video). Empty image URL on add-recipe defaults to `default_recipe.jpg`.

## Theme

`App.UserAppTheme = AppTheme.Unspecified` — the app **follows the system** light/dark mode. Pages use `{AppThemeBinding}` in XAML. There is no manual theme picker.

## Feature checklist (assignment)

| Category | Feature | Status |
|----------|---------|--------|
| Hardware | Text-to-speech (read step aloud) | ✅ |
| Hardware | Stop speech button | ✅ |
| Hardware | Camera capture + image on detail page | ✅ |
| Hardware | GPS latitude / longitude display | ✅ |
| Data | Embedded JSON first | ✅ |
| Data | Local static fallback (second JSON + hardcoded) | ✅ |
| Data | SQLite persistence + CRUD | ✅ |
| UI | System light/dark via `AppThemeBinding` | ✅ |
| UI | Recipe list + detail | ✅ |
| UI | Search | ✅ |
| UI | Category filter (incl. **Drinks**, Dessert) | ✅ |
| UI | Pull-to-refresh (`RefreshView`) | ✅ |
| UX | `ActivityIndicator` while loading | ✅ |
| UX | Haptic feedback (favorite / photo / location) | ✅ |

## Project structure

```
FoodMoment/
├── DataConfig.cs                    # Primary / fallback asset names
├── Resources/Raw/recipes.json       # Primary seed data
├── Resources/Raw/recipes_fallback.json
├── Resources/Images/*.jpg           # Offline recipe thumbnails
├── Services/
│   ├── EmbeddedRecipeDataService.cs
│   ├── LocalFallbackRecipeDataService.cs
│   ├── RecipeDataLoaderService.cs
│   ├── StaticRecipeCatalog.cs
│   └── RecipeRepository.cs
├── Views/ / ViewModels/
└── AppShell.xaml
```

## Run

**Important:** Open `FoodMoment.sln` (not an old `FoodDrinkApp.sln`). In Visual Studio, set the startup dropdown to **Windows Machine** — not Android emulator. If the profile is **Pixel 7 / Android** and the emulator is off, the app will look like it “won’t open”.

```powershell
cd c:\Users\Administrator\Desktop\FoodTime\FoodMoment
dotnet build -f net9.0-windows10.0.19041.0
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

Build output is redirected to `C:\MauiBuild\FoodMoment\` (see `Directory.Build.props`).

Pre-demo verification:

```powershell
.\scripts\verify-demo.ps1
```

See **`docs/DEMO_VERIFICATION.md`** for manual TTS / camera / GPS checks.

## Video demo tips

- Toggle **Windows dark mode** → app colors update automatically  
- **Profile → Reload** → shows whether data came from `EmbeddedJson`, `LocalStaticJson`, or `HardcodedFallback`  
- **Detail** → Read aloud / **Stop** / Capture (photo appears above tabs)  
- **Explore** → Get GPS location (latitude & longitude labels)  
- Full CRUD via Profile → Manage recipes  
- List images work **without network**

## Not implemented

- Live map control (coordinates shown as text; GPS labels only)  
- mockapi.io / remote REST (replaced by local JSON fallback chain)  
- Real backend / user accounts / cloud sync  
- Gallery photo pick, offline URL cache, unit/UI tests, shake gesture, manual theme toggle  
- iOS target framework  
