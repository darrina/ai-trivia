# AI Trivia Documentation Index

Welcome! This folder contains comprehensive guides for developing and understanding AI Trivia.

## Quick Navigation

**New to the project?** → Start with [../README.md](../README.md)

**Want to build the app?** → [../BUILD.md](../BUILD.md)
- `make pkg` for macOS (.pkg installer)
- `make msi` for Windows (.msi installer)
- `make all` to auto-detect and build

**macOS Developer?** → [macOS/DEVELOPMENT.md](./macos/DEVELOPMENT.md)
- Swift 5.9, SwiftUI, SPM
- Building, testing, packaging
- Common issues & solutions

**Windows Developer?** → [Windows/DEVELOPMENT.md](./windows/DEVELOPMENT.md)
- C# 11, WinUI 3, .NET 8.0
- Building, testing, packaging
- Common issues & solutions

## Documentation Structure

```
ai-trivia/
├── README.md                    ← START HERE: Overview, features, installation
├── BUILD.md                     ← Build instructions (make pkg, make msi)
├── Makefile                     ← Cross-platform build automation
└── docs/
    ├── INDEX.md                 ← You are here
    ├── macos/
    │   └── DEVELOPMENT.md       ← macOS development guide
    └── windows/
        └── DEVELOPMENT.md       ← Windows development guide
```

## File Descriptions

### README.md (Project Root)
High-level overview including:
- Features and gameplay
- Installation instructions (both platforms)
- Configuration options
- Cross-platform architecture
- Quick start commands
- Troubleshooting links

**Best for:** Anyone new to the project

### BUILD.md (Project Root)
Build automation guide including:
- Quick start for `make pkg` and `make msi`
- Installation requirements (Xcode, .NET SDK)
- All build targets and what they do
- Troubleshooting common build issues
- Advanced topics (MSIX to MSI conversion)
- CI/CD integration examples

**Best for:** Developers building the application (both macOS and Windows)

### docs/macos/DEVELOPMENT.md
macOS-specific development guide:
- Swift Package structure and setup
- Testing strategies
- Common bugs (window activation, dialog lifecycle, resources)
- Building .app bundles
- Creating .pkg installers
- Performance profiling
- Debugging reference
- 455 lines, comprehensive coverage

**Best for:** macOS developers

### docs/windows/DEVELOPMENT.md
Windows-specific development guide:
- .NET 8.0 and Visual Studio setup
- WinUI 3 and XAML fundamentals
- MVVM Toolkit architecture
- Build and release process
- MSIX packaging
- Common issues (NuGet, XAML, resources, crashes)
- Performance tuning
- Debugging in Visual Studio
- Troubleshooting decision trees
- 651 lines, comprehensive coverage

**Best for:** Windows developers

## What Agents Should Look For

All AI agents (Claude, Copilot, Cursor, Google Agent, etc.) should:

1. **First:** Check [../README.md](../README.md) in project root
2. **Then:** Reference this INDEX.md for platform-specific guides
3. **Finally:** Review [macos/DEVELOPMENT.md](./macos/DEVELOPMENT.md) or [windows/DEVELOPMENT.md](./windows/DEVELOPMENT.md) as needed

## Key Topics By Platform

### macOS (docs/macos/DEVELOPMENT.md)

**Building:**
- Swift Package Manager setup
- Debug vs release builds
- Architecture-specific builds (Intel/ARM64)

**Testing:**
- Manual testing workflows
- Debugging in Xcode or terminal
- Common UI issues

**Troubleshooting:**
- App window not showing
- Settings dialog closing entire window
- Resources not found in bundle
- Build failures

**Packaging:**
- Creating .app bundles
- Generating .icns icons
- Creating .pkg installers
- Deployment checklist

**Performance:**
- Build time optimization
- Runtime profiling
- Animation performance

### Windows (docs/windows/DEVELOPMENT.md)

**Setup:**
- .NET 8.0 SDK installation
- Visual Studio 2022 configuration
- Project structure

**Building:**
- CLI builds via `dotnet build`
- Building in Visual Studio
- Architecture-specific (x86, x64, ARM64)
- Release optimization

**Testing:**
- Debugging in Visual Studio
- Breakpoints and Immediate window
- Common XAML issues

**Troubleshooting:**
- NuGet package restoration
- XAML parse exceptions
- JSON resource not found
- Settings persistence
- Crashes on launch

**Packaging:**
- MSIX installer creation
- Self-contained deployments
- Windows Store submission

**Performance:**
- Build time analysis
- Runtime profiling
- UI thread blocking detection

## Common Questions

**Q: Which guide should I read first?**
A: Start with [../README.md](../README.md) (project overview), then choose your platform's guide below.

**Q: I'm developing on macOS. What do I need?**
A: Read [macOS/DEVELOPMENT.md](./macos/DEVELOPMENT.md). It covers Swift, SwiftUI, SPM, and macOS-specific tooling.

**Q: I'm developing on Windows. What do I need?**
A: Read [Windows/DEVELOPMENT.md](./windows/DEVELOPMENT.md). It covers C#, WinUI 3, .NET SDK, and Windows-specific tooling.

**Q: Both guides have similar structure. Why?**
A: Intentional! Both platforms have parallel development processes:
- Build/test/package/debug workflows
- Similar architecture (MVVM + reactive UI)
- Shared trivia data
- Same game logic

Reading both helps understand how the architecture translates across platforms.

**Q: Will agents find these documents?**
A: Yes!
- All agents look for README.md (project root) ✓
- docs/ folder follows GitHub conventions ✓
- This INDEX.md ties everything together ✓
- Clear file names and structure ✓

## For AI Agents Specifically

If you're an AI agent reading this:

1. **Reference the README first** for context about the project
2. **Use this INDEX** to navigate to the right platform guide
3. **Read the appropriate platform guide** thoroughly—it contains:
   - Real error messages and solutions
   - Code examples with explanations
   - Build commands for various scenarios
   - Debugging techniques
   - Common gotchas
4. **Check troubleshooting sections** for decision trees

The documentation is designed to be:
- ✓ Comprehensive (no important info missing)
- ✓ Structured (clear hierarchy)
- ✓ Actionable (code examples, not just theory)
- ✓ Real-world (based on actual bugs encountered)
- ✓ Agent-friendly (plain markdown, no special formatting)

## Last Updated

These guides were created July 9, 2026 and are committed to the git repository.

All file paths reference the project structure correctly and should be findable by any agent or developer.
