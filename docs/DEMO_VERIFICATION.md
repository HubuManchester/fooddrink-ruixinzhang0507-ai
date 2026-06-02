# Demo verification (Section 5)

Run automated checks:

```powershell
.\scripts\verify-demo.ps1
```

## Verified in code (ready for demo)

| Item | Status | Notes |
|------|--------|-------|
| Recipe list thumbnails | OK | Bundled `Resources/Images/*.jpg` via `RecipeImageSourceConverter` — **no network** |
| TTS read step + Stop | OK | `RecipeDetailViewModel.ReadAloudAsync` / `StopSpeech` |
| Camera → detail preview | OK | `CapturePhotoAsync` saves to AppData, binds `LastCapturedPhotoPath` |
| GPS lat/long | OK | `ExploreViewModel.GetLocationAsync` + permission prompt |
| Data fallback chain | OK | `recipes.json` → `recipes_fallback.json` → `StaticRecipeCatalog` |
| System light/dark | OK | `AppTheme.Unspecified` + `AppThemeBinding` in XAML |

## Manual checks on your device (required before video)

1. **TTS** — Open any recipe → Steps → **Read aloud**. On Windows, ensure *Settings → Time & language → Speech* has a voice installed. Android uses system TTS engine.
2. **Camera** — **Capture** on detail page; confirm photo appears in “Your dish photo” and under Profile → Cooking photos.
3. **Location** — **Explore** → **Get GPS**; enable location for the app in OS settings; confirm latitude/longitude change from “—”.
4. **Reload** — Profile → **Reload**; status should show `Last load: EmbeddedJson`.

## Out of scope (not implemented by design)

- Gallery pick (`PickPhotoAsync`)
- Offline cache for remote URLs
- Unit / UI automated tests
- Page transition animations / full accessibility audit
- Shake / accelerometer
- Manual light/dark toggle (system theme only)
- Interactive map / map pin picker

## Test fallback chain (optional)

To demonstrate secondary JSON in Profile:

1. Temporarily rename `Resources/Raw/recipes.json` to `recipes.json.bak`
2. Rebuild and run → Profile should show `LocalStaticJson` from `recipes_fallback.json`
3. Restore `recipes.json` before submission
