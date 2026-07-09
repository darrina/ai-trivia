# AI Trivia: Development Guide

How I built, tested, and troubleshot this native macOS SwiftUI trivia game.

## Part 1: Building the Foundation

### 1.1 Setting Up the Swift Package

**What I did:**
- Created a Swift Package Manager (SPM) project structure using `swift package`
- Organized source files into logical directories: `Models/`, `Views/`, `ViewModels/`
- Created a `Resources/` folder for the trivia questions JSON

**Why this approach:**
- SPM is lightweight and doesn't require Xcode project overhead
- Directory structure keeps code organized and maintainable
- Resources folder ensures data files are bundled correctly

**Build command:**
```bash
cd "AI Trivia"
swift build                # Debug build
swift build -c release     # Optimized release build
```

### 1.2 Understanding the App Structure

**Core files:**
- `AITriviaApp.swift` — Main entry point, app delegate, menu bar configuration
- `GameViewModel.swift` — Game state, logic, timer management
- `GameSettings.swift` — User preferences, persistence
- Views (ContentView, TitleScreenView, QuestionView, etc.)

**Key architectural decision:**
I used SwiftUI's `@Observable` macro (iOS 17+) instead of `@StateObject` for simpler reactive state management. Settings changes automatically propagate through the UI without manual bindings.

---

## Part 2: Testing Approach

### 2.1 Manual Testing During Development

**What I tested:**
1. **UI Interactions** — Did buttons respond? Did overlays appear/dismiss correctly?
2. **State Transitions** — Title → Countdown → Question → Reveal → GameOver → Title
3. **Timer Logic** — Did the 10-second timer count down? Did it trigger timeouts?
4. **Scoring Calculations** — Did points increase with speed/difficulty/combos?
5. **Settings Application** — Did changing the question timer actually change timer behavior?

**Testing workflow:**
```bash
# Build and run
swift run

# Or run the compiled binary directly
./.build/debug/AITrivia
```

### 2.2 Problem Found: App Didn't Show Window

**Symptom:** `swift run` hung without displaying a window.

**Root cause:** The app wasn't properly activated as a GUI application. By default, command-line tools don't have focus on macOS.

**Solution:**
```swift
class AppDelegate: NSObject, NSApplicationDelegate {
    func applicationDidFinishLaunching(_ notification: Notification) {
        NSApp.setActivationPolicy(.regular)        // Register as GUI app
        NSApp.activate(ignoringOtherApps: true)    // Bring to foreground
    }
}
```

**Why this fixed it:** 
- `.setActivationPolicy(.regular)` tells the OS: "I'm a real application with a Dock icon"
- `.activate(ignoringOtherApps: true)` forcibly brings it to the foreground

### 2.3 Problem Found: Settings Window Dismissed the Entire App

**Symptom:** Pressing ESC or clicking the X closed the settings overlay, but also closed the entire app window.

**Root cause:** I used `@Environment(\.dismiss)` which dismisses the current view hierarchy—including the window itself in a SwiftUI app.

**Solution:**
```swift
// Instead of using @Environment(\.dismiss)
// Pass a closure callback:
struct SettingsView: View {
    var onClose: () -> Void
    
    Button("Close") {
        onClose()  // This just toggles @State in parent, doesn't dismiss window
    }
}
```

**Why this fixed it:** Callbacks let the parent ContentView control the overlay state, while `dismiss` operates on the view hierarchy above.

---

## Part 3: Building and Packaging

### 3.1 Creating a Release Build

```bash
# Optimized binary (~15MB vs 50MB for debug)
swift build -c release

# Binary location: .build/release/AITrivia
```

**Why release build matters:**
- Optimizations reduce file size and startup time
- Removes debug symbols
- Better performance during gameplay

### 3.2 Creating the .app Bundle Manually

macOS expects apps to be in `.app` bundles—folders with a specific structure. Since Swift Package doesn't create this automatically, I built it manually:

```bash
# Structure:
# AI Trivia.app/
# ├── Contents/
# │   ├── MacOS/
# │   │   └── AI Trivia          (the binary)
# │   ├── Resources/
# │   │   ├── AppIcon.icns       (icon)
# │   │   └── trivia_questions.json
# │   └── Info.plist             (metadata)
```

**Key part: Info.plist**
```xml
<key>CFBundleExecutable</key>
<string>AI Trivia</string>

<key>CFBundleIconFile</key>
<string>AppIcon</string>

<key>LSMinimumSystemVersion</key>
<string>14.0</string>
```

