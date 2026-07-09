# Windows AI Trivia: Development Guide

How to build, test, and troubleshoot the .NET/WinUI version of AI Trivia.

## Part 1: Setting Up the Project

### 1.1 Prerequisites

Install these tools:

- **.NET 8.0 SDK** → https://dotnet.microsoft.com/download
- **Visual Studio 2022** (Community or Pro) with:
  - Desktop development with C#
  - Windows development tools
  - .NET desktop development
- **Windows 10 Build 19041+** or Windows 11

**Verify installation:**
```bash
dotnet --version          # Should show 8.0.x
dotnet workload list      # Should include windows, maui
```

### 1.2 Project Structure

The WinUI project uses this layout:

```
AITrivia.WinUI/
├── App.xaml                    # App-level resources and startup
├── App.xaml.cs
├── MainWindow.xaml             # Shell window (all pages switch inside)
├── MainWindow.xaml.cs
├── Views/                      # Page controls
│   ├── TitleScreenPage.xaml
│   ├── CountdownPage.xaml
│   ├── QuestionPage.xaml
│   └── GameOverPage.xaml
├── Controls/                   # Reusable UI components
│   ├── SettingsControl.xaml
│   ├── AboutControl.xaml
│   ├── ParticleCanvas.cs       # Custom particle effects
│   └── AnswerButton.cs
├── ViewModels/
│   └── GameViewModel.cs        # Game logic, timer, scoring
├── Models/
│   ├── GameSettings.cs         # Settings + persistence
│   ├── TriviaBank.cs           # Question loading
│   └── TriviaQuestion.cs
├── AITrivia.WinUI.csproj       # Build configuration
├── GlobalUsings.cs             # Global using statements
├── AppLog.cs                   # Structured logging
└── app.manifest                # Windows app metadata
```

### 1.3 Building the Project

**From command line:**
```bash
cd AITrivia.WinUI

# Restore NuGet dependencies
dotnet restore

# Build (default x64 debug)
dotnet build

# Run directly
dotnet run
```

**From Visual Studio:**
1. Open `AITrivia.WinUI.sln` (if it exists) or the folder
2. Select `AITrivia.WinUI` as startup project
3. Press F5 to debug

**Architecture options:**
```bash
# Build for specific architecture
dotnet build -p:Platform=x64      # 64-bit (default)
dotnet build -p:Platform=x86      # 32-bit
dotnet build -p:Platform=arm64    # ARM64 (Windows on ARM)
```

---

## Part 2: Key Technologies

### 2.1 WinUI 3 and XAML

**XAML** is the markup language for UI. Example:

```xml
<StackPanel Spacing="10">
    <TextBlock Text="Score" FontSize="20"/>
    <Button Content="Start Game" Click="StartGame_Click"/>
</StackPanel>
```

**Key concepts:**
- **Bindings** connect UI to ViewModel properties
- **Controls** are reusable UI components
- **Pages** are full screens within MainWindow
- **Resources** define colors, styles, templates

**Sample binding:**
```xml
<!-- XAML -->
<TextBlock Text="{Binding Score}" FontSize="24"/>

<!-- ViewModel -->
[ObservableProperty] private int _score = 0;
```

### 2.2 MVVM Toolkit

We use `CommunityToolkit.Mvvm` for reactive binding. It auto-generates boilerplate:

```csharp
// Declare observable properties with [ObservableProperty]
[ObservableProperty] private double _timeRemaining = 10.0;

// Automatically generates: TimeRemaining property + PropertyChanged events
// Access via: gameViewModel.TimeRemaining

// Commands for button clicks:
[RelayCommand]
private void StartGame()
{
    Phase = GamePhase.Question;
}
```

This is vastly cleaner than manually implementing `INotifyPropertyChanged`.

### 2.3 Application Lifecycle

**Entry point:**
```csharp
public partial class App : Application
{
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        GameSettings = new GameSettings();      // Load settings
        GameViewModel = new GameViewModel();    // Create game logic
        
        _window = new MainWindow();
        _window.Activate();
    }
}
```

**Error handling:**
```csharp
UnhandledException += OnUnhandledException;
AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
```

