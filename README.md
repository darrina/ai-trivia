# AI Trivia

A fast-paced, fun trivia game featuring AI and machine learning topics. Built natively for both macOS (Swift/SwiftUI) and Windows (.NET/WinUI).

## Features

- **Endless gameplay** — Play until you decide to stop
- **Speed-based scoring** — Answer faster = more points
- **Combo multipliers** — Build streaks for exponential score multipliers
- **Difficulty levels** — Easy, medium, and hard questions with different point multipliers
- **Configurable difficulty** — Customize timer, scoring multipliers, and enabled topics
- **Persistent settings** — Your preferences are automatically saved
- **Snarky host** — "You Don't Know Jack" style commentary
- **Beautiful UI** — Glowing neural network graphics and smooth animations
- **90+ questions** — Questions covering 9 AI/ML topics from lecture notes

## Gameplay

1. Start a game from the title screen
2. Answer 4-choice questions about AI and ML
3. **Faster answers = more points**
   - Time bonus × difficulty multiplier × combo multiplier
   - Easy: 1x, Medium: 1.5x, Hard: 2x
4. Build streaks to increase your combo (up to 4x by default)
5. See how many questions you can answer before taking a break

## Scoring Formula

```
Points = (Time Remaining × 100) × Difficulty × Combo Multiplier

Combo Multiplier = 1.0 + (Streak - 1) × Streak Bonus
```

Example: Answer a hard question in 3 seconds on a 4x streak:
- Base: 3 × 100 = 300 points
- Difficulty: 300 × 2.0 = 600 points
- Combo: 600 × 4.0 = **2,400 points** 🔥

## Installation

### macOS

```bash
# Download and run the installer
open AI\ Trivia.pkg

# Or build from source
cd "AI Trivia"
swift run
```

**Requirements:** macOS 14.0+

### Windows

```bash
# Using Visual Studio
cd AITrivia.WinUI
dotnet build
dotnet run

# Or download the installer
# [Installer coming soon]
```

**Requirements:** Windows 10 (build 19041) or later, .NET 8.0

## Configuration

Open settings with **⌘, (macOS)** or **Ctrl+, (Windows)** to customize:

- **Timer** — Question duration (3-30 seconds)
- **Scoring** — Combo multiplier, streak bonus, difficulty multipliers
- **Categories** — Enable/disable specific AI/ML topics

Settings are auto-saved to:
- macOS: `~/.config/aitrivia/config.json`
- Windows: `%USERPROFILE%\.config\aitrivia\config.json`

## Project Structure

```
ai-trivia/
├── AI Trivia/              # macOS app (Swift/SwiftUI)
│   ├── Sources/
│   │   ├── Views/
│   │   ├── ViewModels/
│   │   └── Models/
│   └── Package.swift
│
├── AITrivia.WinUI/         # Windows app (.NET/WinUI)
│   ├── Views/
│   ├── ViewModels/
│   ├── Models/
│   ├── Controls/
│   └── AITrivia.WinUI.csproj
│
├── data/                   # Lecture PDFs (used to generate questions)
├── trivia_questions.json   # 90 questions (shared across platforms)
├── README.md              # This file
├── DEVELOPMENT.md         # macOS development guide
└── DEVELOPMENT-WINDOWS.md # Windows development guide
```

## Cross-Platform Architecture

Both versions share:
- **Trivia data** (`trivia_questions.json`) — Same 90 questions on both platforms
- **Game logic** — Identical scoring, timer, and state management
- **UI/UX** — Consistent look, feel, animations across platforms

Platform-specific:
- **macOS:** Swift, SwiftUI, SPM
- **Windows:** C#, XAML, WinUI 3

## Development

**Quick start:** See [BUILD.md](./BUILD.md) for building `.pkg` (macOS) and `.msi` (Windows) installers using `make`.

For detailed guides, see [docs/INDEX.md](./docs/INDEX.md):
- [macOS Development](./docs/macos/DEVELOPMENT.md) — Swift/SwiftUI, SPM, building .app bundles
- [Windows Development](./docs/windows/DEVELOPMENT.md) — C#/XAML, .NET SDK, MSIX packaging

## Questions & Topics

The 90 trivia questions cover:

1. **LLMs and AI Fundamentals** (10 Q)
2. **Prompt and Context Engineering** (10 Q)
3. **Measuring AI Performance & Metrics** (10 Q)
4. **Data Handling & Preprocessing** (10 Q)
5. **Retrieval Augmented Generation (RAG)** (10 Q)
6. **Building an AI Agent** (10 Q)
7. **Model Tuning** (10 Q)
8. **AI as a CoPilot** (10 Q)
9. **DevOps, MLOps, LLMOps** (10 Q)

All questions were extracted from lecture slides and include fun facts and snarky commentary.

## Technical Details

### macOS

- **Language:** Swift 5.9+
- **Framework:** SwiftUI
- **Package Manager:** Swift Package Manager (SPM)
- **Minimum OS:** macOS 14.0
- **Architecture:** Supports Intel (x64) and Apple Silicon (arm64)

### Windows

- **Language:** C# 11
- **Framework:** WinUI 3
- **Runtime:** .NET 8.0
- **Minimum OS:** Windows 10 (build 19041)
- **Architectures:** x86, x64, arm64
- **UI Toolkit:** Microsoft.UI.Xaml + Community Toolkit MVVM

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| ⌘, (Mac) / Ctrl+, (Win) | Open Settings |
| ESC | Close Settings/About |
| Enter/Return | In Settings, apply or confirm |
| ⌘Q (Mac) / Alt+F4 (Win) | Quit Application |

## Building & Packaging

### Create macOS Installer

```bash
cd "AI Trivia"
swift build -c release
# See DEVELOPMENT.md for .app bundle and .pkg creation
```

### Create Windows Installer

```bash
cd AITrivia.WinUI
dotnet publish -c Release -f net8.0-windows10.0.19041.0
# See DEVELOPMENT-WINDOWS.md for MSIX packaging
```

## Troubleshooting

### macOS

See [DEVELOPMENT.md](./DEVELOPMENT.md) for troubleshooting common issues:
- App window not showing
- Settings not persisting
- Resources not found
- Build failures

### Windows

See [DEVELOPMENT-WINDOWS.md](./DEVELOPMENT-WINDOWS.md) for troubleshooting:
- Dependencies not found
- XAML parsing errors
- Settings location issues
- Runtime exceptions

## Future Enhancements

- [ ] Daily challenge mode
- [ ] Leaderboards and high scores
- [ ] Custom question packs
- [ ] Accessibility improvements (screen reader support)
- [ ] Localization (multiple languages)
- [ ] Multiplayer mode
- [ ] Question editor UI
- [ ] Statistics/analytics dashboard

## Contributing

This project welcomes contributions! Areas for improvement:

- Add more trivia questions
- Improve UI/UX design
- Add new game modes
- Performance optimizations
- Bug fixes and stability improvements

## License

[Add license here if applicable]

## About

Built as a fun educational trivia game to test knowledge of AI and machine learning concepts. Built with ❤️ in SwiftUI and WinUI 3.

---

**Got questions?** Check the [macOS](./DEVELOPMENT.md) or [Windows](./DEVELOPMENT-WINDOWS.md) development guides for detailed technical information.
