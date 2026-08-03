# Build Guide for AI Trivia

This guide covers building AI Trivia using the **Makefile** for both macOS and Windows platforms.

## Quick Start

### macOS
```bash
make pkg              # Build .pkg installer (requires macOS)
make clean           # Remove build artifacts
```

### Windows
```bash
make msi              # Build .msi installer (requires Windows)
make clean           # Remove build artifacts
```

### Either Platform
```bash
make all             # Auto-detect OS and build platform-specific installer
make info            # Show build configuration
make help            # Show all available targets
```

## Installation Requirements

### macOS
- **Xcode Command Line Tools** (for Swift compiler and build tools)
  ```bash
  xcode-select --install
  ```
- **Swift 5.9+** (included with Xcode)
- **pkgbuild** (included with Xcode)

### Windows
- **.NET 8.0 SDK** or newer
  - Download: https://dotnet.microsoft.com/download
- **Visual Studio 2022** (optional, for development/debugging)
  - Or use any text editor + CLI build tools
- **Git Bash** or other Unix-like shell (for running Makefile commands)
  - Download: https://git-scm.com/download/win

## Build Targets

### `make pkg` (macOS only)
Builds the `.pkg` installer for macOS.

**What it does:**
1. Compiles `AITrivia.Swift/` with Swift Package Manager (SPM)
2. Creates `.app` bundle structure (`AI Trivia.app/Contents/MacOS/`, `Resources/`, `Info.plist`)
3. Packages with `pkgbuild` into `dist/AITrivia-1.0.0-macos.pkg`

**Output:**
- `dist/AITrivia-1.0.0-macos.pkg` — Ready to distribute or double-click to install

**Troubleshooting:**
- **"swift: command not found"** → Run `xcode-select --install`
- **"pkgbuild: command not found"** → Xcode is not installed
- **Permission errors** → Run `sudo chmod +x scripts/create_plist.sh`

### `make msi` (Windows only)
Builds the `.msi` installer for Windows.

**What it does:**
1. Restores NuGet packages
2. Publishes `AITrivia.WinUI/` as self-contained release build
3. Enables MSIX package generation (`GenerateAppxPackageOnBuild=true`)
4. Copies MSIX to `dist/` as output artifact

**Output:**
- `dist/AITrivia-1.0.0-windows.msix` — Ready to distribute
- Optional: Convert to `.msi` using WiX Toolset (see below)

**Troubleshooting:**
- **"dotnet: command not found"** → Install .NET 8.0 SDK
- **"Could not restore NuGet packages"** → Check internet connection, try `dotnet restore AITrivia.WinUI/`
- **XAML parse errors** → Open project in Visual Studio to see detailed errors

### `make all`
Auto-detects the current OS and builds the appropriate installer.

**On macOS:** Equivalent to `make pkg`  
**On Windows:** Equivalent to `make msi`

### `make clean`
Removes all build artifacts:
- `dist/` directory (build outputs)
- `.app` bundle (if present in root)
- `.build/` directories in each project
- `bin/` and `obj/` directories in Windows project

### `make info`
Displays build configuration without building:
```
Version: 1.0.0
Output Dir: dist
Detected OS: macos

macOS Configuration:
  Source: AITrivia.Swift
  App Name: AI Trivia
  Binary: AITrivia.Swift/.build/arm64-apple-macosx/release/AITrivia
  Output: dist/AITrivia-1.0.0-macos.pkg

Windows Configuration:
  Project: AITrivia.WinUI/AITrivia.WinUI.csproj
  MSIX: dist/AITrivia-1.0.0-windows.msix
  MSI: dist/AITrivia-1.0.0-windows.msi
```

### `make help`
Shows all available targets and quick reference.

## Build Artifacts

After a successful build, check the `dist/` folder:

### macOS
```
dist/
├── AITrivia-1.0.0-macos.pkg    # Ready to distribute
├── pkg_root/                    # Intermediate (can delete)
└── AI Trivia.app/              # Intermediate (can delete)
```

**To install locally:**
```bash
open dist/AITrivia-1.0.0-macos.pkg
# or
installer -pkg dist/AITrivia-1.0.0-macos.pkg -target /
```

### Windows
```
dist/
├── AITrivia-1.0.0-windows.msix  # Ready to distribute
└── AITrivia-1.0.0-windows.msi   # If created from MSIX
```

**To install locally:**
- Double-click `.msix` to install, or
- Use `Add-AppxPackage -Path dist/AITrivia-1.0.0-windows.msix` in PowerShell

## Configuration

Edit `Makefile` to customize build behavior:

```makefile
VERSION := 1.0.0         # Version number for .pkg/.msi
APP_NAME := AITrivia     # Binary name (don't change)
DISPLAY_NAME := AI Trivia # Display name in .app bundle (optional to change)
```

## Advanced: Creating .msi on Windows

The current build creates `.msix` (Microsoft's modern installer format). To create a traditional `.msi` file:

### Option 1: WiX Toolset (Recommended)
Install WiX Toolset v3.14+: https://github.com/wixtoolset/wix3/releases

Then use `heat`, `candle`, and `light` to generate `.msi`:
```bash
# Generate component references
heat dir AITrivia.WinUI\bin\Release\net8.0-windows10.0.19041.0\publish -o components.wxs

# Compile WiX source
candle components.wxs -o obj\

# Link to create .msi
light obj\components.wixobj -o AITrivia-1.0.0-windows.msi
```

### Option 2: MSIXHero
Use MSIXHero app to convert `.msix` → `.msi`: https://msixhero.net/

## CI/CD Integration

### GitHub Actions
Example workflow to build on push:
```yaml
name: Build

on: [push]

jobs:
  macos:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v3
      - run: make pkg
      - uses: actions/upload-artifact@v3
        with:
          name: macos-pkg
          path: dist/*.pkg

  windows:
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      - run: make msi
      - uses: actions/upload-artifact@v3
        with:
          name: windows-msi
          path: dist/*.msix
```

## Troubleshooting

### "missing separator" error in Makefile
**Solution:** Use a real tab character for recipe indentation, not spaces. Most editors have a "convert to tabs" option.

### Build timeout
**Solution:** Build artifacts are cached. Use `make clean` then `make pkg` to force a full rebuild.

### Multiple architectures (arm64 vs x86_64)
The macOS binary is built for the current architecture (arm64 on Apple Silicon, x86_64 on Intel). To build for both:
```bash
cd AITrivia.Swift
swift build -c release --arch arm64 --arch x86_64
```

## For More Information

- **macOS Development:** See [docs/macos/DEVELOPMENT.md](./docs/macos/DEVELOPMENT.md)
- **Windows Development:** See [docs/windows/DEVELOPMENT.md](./docs/windows/DEVELOPMENT.md)
- **Project README:** See [README.md](./README.md)