This catches:
- Unhandled XAML/UI exceptions
- Background task exceptions
- Unobserved Task exceptions

---

## Part 3: Testing Approach

### 3.1 Manual Testing

**UI interaction checklist:**
1. [ ] Buttons are clickable and respond
2. [ ] Page transitions are smooth
3. [ ] Timer counts down correctly
4. [ ] Score updates in real-time
5. [ ] Settings dialog opens/closes
6. [ ] Settings apply immediately to next question

**Testing workflow:**
```bash
dotnet run                    # Start the app
# Manually click through gameplay
```

### 3.2 Debugging in Visual Studio

**Breakpoints:**
1. Click in the margin next to a line
2. Press F5 to run under debugger
3. App pauses at breakpoints
4. Inspect variables in Debug pane

**Common debugger commands:**
- F10 — Step over (execute current line)
- F11 — Step into (dive into function calls)
- Shift+F11 — Step out (exit function)
- Ctrl+Alt+B — Breakpoints window
- Ctrl+Alt+W — Watch window (monitor variable changes)

### 3.3 Common Issues & Fixes

#### Issue: "Project file is missing"

**Symptom:** Error opening project in Visual Studio

**Solution:**
```bash
# Make sure you're in the right directory
cd AITrivia.WinUI
dotnet sln add AITrivia.WinUI.csproj
```

#### Issue: "NuGet packages not restoring"

**Symptom:** Missing dependencies, build fails

**Root cause:** Offline or corrupted NuGet cache

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Rebuild
dotnet build
```

#### Issue: "trivia_questions.json not found"

**Symptom:** Runtime error: File not found at `Assets\trivia_questions.json`

**Root cause:** The project file links to `../trivia_questions.json`, but it's not in `Assets/`

**How we fixed it:**
```xml
<!-- In AITrivia.WinUI.csproj -->
<ItemGroup>
    <Content Include="..\trivia_questions.json" 
              Link="Assets\trivia_questions.json">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
</ItemGroup>
```

This copies the JSON to output directory during build.

#### Issue: "App crashes on launch"

**Symptom:** App starts but immediately closes

**How to diagnose:**
```bash
# Run with verbose logging
dotnet run --verbose
```

Then check `AppLog.cs` output—it logs every major step:
```csharp
AppLog.Info("OnLaunched: begin");
AppLog.Info("OnLaunched: GameSettings created");
AppLog.Info("OnLaunched: GameViewModel created");
```

#### Issue: "Settings not persisting"

**Symptom:** Changed settings, closed app, reopened—settings were reset

**Root cause:** Same as macOS—not saving to disk on every change

**Solution:**
```csharp
[ObservableProperty]
private double _questionTime = 10.0;

partial void OnQuestionTimeChanged(double value)
{
    Save();  // Auto-save when property changes
}
```

#### Issue: "XAML Parse Exception"

**Symptom:** Error at line 42 of QuestionPage.xaml—"Type 'xxx' not found"

**Root cause:** Typo in XAML or missing namespace

**Solution:**
```xml
<!-- Check xmlns declarations at top of file -->
<Page
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:AITrivia.Controls"
    />

<!-- Make sure custom controls are properly referenced -->
<local:SettingsControl/>   <!-- Correct -->
<!-- NOT: <SettingsControl/> -->
```

---

## Part 4: Building for Release

### 4.1 Release Build

```bash
# Optimized binary (much faster than debug)
dotnet build -c Release

# Binary location: bin\Release\net8.0-windows10.0.19041.0\
```

**Why release build matters:**
- JIT compiler optimizations
- No debug symbols (smaller file)
- Better runtime performance
- Closer to what users will experience

### 4.2 Self-Contained Deployment

```bash
# Include .NET runtime in the package
dotnet publish -c Release -f net8.0-windows10.0.19041.0 \
    -p:PublishSelfContained=true \
    -p:RuntimeIdentifier=win-x64
