# Copilot Instructions for this Repository

## Build, test, and lint

This Unity project does **not** define repository-level build/lint scripts (no CI workflow, no custom build script, no linter config).

### Build

- Use Unity Editor build settings (`File -> Build Settings`) with scene `Assets/Scenes/SampleScene.unity`.
- If you add automated builds later, prefer a dedicated static build entry method and call it via Unity batchmode `-executeMethod`.

### Test (Unity Test Framework is installed)

- Run all EditMode tests:
  ```powershell
  "<UNITY_EDITOR_PATH>\Unity.exe" -batchmode -quit -projectPath "E:\.useful\code\unity\GameJamTest" -runTests -testPlatform editmode -testResults "TestResults\editmode.xml"
  ```
- Run a single test (or test class) by filter:
  ```powershell
  "<UNITY_EDITOR_PATH>\Unity.exe" -batchmode -quit -projectPath "E:\.useful\code\unity\GameJamTest" -runTests -testPlatform editmode -testFilter "Namespace.ClassName.TestName" -testResults "TestResults\single.xml"
  ```

> There are currently no committed `Assets/**/Tests` test files in this repository.

### Lint

- No lint command is configured in the repo today.

## High-level architecture

- **Map bootstrap and board topology**: `PlotManageSystem` is the central runtime entry point for map generation. It reads `MapPlotConfigData` (`ScriptableObject`), instantiates `PlotView` prefabs by `PlotTypeEnum`, assigns grid coordinates, and provides neighbor lookup with a fixed 6-direction order (`left-up, right-up, right, right-down, left-down, left`).
- **Tile domain model**: `PlotView` stores per-tile runtime state (`isDetected`, `isSelected`, productions, improvements, monsters, special rewards) and applies/clears effects from `EventView`.
- **Event-driven interaction layer**: `EventCenter` is a global string-keyed event bus (`UnityAction` based). Input scripts (`CameraClick`, `CameraController`) publish events; gameplay/view scripts subscribe and react.
- **Extensibility points**:
  - `EventView` is an abstract event behavior contract (`ApplyEvent`/`ResolveEvent`).
  - `ImprovementView` is an abstract improvement contract returning `TotalProductionData`; `TestImprovement` demonstrates neighborhood-dependent production.
- **Data assets**: gameplay configuration is ScriptableObject-driven (`MapPlotConfigData`, `ProductData`, map/product `.asset` files under `Assets/Scripts/Data/**`).
- **UI layer**: `UIMgr` loads panel prefabs from `Resources/UI/<PanelTypeName>` and manages lifecycle; `BasePanel` handles fade in/out through `CanvasGroup`.

## Key conventions in this codebase

- **Event names are string literals (mostly Chinese text)** and must match exactly across publishers/subscribers (examples: `"鼠标悬停进入地块"`, `"事件生效"`). Keep payload type consistent with listener generic type.
- **Manager-style singletons** use shared bases:
  - `SingletonBaseWithMono<T>` for `MonoBehaviour` managers (auto-create + `DontDestroyOnLoad`)
  - `SingletonBaseWithoutMono<T>` for pure C# singleton utilities
- **Panel naming convention is strict**: panel class name == prefab name under `Resources/UI/`; `UIMgr.ShowPanel<T>()` depends on this.
- **Hex-grid coordinate assumptions are fixed**: `x/y` are zero-based grid coordinates stored on `PlotView`; neighbor order from `GetNearbyPlots` is relied upon by improvement logic.
- **Config-first workflow**: new plot/product content is expected as ScriptableObject assets rather than hardcoded constants.
