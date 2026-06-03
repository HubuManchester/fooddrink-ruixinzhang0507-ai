# FoodMoment — 录屏讲解脚本 / Screencast Script

**目标时长 Target duration:** 10–15 分钟  
**建议分辨率:** 1920×1080，系统字体放大，关闭无关通知  
**上传:** mmutube → 嵌入 Moodle

---

## 时间分配建议 Suggested timing

| 段落 Section | 分钟 Min |
|--------------|----------|
| 1 开场 Opening | 0:30 |
| 2 UI/UX 与无障碍 | 2:00 |
| 3 移动硬件 | 3:00 |
| 4 功能演示 | 3:00 |
| 5 验证与错误处理 | 2:00 |
| 6 数据与部署 | 1:30 |
| 7 代码质量与 GitHub（口头） | 1:30 |
| 8 结尾 Closing | 0:30 |

---

## 1. 开场 Opening（约 0:30）

**中文：**  
大家好，我是 Ruixin Zhang。这段录屏展示我的 .NET MAUI 应用 **FoodMoment**——一个饮食食谱应用，支持 Windows 与 Android。导师不会本地运行我的代码，因此我会在画面中完整演示各项功能，并说明它们如何对应评分标准：UI/UX 与无障碍、移动硬件、功能、验证与错误处理、部署，以及代码质量与 GitHub 使用情况。

**English:**  
Hello, I am Ruixin Zhang. This screencast demonstrates my .NET MAUI app **FoodMoment**, a food and drink recipe companion that targets **Windows** and **Android**. As assessors will not run the project locally, I will show every feature on screen and explain how each one maps to the rubric: UI/UX and accessibility, mobile hardware, functionality, validation and error handling, deployment, and my code quality and GitHub practices.

**[DEMO]** 显示已启动的应用主界面 / Show the running app on Home tab.

---

## 2. UI/UX 与无障碍 UI/UX & Accessibility（约 2:00）

**中文：**  
在 UI/UX 方面，我使用 Shell 底部 TabBar 组织三个主页面：Browse、Explore、Profile，减少用户迷路。列表采用卡片式布局、圆角与阴影，并在 `Styles.xaml` 和 `Colors.xaml` 中集中定义样式。应用通过 `AppThemeBinding` 跟随系统深浅色；在 Profile 还可手动选择 Light、Dark 或 System。加载数据时显示 `ActivityIndicator`，列表支持下拉刷新。无障碍方面，收藏按钮设置了 `SemanticProperties.Hint`（如 “Toggle favorite”），按钮最小点击区域约 44pt，便于读屏与触控。

**English:**  
For UI/UX, I use a Shell **TabBar** with three main areas: Browse, Explore, and Profile, so navigation stays predictable. The recipe list uses card layout, rounded corners, and shadows, with shared styles in `Styles.xaml` and `Colors.xaml`. The app follows system light/dark mode via `AppThemeBinding`, and Profile adds a manual **Light / Dark / System** theme picker. An `ActivityIndicator` appears while data loads, and the home list supports **pull-to-refresh**. For accessibility, favorite buttons expose `SemanticProperties.Hint` (e.g. “Toggle favorite”), and controls meet a minimum touch target for screen readers and touch input.

**[DEMO]**  
1. 切换 Windows 深色模式，展示列表与详情颜色变化。  
2. Profile → 切换 Theme。  
3. Home 下拉刷新。  
4. 将鼠标悬停在收藏 ♡ 按钮上（若用 Narrator，可简短演示 Hint）。

---

## 3. 移动硬件 Mobile hardware（约 3:00）

**中文：**  
本应用使用了 **四种** 不同的移动/设备能力，超过评分表“至少四种硬件”的要求。第一，**文字转语音（TTS）**：在食谱详情页的 Steps 标签下，Read aloud 会朗读当前步骤，Stop 可取消朗读。第二，**相机**：Capture 调用摄像头拍照，照片保存在应用数据目录并显示在详情页，同时写入烹饪记录。第三，**GPS 定位**：Explore 页点击 Get GPS 获取经纬度，并尝试逆地理编码显示地址。第四，**触觉反馈（Haptic）**：收藏、拍照成功、定位更新时会触发轻微震动（在支持的设备上）。未实现摇一摇/加速度计，因作业中为可选项。