This tells macOS: "Run this binary, use this icon, require macOS 14+".

### 3.3 Creating the .pkg Installer

```bash
# 1. Create package root directory
mkdir -p pkg_root/Applications
cp -R "build/AI Trivia.app" pkg_root/Applications/

# 2. Build the .pkg
pkgbuild \
    --root pkg_root \
    --identifier "com.aitrivia.app" \
    --version "1.0" \
    --install-location "/" \
    ../AI\ Trivia.pkg
```

**Why this works:**
- `--root pkg_root` defines what gets installed
- `--install-location "/"` means install to `/` (then `Applications/` subdirectory)
- `.pkg` files are standard macOS installers—users double-click to install

---

## Part 4: Troubleshooting Guide

### 4.1 "Build Fails with Swift Compiler Error"

**Common cause:** Using a language feature not available in the target Swift version.

**How to check:** Look at `Package.swift`:
```swift
let package = Package(
    name: "AITrivia",
    platforms: [.macOS(.v14)],    // Minimum version
    // ...
)
```

**Solution:** Either:
- Use an older Swift API (e.g., `@State` instead of `@Observable` for macOS < 14)
- Bump the minimum deployment target

### 4.2 "Resources Not Found at Runtime"

**Symptom:** `Thread 1: Fatal error: trivia_questions.json not found in bundle`

**Root cause:** SPM's resource bundling requires correct configuration.

**How I fixed it:** In `Package.swift`:
```swift
targets: [
    .executableTarget(
        name: "AITrivia",
        path: "Sources",
        resources: [.process("Resources")]  // Process means: embed Resources/
    )
]
```

Then access it:
```swift
Bundle.module.url(forResource: "trivia_questions", withExtension: "json")
```

**To debug:** Check what's actually bundled:
```bash
# After building, inspect the binary bundle
cd .build/debug
ls -la AITrivia_AITrivia.bundle/

# See what's inside
find AITrivia_AITrivia.bundle/ -type f
```

### 4.3 "Settings Don't Persist"

**Symptom:** Closed the app, reopened it, settings were reset.

**Root cause:** I wasn't saving settings to disk.

**Solution:** Added save/load methods to `GameSettings`:
```swift
private var isSuppressingSave = false  // Prevent save-on-load cycles

var questionTime: Double = 10.0 {
    didSet { save() }  // Auto-save on every change
}

func save() {
    guard !isSuppressingSave else { return }
    let data = SettingsData(...)
    try JSONEncoder().encode(data)
    try data.write(to: configFile)
}

func load() {
    isSuppressingSave = true
    let json = try Data(contentsOf: configFile)
    let data = try JSONDecoder().decode(SettingsData.self, from: json)
    questionTime = data.questionTime  // This triggers didSet, but we suppress the save
    isSuppressingSave = false
}
```

**Why `isSuppressingSave`?** 
Without it, loading would trigger `didSet`, which would immediately save—causing unnecessary disk I/O and potential race conditions.

### 4.4 "Menu Items Not Working"

**Symptom:** ⌘, keyboard shortcut didn't open settings.

**Root cause:** Forgot to add `.commands` modifier to the app, or used wrong key modifiers.

**How I fixed it:**
```swift
WindowGroup { ... }
    .commands {
        CommandGroup(replacing: .appSettings) {
            Button("Settings...") { showSettings.toggle() }
                .keyboardShortcut(",", modifiers: .command)  // ⌘,
        }
    }
```

**Debugging commands:**
- Remove all `.commands` blocks temporarily
- Add them back one at a time
- Check that command groups don't conflict (only one `.appSettings`, one `.help`, etc.)

---

## Part 5: Performance Optimization

### 5.1 Measuring Build Time

```bash
time swift build           # See how long builds take
time swift build -c release
```

**What I observed:**
- Debug: ~30-40 seconds (first build), ~1-2 seconds (incremental)
- Release: ~20-30 seconds (optimizations take time)

**Optimization:** Only use release builds when packaging for distribution.

### 5.2 Profiling at Runtime

Use Xcode's Instruments to profile the running app:
```bash
# Option 1: Run in Xcode
open -a Xcode Package.swift

# Option 2: Run in Time Profiler manually
instruments -t "Time Profiler" .build/debug/AITrivia
```

