# Makefile for AI Trivia - Cross-platform build automation
# Targets: pkg (macOS), msi (Windows), all, clean, help
# 
# Usage:
#   make pkg              # Build macOS .pkg installer
#   make msi              # Build Windows .msi installer  
#   make all              # Auto-detect OS and build
#   make clean            # Remove build artifacts
#   make help             # Show this message

.PHONY: help all pkg msi macos windows clean info

# Configuration
VERSION := 1.0.0
APP_NAME := AITrivia
DISPLAY_NAME := AI Trivia
OUTPUT_DIR := dist
UNAME_S := $(shell uname -s)
OS_DETECTED := $(if $(findstring Darwin,$(UNAME_S)),macos,$(if $(findstring Windows_NT,$(OS)),windows,unknown))

# macOS variables
MACOS_SOURCE_DIR := AITrivia.Swift
MACOS_PKG_IDENTIFIER := com.aitrivia.app
MACOS_BINARY := $(MACOS_SOURCE_DIR)/.build/arm64-apple-macosx/release/$(APP_NAME)
MACOS_BUNDLE := $(DISPLAY_NAME).app
MACOS_PKG_ROOT := $(OUTPUT_DIR)/pkg_root
MACOS_PKG_OUTPUT := $(OUTPUT_DIR)/$(APP_NAME)-$(VERSION)-macos.pkg

# Windows variables
WINDOWS_PROJ_DIR := AITrivia.WinUI
WINDOWS_CSPROJ := $(WINDOWS_PROJ_DIR)/AITrivia.WinUI.csproj
WINDOWS_BUILD_DIR := $(WINDOWS_PROJ_DIR)/bin/Release/net8.0-windows10.0.19041.0
WINDOWS_MSIX_OUTPUT := $(OUTPUT_DIR)/$(APP_NAME)-$(VERSION)-windows.msix
WINDOWS_MSI_OUTPUT := $(OUTPUT_DIR)/$(APP_NAME)-$(VERSION)-windows.msi

# Default target: auto-detect OS
all: $(OS_DETECTED)

help:
	@echo "AI Trivia - Build System"
	@echo "======================="
	@echo ""
	@echo "Targets:"
	@echo "  make pkg              Build macOS .pkg installer"
	@echo "  make msi              Build Windows .msi installer"
	@echo "  make all              Auto-detect OS and build"
	@echo "  make macos            Build macOS version"
	@echo "  make windows          Build Windows version"
	@echo "  make info             Show build configuration"
	@echo "  make clean            Remove build artifacts"
	@echo "  make help             Show this message"
	@echo ""
	@echo "Environment:"
	@echo "  VERSION: $(VERSION)"
	@echo "  Detected OS: $(OS_DETECTED)"

info:
	@echo "Build Configuration"
	@echo "==================="
	@echo "Version: $(VERSION)"
	@echo "Output Dir: $(OUTPUT_DIR)"
	@echo "Detected OS: $(OS_DETECTED)"
	@echo ""
	@echo "macOS Configuration:"
	@echo "  Source: $(MACOS_SOURCE_DIR)"
	@echo "  App Name: $(DISPLAY_NAME)"
	@echo "  Bundle: $(MACOS_BUNDLE)"
	@echo "  Binary: $(MACOS_BINARY)"
	@echo "  Output: $(MACOS_PKG_OUTPUT)"
	@echo ""
	@echo "Windows Configuration:"
	@echo "  Project: $(WINDOWS_CSPROJ)"
	@echo "  MSIX: $(WINDOWS_MSIX_OUTPUT)"
	@echo "  MSI: $(WINDOWS_MSI_OUTPUT)"

# ============================================================================
# macOS Targets
# ============================================================================

macos: pkg

pkg: $(MACOS_PKG_OUTPUT)
	@echo ""
	@echo "✓ macOS installer created: $(MACOS_PKG_OUTPUT)"