```

This creates a folder that runs **without installing .NET**.

### 4.3 Creating MSIX Installer

MSIX is Microsoft's modern app installer format. Steps:

1. **Enable MSIX tooling** in `.csproj`:
```xml
<EnableMsixTooling>true</EnableMsixTooling>
```

2. **Create AppPackages project** in Visual Studio:
   - Right-click → Packaging Options
   - Check "Create app package"
   
3. **Build MSIX**:
```bash
dotnet publish -c Release --self-contained \
    -p:GenerateAppxPackageOnBuild=true
```

Result: `bin\Release\net8.0-windows10.0.19041.0\AppPackages\AITrivia_1.0.0.0_x64.msix`

Users double-click to install from Microsoft Store or sideload.

---

## Part 5: Architecture Details

### 5.1 Page Navigation

Games use a single MainWindow with pages that swap inside:

```csharp
// GameViewModel controls which page shows
[ObservableProperty]
private GamePhase _phase = GamePhase.TitleScreen;

// MainWindow listens to phase changes
private void OnPhaseChanged(object sender, PropertyChangedEventArgs e)
{
    switch (App.GameViewModel.Phase)
    {
        case GamePhase.TitleScreen:
            MainFrame.Navigate(typeof(TitleScreenPage));
            break;
        case GamePhase.Question:
            MainFrame.Navigate(typeof(QuestionPage));
            break;
        // ...
    }
}
```

### 5.2 Timers

WinUI uses `DispatcherTimer` (runs on UI thread):

```csharp
private DispatcherTimer _questionTimer;

private void StartTimer()
{
    _questionTimer = new DispatcherTimer();
    _questionTimer.Interval = TimeSpan.FromMilliseconds(50);
    _questionTimer.Tick += (s, e) =>
    {
        TimeRemaining -= 0.05;
        if (TimeRemaining <= 0)
        {
            TimeExpired();
        }
    };
    _questionTimer.Start();
}
```

**Why `DispatcherTimer`?**
- Runs on UI thread (safe to update UI directly)
- Automatic cleanup when window closes
- No threading issues

### 5.3 Settings Persistence

Similar to macOS but uses Windows paths:

```csharp
// macOS: ~/.config/aitrivia/config.json
// Windows: %USERPROFILE%\.config\aitrivia\config.json

private static readonly string ConfigDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
    ".config", "aitrivia");

private void Save()
{
    Directory.CreateDirectory(ConfigDir);
    File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(data));
}

private void Load()
{
    if (File.Exists(ConfigFile))
    {
        var data = JsonConvert.DeserializeObject(File.ReadAllText(ConfigFile));
        // Apply settings...
    }
}
```

### 5.4 Logging

Custom `AppLog` class centralizes logging:

```csharp
public static class AppLog
{
    public static string LogPath { get; private set; }

    public static void Info(string message)
    {
        var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [INFO] {message}";
        File.AppendAllText(LogPath, logEntry + Environment.NewLine);
        System.Diagnostics.Debug.WriteLine(logEntry);
    }

    public static void Error(string message, Exception ex)
    {
        var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [ERROR] {message}\n{ex}";
        File.AppendAllText(LogPath, logEntry + Environment.NewLine);
    }
}
```

**Log file location:** `%APPDATA%\AITrivia\app.log`

---

## Part 6: Performance Tuning

### 6.1 Measuring Build Time

```bash
# Time the build
Measure-Command { dotnet build }

# Profile: clean build usually takes 15-30s
# Incremental (after small changes): 2-5s
```

### 6.2 Profiling Runtime Performance

Use Visual Studio's Diagnostic Tools (Debug → Profiler):

1. Start debugging (F5)
2. Debug → Windows → Show Diagnostic Tools
3. Watch CPU, memory, UI thread time
4. Look for:
   - Long frames (jank = > 16ms per frame @ 60fps)
   - Memory leaks (growing memory over time)
   - UI thread blocking

### 6.3 Common Performance Pitfalls

**Blocking UI thread:**
```csharp
// BAD: Blocks UI for 2 seconds
System.Threading.Thread.Sleep(2000);

// GOOD: Use async/await
await Task.Delay(2000);
```

**Excessive property change notifications:**
```csharp
// BAD: Fires PropertyChanged for every item
foreach (var item in items)
    Items.Add(item);  // 1000 PropertyChanged events