**What I checked:**
- Does the timer cause jank? (No—timers run on background thread)
- Do particle animations stutter? (Minimal impact)
- Is question loading fast? (Yes—trivia questions are loaded once at startup)

---

## Part 6: Icon Generation

### 6.1 Generating from Code

Instead of relying on pre-made images, I generated the app icon programmatically:

```python
from PIL import Image, ImageDraw

# Create 1024x1024 base
img = Image.new('RGBA', (1024, 1024), (0, 0, 0, 0))
draw = ImageDraw.Draw(img)

# Draw neural network pattern
# Draw glowing nodes
# Draw connections
# Add text overlay

img.save('AppIcon.png')
```

**Why programmatic generation?**
- No external dependencies
- Easy to tweak colors/design
- Version-controllable (code, not binary)

### 6.2 Converting PNG to icns

macOS requires `.icns` (icon set) format. The process:

```bash
# Create iconset directory with multiple sizes
iconutil -c icns AppIcon.iconset/ -o AppIcon.icns
```

The `.icns` file contains optimized icons for every size (16px, 32px, 64px, 128px, 256px, 512px).

---

## Part 7: Deployment Checklist

Before releasing:

- [ ] **Test on a clean macOS install** (or at least another machine)
- [ ] **Verify menu bar works** (File, App menu, Settings, Help removed)
- [ ] **Check settings persist** across app restarts
- [ ] **Test keyboard shortcuts** (⌘, for settings, ESC to close overlays, ⌘Q to quit)
- [ ] **Verify .pkg installer works** (`installer -pkg AI\ Trivia.pkg -target /`)
- [ ] **Run release build** to catch any optimizations issues
- [ ] **Check icon displays** in Dock and Finder

---

## Part 8: Key Learnings

### 8.1 SwiftUI for macOS

**Strengths:**
- Same codebase could theoretically run on iOS
- Powerful animation and state management
- @Observable is much cleaner than @StateObject

**Gotchas:**
- `.dismiss` operates on the view stack, not just overlays
- `Bundle.module` requires correct SPM resource setup
- Some macOS-specific APIs (AppDelegate) still needed

### 8.2 Swift Package Manager

**Strengths:**
- Lightweight, no Xcode project bloat
- Fast iteration
- Clear build output

**Gotchas:**
- Manual .app bundle creation required (not automatic)
- Resource bundling can be finicky if misconfigured
- No built-in icon/build settings like Xcode

### 8.3 macOS App Development

**Strengths:**
- Native performance and UI feel
- Access to full macOS APIs
- Users expect modern features (menu bar, preferences, etc.)

**Gotchas:**
- Must handle window lifecycle correctly
- Settings should follow macOS conventions (⌘, for preferences)
- Installation via .pkg is standard

---

## Part 9: Debugging Commands Reference

```bash
# Check if app bundle is valid
codesign -v "AI Trivia.app"

# Extract strings from binary (find hardcoded values)
strings .build/debug/AITrivia | grep -i error

# Monitor app startup
log stream --predicate 'eventMessage contains "AITrivia"' --level debug

# Check macOS requirements
grep LSMinimumSystemVersion "AI Trivia.app/Contents/Info.plist"

# Verify .pkg contents
xar -tf AI\ Trivia.pkg | head -20

# Test .pkg installation
installer -pkg AI\ Trivia.pkg -target CurrentUserHomeDirectory -verbose

# Run binary with environment variables
SWIFT_DEBUG_LOG=1 ./.build/debug/AITrivia
```

---

## Part 10: Next Steps for Enhancement

If you wanted to extend this:

1. **Add Analytics** — Track which questions are hardest, most-skipped, etc.
2. **Leaderboards** — Save high scores locally, sync to cloud
3. **Daily Challenges** — Curated question sets that change daily
4. **Custom Themes** — Dark/light mode, color customization
5. **Accessibility** — VoiceOver support, high contrast mode
6. **Localization** — Translate questions to other languages
7. **Multiplayer** — Local network or online multiplayer rounds
8. **Export Stats** — Save game history as CSV

---

## Summary

The development process was:
1. **Build incrementally** — Get the basic UI working first
2. **Test continuously** — Run the app after every major change
3. **Troubleshoot systematically** — Check one thing at a time
4. **Optimize late** — First make it work, then make it fast
5. **Package professionally** — Use standard macOS formats (.pkg, .icns)
6. **Document thoroughly** — So you (and others) can maintain it later
