# Repository Guidelines

## Project Structure & Module Organization

The solution is `Source/DevelopTools.sln`, containing the .NET 8 WPF application in `Source/DevelopToolsCore/`. UI definitions live in `App.xaml` and `MainWindow.xaml`; code-behind should remain limited to view-specific behavior. Application logic follows an MVVM-style layout: `ViewModels/` coordinates UI behavior, `Models/` holds Revit add-in data, and `Infrastructure/` contains reusable binding and command helpers. Localized resource dictionaries are under `Languages/`. Embedded RevitLookup and AddInManager binaries are stored in `Resources/`, while external compile-time dependencies are in `ThirdParty/`. Documentation images belong in `Documents/Images/`.

## Build, Test, and Development Commands

Run commands from the repository root on Windows:

```powershell
dotnet restore Source/DevelopTools.sln
dotnet build Source/DevelopTools.sln -c Debug
dotnet run --project Source/DevelopToolsCore/DevelopToolsCore.csproj
dotnet build Source/DevelopTools.sln -c Release
```

`restore` downloads NuGet dependencies, `build` verifies compilation, and `run` launches the desktop application. The project targets `net8.0-windows8.0`, so use a current .NET 8 SDK on Windows.

## Coding Style & Naming Conventions

Use four-space indentation in C# and XAML. Follow existing C# conventions: PascalCase for types, methods, commands, and public properties; camelCase for parameters and locals; `_camelCase` for private fields. Keep nullable-reference warnings enabled and prefer clear MVVM bindings over UI logic in code-behind. When adding visible text, add matching keys to both `Languages/en-US.xaml` and `Languages/zh-CN.xaml`. Do not edit generated `Properties/Resources.Designer.cs` manually.

## Testing Guidelines

No automated test project currently exists. At minimum, run a Debug build and launch the application. Manually verify Revit version discovery, language switching, selection state, and add-in installation. Test current-user installation before all-users installation, which may require elevation. If adding tests, place them under `Tests/DevelopToolsCore.Tests/` and name files `TypeNameTests.cs`.

## Commit & Pull Request Guidelines

History uses short, imperative summaries such as `fix resource key` and `add app icon`; keep commits focused and similarly concise. Pull requests should explain the user-visible change, list validation performed, and link relevant issues. Include before/after screenshots for XAML or localization changes. Call out changes to bundled DLLs, supported Revit versions, or installation paths explicitly.