**English:**  
The app uses **four distinct hardware/device capabilities**, above the rubric threshold of four. First, **text-to-speech (TTS)**: on the recipe detail page under Steps, **Read aloud** speaks the current step and **Stop** cancels speech. Second, the **camera**: **Capture** takes a photo, saves it under app data, shows it on the detail page, and records it in cook history. Third, **GPS**: on Explore, **Get GPS** reads latitude and longitude and attempts reverse geocoding for an address. Fourth, **haptic feedback** on favorite, successful capture, and location update where supported. Shake/accelerometer is not implemented as it was optional for this coursework.

**[DEMO]**  
1. 打开任意食谱 → Steps → **Read aloud** → **Stop**。  
2. **Capture** → 展示 “Your dish photo” 预览。  
3. Explore → **Get GPS**（事先在系统设置中开启定位权限）。  
4. 简要说明：收藏时设备可能有触觉反馈（真机或模拟器上可提一句）。

---

## 4. 功能 Functionality（约 3:00）

**中文：**  
功能上，应用提供完整的食谱浏览与本地管理。Home 支持 **搜索**、**分类筛选**（含 Drinks、Lunch、Dessert 等）和 **收藏**。详情页展示食材、步骤、营养信息（卡路里与宏量营养素）。Profile 可 **Reload data** 查看数据来源（EmbeddedJson / LocalStaticJson / HardcodedFallback），进入 **Manage recipes** 进行 **增删改查（CRUD）**。所有列表缩略图使用 `Resources/Images/` 下的本地 JPG，**无需联网** 即可演示。共 14 条内置食谱，数据持久化在 SQLite 中。

**English:**  
For functionality, the app offers full recipe browsing and local management. **Home** supports **search**, **category filters** (including Drinks, Lunch, Dessert), and **favorites**. The detail page shows ingredients, steps, and **nutrition** (calories and macros). **Profile** can **Reload data** to show the load source (EmbeddedJson, LocalStaticJson, or HardcodedFallback) and open **Manage recipes** for full **CRUD**. Thumbnails use bundled JPGs under `Resources/Images/`, so the demo works **offline**. There are 14 seed recipes with **SQLite** persistence.

**[DEMO]**  
1. 搜索 “matcha” 或筛选 Drinks。  
2. 点开详情 → 切换 Ingredients / Steps → 指出营养面板。  
3. Profile → Reload → 读出来源一行文字。  
4. Manage recipes → Add 或 Edit 一条 → Save → 回到列表确认。  
5. 断网（可选）：说明图片仍显示。

---

## 5. 验证与错误处理 Validation & error handling（约 2:00）

**中文：**  
验证与错误处理贯穿多处。添加/编辑食谱时，`RecipeEditViewModel.ValidateInput` 会检查标题、描述、分类和至少一步骤；不通过则弹出 **DisplayAlert**，应用不会崩溃。数据加载使用 `RecipeDataLoaderService` 的 **try-catch 链**：主 JSON 失败则读备用 JSON，再失败则使用硬编码目录。相机由 `CameraPhotoService` 捕获权限异常与设备不支持情况，并向用户显示提示；Windows 上还有 CameraCaptureUI 回退。定位若拒绝权限，Explore 页会显示 “Location permission denied” 而不是崩溃。

**English:**  
Validation and error handling appear in several layers. When adding or editing a recipe, `ValidateInput` checks title, description, category, and at least one step; failures show a **DisplayAlert** without crashing. Data loading uses a **try-catch chain** in `RecipeDataLoaderService`: primary JSON, then fallback JSON, then a hardcoded catalog. The camera service handles permission and unsupported-device errors with user messages and a Windows camera UI fallback. If location permission is denied, Explore shows a clear message instead of crashing.

**[DEMO]**  
1. Manage recipes → Add → **故意留空标题** → Save → 展示验证弹窗。  
2. （可选）关闭定位权限 → Get GPS → 展示拒绝提示。  
3. （可选）口述相机权限/失败时的提示文案，不必真关掉摄像头。

---

## 6. 数据回退与跨平台部署 Data fallback & deployment（约 1:30）