// GOOD: Batch updates
Items.AddRange(items);  // 1 event
```

---

## Part 7: Debugging Commands Reference

```csharp
// Print to Debug Output pane
System.Diagnostics.Debug.WriteLine($"Score: {gameViewModel.Score}");

// Conditional breakpoints
if (score > 1000) { }  // Breakpoint on this line, break only if condition true

// Immediate window (Ctrl+Alt+I)
gameViewModel.Score = 5000  // Modify at runtime
```

**Common WinUI-specific debugging:**
```xml
<!-- Add debug attributes to XAML -->
<Page d:DesignHeight="400" d:DesignWidth="800">
    <!-- 'd:' namespace is design-time only, ignored at runtime -->
</Page>

<!-- Check binding errors in Output pane -->
<!-- Binding failed: 'Score' on type 'GameViewModel' -->
```

---

## Part 8: Cross-Platform Considerations

### 8.1 File Paths

**Different per platform, but code should be path-agnostic:**

```csharp
// GOOD: Use Path.Combine (works on Windows and macOS/Linux)
string configFile = Path.Combine(configDir, "config.json");

// BAD: Hardcoded backslashes (breaks on macOS)
// string configFile = "C:\\Users\\...\\config.json";
```

### 8.2 Environment Variables

Windows and macOS use different special folders:

```csharp
// macOS: $HOME
// Windows: %USERPROFILE%
string home = Environment.GetFolderPath(
    Environment.SpecialFolder.UserProfile);

// macOS: $HOME/Library/Application Support
// Windows: %APPDATA%
string appData = Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData);
```

### 8.3 Keyboard Shortcuts

- **macOS:** Cmd+, (Command+Comma)
- **Windows:** Ctrl+, (Control+Comma)

**Platform detection:**
```csharp
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    helpText = "Press Ctrl+, for settings";
else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    helpText = "Press Cmd+, for settings";
```

---

## Part 9: Known Limitations & Future Work

### 9.1 Current Limitations

- **No multiplayer:** Single-player only
- **No cloud sync:** Settings only save locally
- **No accessibility features:** Could improve screen reader support
- **No dark mode toggle:** Uses system setting
- **No question editor:** Questions are read-only JSON

### 9.2 Future Enhancement Ideas

- [ ] Add MVVM framework (if complexity grows)
- [ ] Telemetry/analytics dashboard
- [ ] Custom question importer
- [ ] Leaderboard sync to cloud
- [ ] Accessibility audit and improvements
- [ ] Unit tests for game logic
- [ ] Integration tests for UI
- [ ] GitHub Actions CI/CD pipeline
- [ ] Auto-updates via MSIX

---

## Part 10: Troubleshooting Decision Tree

**"My build is failing"**
→ Run `dotnet restore` first
→ Check .csproj for typos
→ Try `dotnet clean && dotnet build`

**"App won't start"**
→ Check `AppLog.Info` messages for where it crashes
→ Verify `..\trivia_questions.json` exists relative to .csproj
→ Try running from Visual Studio debugger to see exact error

**"Settings not saving"**
→ Verify `AppLog` shows Save() calls
→ Check `%USERPROFILE%\.config\aitrivia\config.json` exists
→ Ensure directory has write permissions

**"UI looks wrong or doesn't update"**
→ Check if property is `[ObservableProperty]`
→ Verify XAML Binding uses correct property name
→ Look for XAML Parse exceptions in Output pane

**"Game logic is wrong"**
→ Review `GameViewModel.cs` logic
→ Add AppLog.Info() statements to trace execution
→ Use Visual Studio debugger to step through calculations

---

## Summary

Windows development for AI Trivia uses:
1. **C#** for application code
2. **XAML** for UI markup
3. **WinUI 3** for UI framework
4. **MVVM Toolkit** for reactive bindings
5. **.NET 8.0** runtime
6. **Visual Studio 2022** for development

The architecture is similar to macOS version but uses platform-specific technologies. Following this guide should help you build, test, package, and troubleshoot successfully!