$(MACOS_PKG_OUTPUT): $(MACOS_BINARY) | $(OUTPUT_DIR)
	@echo "Creating .app bundle and .pkg installer..."
	@rm -rf "$(MACOS_PKG_ROOT)" "$(MACOS_BUNDLE)"
	@mkdir -p "$(MACOS_PKG_ROOT)/Applications"
	@mkdir -p "$(MACOS_BUNDLE)/Contents/MacOS"
	@mkdir -p "$(MACOS_BUNDLE)/Contents/Resources"
	@echo "  • Copying binary..."
	@cp "$(MACOS_BINARY)" "$(MACOS_BUNDLE)/Contents/MacOS/$(APP_NAME)"
	@chmod +x "$(MACOS_BUNDLE)/Contents/MacOS/$(APP_NAME)"
	@echo "  • Creating Info.plist..."
	@./scripts/create_plist.sh "$(MACOS_BUNDLE)/Contents/Info.plist" "$(VERSION)"
	@echo "  • Copying resources..."
	@cp trivia_questions.json "$(MACOS_BUNDLE)/Contents/Resources/" 2>/dev/null || true
	@cp "$(MACOS_SOURCE_DIR)/Sources/Resources"/* "$(MACOS_BUNDLE)/Contents/Resources/" 2>/dev/null || true
	@echo "  • Moving to pkgbuild root..."
	@cp -r "$(MACOS_BUNDLE)" "$(MACOS_PKG_ROOT)/Applications/"
	@echo "  • Building .pkg..."
	@pkgbuild \
		--root "$(MACOS_PKG_ROOT)" \
		--identifier "$(MACOS_PKG_IDENTIFIER)" \
		--version "$(VERSION)" \
		--install-location "/" \
		"$(MACOS_PKG_OUTPUT)"

$(MACOS_BINARY):
	@echo "Building macOS binary (release mode)..."
	@cd "$(MACOS_SOURCE_DIR)" && swift build -c release

# ============================================================================
# Windows Targets
# ============================================================================

windows: msi

msi: $(WINDOWS_MSI_OUTPUT)
	@echo ""
	@echo "✓ Windows installer created: $(WINDOWS_MSI_OUTPUT)"

$(WINDOWS_MSI_OUTPUT): | $(OUTPUT_DIR)
	@echo "Building Windows application..."
	@dotnet publish "$(WINDOWS_CSPROJ)" \
		-c Release \
		--self-contained \
		-f net8.0-windows10.0.19041.0 \
		-p:GenerateAppxPackageOnBuild=true
	@echo "Locating MSIX artifact..."
	@for /r "$(WINDOWS_BUILD_DIR)" %%F in (*.msix) do copy "%%F" "$(WINDOWS_MSIX_OUTPUT)" >nul 2>&1
	@if exist "$(WINDOWS_MSIX_OUTPUT)" copy "$(WINDOWS_MSIX_OUTPUT)" "$(WINDOWS_MSI_OUTPUT)" >nul 2>&1

# ============================================================================
# Utility Targets
# ============================================================================

$(OUTPUT_DIR):
	@mkdir -p "$(OUTPUT_DIR)" 2>/dev/null || mkdir "$(OUTPUT_DIR)"

clean:
	@echo "Cleaning build artifacts..."
	@rm -rf "$(OUTPUT_DIR)" 2>/dev/null || rmdir /s /q "$(OUTPUT_DIR)" 2>nul || true
	@rm -rf "$(MACOS_BUNDLE)" 2>/dev/null || rmdir /s /q "$(MACOS_BUNDLE)" 2>nul || true
	@rm -rf "$(MACOS_SOURCE_DIR)/.build" 2>/dev/null || rmdir /s /q "$(MACOS_SOURCE_DIR)\.build" 2>nul || true
	@rm -rf "$(WINDOWS_PROJ_DIR)/bin" 2>/dev/null || rmdir /s /q "$(WINDOWS_PROJ_DIR)\bin" 2>nul || true
	@rm -rf "$(WINDOWS_PROJ_DIR)/obj" 2>/dev/null || rmdir /s /q "$(WINDOWS_PROJ_DIR)\obj" 2>nul || true
	@echo "✓ Clean complete"

$(OS_DETECTED):
	@echo "Building for $(OS_DETECTED)..."
