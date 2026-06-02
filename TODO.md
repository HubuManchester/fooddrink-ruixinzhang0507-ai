# FoodMoment — Remaining Work & Asset Checklist

This document lists what is **already in code** versus what you still need to **configure, download, or complete** before submission and video demo. Use it as a homework / delivery checklist.

---

## Quick summary

| Area | Status |
|------|--------|
| Core app structure (Shell, MVVM, SQLite, CRUD) | Done in code |
| Embedded `recipes.json` + API fallback chain | Done in code — **API URL & JSON on server need your setup** |
| Recipe list images | **Remote URLs only** — no local recipe photos bundled |
| Hardware (TTS, camera, GPS text) | Done in code — **test on real device / emulator** |
| System light/dark theme | Done |
| Interactive map | **Not implemented** (text coordinates only) |
| Your own Mock API project | **Not done** — placeholder URL may 404 |
| Local image files under `Resources/Images/` | **Folder empty** — see below |
| 4-phase GitHub commit history | **Not done** (process, not code) |

---

## 1. Images & file paths

### 1.1 How images work today

All seed recipes use **`imageUrl` = HTTPS link** (Unsplash). The app binds `Image Source="{Binding ImageUrl}"` on list/detail pages.  
**If the device has no internet**, list thumbnails may be **blank** even though titles load from SQLite/JSON.

Cooking photos from the camera use **local paths** under:

`FileSystem.AppDataDirectory/records/cook_{recipeId}_{timestamp}.jpg`

Those are created at runtime; you do not add them to the repo.

### 1.2 Remote URLs currently in the project (verify they still load)

**Embedded `Resources/Raw/recipes.json`** — 8 recipes:

| # | Recipe | `imageUrl` (remote) |
|---|--------|---------------------|
| 1 | Classic Tomato Eggs | `https://images.unsplash.com/photo-1608039829572-7854f8d7b2fd?w=400` |
| 2 | Avocado Toast | `https://images.unsplash.com/photo-1525351484163-7529414344d8?w=400` |
| 3 | Iced Matcha Latte | `https://images.unsplash.com/photo-1515823064-d6e0f004a4f2?w=400` |
| 4 | Fresh Orange Juice | `https://images.unsplash.com/photo-1621506289937-a8e4df240d0b?w=400` |
| 5 | Berry Smoothie Bowl | `https://images.unsplash.com/photo-1590301157890-4810ed221531?w=400` |
| 6 | Stir-Fried Greens | `https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400` |
| 7 | Chocolate Lava Cake | `https://images.unsplash.com/photo-1624353365286-3f8d62daad51?w=400` |
| 8 | Honey Lemon Tea | `https://images.unsplash.com/photo-1544787219-7f47ccb76574?w=400` |

**Default when adding a recipe without image URL** (`RecipeEditViewModel.cs`):

`https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400`

**Hardcoded API fallback** (`MockRecipeApiService.GetHardcodedFallbackRecipes`) — 3 URLs (same Unsplash pattern).

**Action for you:**

- [ ] Open each URL in a browser; replace any broken link.
- [ ] **Recommended for demo reliability:** download 8–10 food/drink photos (royalty-free), put under `FoodMoment/Resources/Images/`, e.g.  
  `tomato_eggs.jpg`, `matcha_latte.jpg`, …  
  Then change `recipes.json` to use MAUI local images, e.g.  
  `"imageUrl": "tomato_eggs.jpg"`  
  (MAUI resolves `MauiImage` assets by filename in bindings.)
- [ ] Or keep URLs but **record demo with network on**.

### 1.3 Bundled MAUI images (app branding)

| File | Purpose | Status |
|------|---------|--------|
| `Resources/AppIcon/appicon.svg` | App icon | Present |
| `Resources/AppIcon/appiconfg.svg` | Icon foreground | Present |
| `Resources/Splash/splash.svg` | Splash screen | Present |
| `Resources/Images/dotnet_bot.png` | Referenced in `.csproj` | **Missing** — folder `Resources/Images/` is empty |

**Action:**

- [ ] Remove the `MauiImage Update="Resources\Images\dotnet_bot.png"` line from `FoodMoment.csproj`, **or**
- [ ] Add a real `dotnet_bot.png` (or replace with your own placeholder image).

### 1.4 Optional local recipe image set (suggested filenames)

If you bundle local images, aim for **at least one image per category** (food + drinks):