**中文：**  
数据方面采用“双保险”本地策略：嵌入的 `recipes.json`、备用的 `recipes_fallback.json` 和 `StaticRecipeCatalog`，无需 mockapi.io。部署上，项目在 **Windows** 与 **Android** 两个目标框架下构建；README 中有 `dotnet build` 命令和 `verify-demo.ps1` 脚本。本录屏在 Windows 上运行，同时说明 Android 项目文件与权限已在 `AndroidManifest` 中配置，可在 Visual Studio 切换到 Android 模拟器或真机运行同一套代码。

**English:**  
Data uses a local **dual-insurance** strategy: embedded `recipes.json`, `recipes_fallback.json`, and `StaticRecipeCatalog`, without mockapi.io. For deployment, the solution builds for **Windows** and **Android**; the README documents build commands and `verify-demo.ps1`. This recording runs on **Windows**, and I note that Android manifests and permissions are configured so the same codebase can run on an emulator or device from Visual Studio.

**[DEMO]**  
1. Profile → Reload → 显示 **EmbeddedJson**。  
2. 快速展示 VS 启动配置为 **Windows Machine**（或 Android 配置截图 3 秒）。  
3. （可选）打开 `FoodMoment.csproj` 显示双 TargetFramework 一行。

---

## 7. 代码质量与 GitHub（口头，约 1:30）

**中文：**  
代码质量方面，我采用 **MVVM**：Views 与 ViewModels 分离，业务逻辑放在 Services（如 `RecipeRepository`、`CameraPhotoService`）。通过接口与 `MauiProgram` 依赖注入便于测试与复用。命名遵循 C# 惯例，关键服务带有 XML 注释。GitHub 上，我将工作分为多次有意义的提交，例如：初始 MAUI 应用、相机与本地图片、英文 README、README 架构与排错说明；提交信息为英文，作者为 Ruixin Zhang，仓库链接在 README 中。未使用 Cursor 共创署名。

**English:**  
For code quality, I use **MVVM**: Views and ViewModels are separated, with business logic in **Services** such as `RecipeRepository` and `CameraPhotoService`. Interfaces and DI in `MauiProgram` improve reuse and testability. Naming follows C# conventions, with XML comments on key services. On **GitHub**, I split work into meaningful commits—for example the initial MAUI app, camera and bundled images, English README, and README architecture/troubleshooting—with clear English messages under Ruixin Zhang, linked from the README.

**[DEMO]**  
切到浏览器 GitHub 仓库 → Commits 页滚动展示 3–5 条提交（约 20 秒）。可打开 README 英文版一眼。

---

## 8. 结尾 Closing（约 0:30）

**中文：**  
以上演示了 FoodMoment 如何满足 UI/UX、四种硬件、功能、验证、双平台部署以及代码与 GitHub 规范。所有缩略图离线可用，硬件功能均可在画面中看到。感谢观看。

**English:**  
That concludes how FoodMoment meets the rubric for UI/UX, four hardware features, functionality, validation, cross-platform deployment, and code/GitHub practices. All thumbnails work offline, and hardware features were shown on screen. Thank you for watching.

**[DEMO]** 回到 Home 页或显示应用 Logo / Return to Home or app title screen.

---

## 录屏前检查清单 Pre-recording checklist

- [ ] Windows 已安装 TTS 语音包  
- [ ] 相机 / 定位权限已允许  
- [ ] 应用已 Reload，列表约 14 条食谱  
- [ ] 关闭邮件、Teams 等通知  
- [ ] VS 调试栏可隐藏，仅留应用窗口  
- [ ] 麦克风试录 10 秒确认音量  
- [ ] mmutube 账号可登录  

---

## 提交提醒 Submission reminder

1. 代码已推送到 GitHub classroom 仓库。  
2. 录屏 10–15 分钟，上传 **mmutube**。  
3. 在 Moodle 提交区 **嵌入** 视频链接（不要只交超大文件）。  
4. 未提交录屏 → 成绩可能为 **0 分**（评分表明确要求）。

---

*脚本版本：与 master 分支功能一致（TTS / Camera / GPS / Haptics / 14 recipes / English README）。*