| Suggested file | Suggested use |
|----------------|---------------|
| `tomato_eggs.jpg` | Chinese / breakfast |
| `avocado_toast.jpg` | Breakfast |
| `matcha_latte.jpg` | Drinks |
| `orange_juice.jpg` | Drinks |
| `smoothie_bowl.jpg` | Drinks |
| `stir_fry_greens.jpg` | Vegetarian |
| `lava_cake.jpg` | Dessert |
| `honey_lemon_tea.jpg` | Drinks |
| `default_recipe.jpg` | Fallback in `RecipeEditViewModel` when URL empty |

License: use Unsplash download, Wikimedia Commons, or your own photos; note source in report if required.

---

## 2. API & data configuration

### 2.1 Mock API (must be set up by you)

**Config file:** `FoodMoment/ApiConfig.cs`

```csharp
public const string MockRecipesUrl =
    "https://675f8f8d8e2974791ad9759f.mockapi.io/api/v1/recipes";
```

This URL is a **placeholder**. It may not be your project and may **fail at runtime** (then the app uses hardcoded fallback — still works for demo, but graders may want to see a real HTTP success).

**Action:**

- [ ] Create a project on [mockapi.io](https://mockapi.io) (or similar).
- [ ] Create a `recipes` resource with fields matching `Models/Recipe.cs`:  
  `id`, `title`, `description`, `imageUrl`, `category`, `ingredients` (array), `steps` (array), `isFavorite` (optional).
- [ ] Paste your endpoint into `ApiConfig.MockRecipesUrl`.
- [ ] Paste **the same JSON shape** as `recipes.json` (or a JSON **array** of recipes — code supports both).
- [ ] Test: Profile → **Reload (JSON → API fallback)** with Wi‑Fi on; confirm Profile shows `Last load: MockApi` when you temporarily rename/delete embedded file to force API path (optional test).

**Expected JSON shapes (either works):**

```json
{ "recipes": [ { "id": 1, "title": "...", ... } ] }
```

or

```json
[ { "id": 1, "title": "...", ... } ]
```

### 2.2 Embedded JSON (mostly done)

| Item | Path | Action |
|------|------|--------|
| Seed data | `Resources/Raw/recipes.json` | Done — add more recipes/drinks if you want a richer demo |
| Build copy | Auto via `<MauiAsset Include="Resources\Raw\**" />` | No change needed |

### 2.3 SQLite database

| Item | Notes |
|------|--------|
| File | `foodmoment.db3` in app data directory (created on first run) |
| Re-seed | Uninstall app or delete DB if you change `recipes.json` IDs and need a clean seed |

**Action:**

- [ ] After changing seed JSON heavily, clear app data once and relaunch.

### 2.4 Data loading logic (done — for reference)

Implemented in `RecipeDataLoaderService.cs`:

1. Try embedded `recipes.json`
2. `catch` → HTTP `ApiConfig.MockRecipesUrl`
3. `catch` → `GetHardcodedFallbackRecipes()`

No further code required unless your teacher wants logging/UI proof of which step ran (Profile already shows last source message).

---

## 3. Maps & location (optional / partial)

| Item | Status |
|------|--------|
| GPS latitude/longitude labels | Done on Explore page |
| Reverse geocode (address) | Done (may fail without network) |
| Nearby places list | **Mock offsets** — not Google Places API |
| `Microsoft.Maui.Controls.Maps` | **Removed** from project (was causing Windows crashes) |
| Google Maps API key | Still in `Platforms/Android/Resources/values/strings.xml` as `PASTE_YOUR_GOOGLE_MAPS_API_KEY_HERE` — **unused** while map control is off |

**Action (only if you re-enable map):**

- [ ] Add NuGet `Microsoft.Maui.Controls.Maps` and `UseMauiMaps()` in `MauiProgram.cs`
- [ ] Add `<maps:Map>` back to `ExplorePage.xaml`
- [ ] Register a real Google Maps API key in `strings.xml` (Android) and equivalent for Windows
- [ ] Test on Android emulator with Google Play

For minimum assignment requirements, **text lat/long is enough** — map is optional.

---

## 4. Platform & permissions

### 4.1 Android (`Platforms/Android/AndroidManifest.xml`)

Already declared: `INTERNET`, `CAMERA`, `ACCESS_FINE_LOCATION`, `ACCESS_COARSE_LOCATION`.

**Action:**

- [ ] Run on **physical phone or emulator** and accept permission dialogs for camera & location when testing.
- [ ] Remove or fill `google_maps_api_key` if you are not using maps (avoids confusion in report).

### 4.2 Windows (`Platforms/Windows/Package.appxmanifest`)

Only `runFullTrust` is listed. MAUI often prompts at runtime for camera/location on Windows 10/11.

**Action:**

- [ ] Test camera on Windows (may open webcam); if it fails, demo camera on **Android** in video.
- [ ] Consider adding capabilities if required by course docs: `webcam`, `location` (check latest MAUI Windows docs).

### 4.3 Application identity

| Item | Current value | Action |
|------|---------------|--------|
| `ApplicationId` | `com.companyname.foodmoment` | [ ] Change to your own package name if required |
| Publisher / display name in manifest | Placeholders | [ ] Update for store-style submission if needed |

### 4.4 iOS / macOS

**Not in** `TargetFrameworks` (only Android + Windows).

**Action:** [ ] Add only if course requires Apple platform.

---

## 5. Features — done vs partial vs not started

### Done (implemented in code)

- [x] Food & drink categories (`Drinks`, `Dessert`, etc.)
- [x] Recipe list + detail + CRUD pages
- [x] Search + category filter + pull-to-refresh
- [x] Favorites + cook history with photo path
- [x] TTS read step + **Stop** button
- [x] Camera capture + preview on detail page
- [x] GPS coordinates on Explore
- [x] System theme via `AppTheme.Unspecified` + `AppThemeBinding`
- [x] Dual-insurance data load (JSON → API → hardcoded)

### Partial / demo-dependent (you must verify)

- [ ] TTS audible on target device (Windows vs Android voice differs)
- [ ] Camera shows captured image on detail page (test once before recording)
- [ ] Location returns real coordinates (enable location services on PC/phone)
- [ ] Unsplash images visible in list (needs network)

### Not implemented (optional or out of scope)

- [ ] Interactive map with pins
- [ ] Real nearby POI API (Google Places, Foursquare, etc.)
- [ ] User login / cloud sync / real backend
- [ ] Pick photo from gallery (only **camera** capture, no `PickPhotoAsync`)
- [ ] Offline image cache for remote `imageUrl`
- [ ] Unit / UI automated tests
- [ ] Page transition animations / full accessibility audit
- [ ] Shake gesture / accelerometer (optional extra hardware)
- [ ] Manual light/dark toggle (removed on purpose — system only)

---

## 6. GitHub — four phased commits (process)

Required by your brief; **not stored in repo yet** as a history you can show.

Suggested split:

| Phase | Suggested content | Status |
|-------|-------------------|--------|
| 1 | Shell, Home list, Recipe detail skeleton | [ ] Commit + push |
| 2 | TTS, camera, location | [ ] Commit + push |
| 3 | Theme (`AppThemeBinding`), styles, refresh, drinks | [ ] Commit + push |
| 4 | `recipes.json`, `ApiConfig`, fallback loader, fixes, README/TODO | [ ] Commit + push |

**Action:**

- [ ] Initialize git repo if needed, add `.gitignore` (exclude `bin/`, `obj/`)
- [ ] Push to GitHub with meaningful commit messages in English

---

## 7. Video demo checklist (ensure code path works)

Record slowly; grader may not run the app.

- [ ] App opens to recipe list with **images visible** (network on or local images added)
- [ ] Search + category **Drinks** shows beverage items
- [ ] Open detail → **Ingredients / Steps** tabs
- [ ] **Read aloud** → hear speech → **Stop**
- [ ] **Capture** → photo appears on detail page → visible under Profile → Cooking photos
- [ ] **Explore** → **Get GPS** → latitude & longitude update
- [ ] **Profile → Manage recipes** → Add → appears in list → Edit → Delete
- [ ] **Windows Settings → Dark mode** → app colors change
- [ ] (Optional) Profile → **Reload** → mention data source line on screen

---

## 8. Documentation files

| File | Purpose |
|------|---------|
| `README.md` | Project overview & run instructions — **update** if you change API URL or images |
| `TODO.md` | This checklist |

**Action:**

- [ ] After finishing API and images, update README “Not implemented” and Mock API sections with your real endpoint.

---

## 9. Copyright / course rules

- [ ] Confirm whether **generative AI** is allowed for code/XAML in your official brief (class discussion mentioned mixed guidance).
- [ ] If using Unsplash URLs, comply with [Unsplash License](https://unsplash.com/license) (attribution appreciated in report).
- [ ] If using Stitch or other UI generators, confirm IP/policy with instructor.

---

## 10. Priority order (recommended)

1. **Fix or bundle recipe images** (biggest visual impact for video).  
2. **Create and wire your Mock API** + test Reload.  
3. **Remove/fix `dotnet_bot.png` csproj reference**.  
4. **Run full demo path** on Android or Windows once.  
5. **Four Git commits** with clear messages.  
6. Optional: re-enable map + Google API key if you want extra polish.

---

*Generated for FoodMoment project — align with `README.md` and in-repo code as of the latest implementation.*
