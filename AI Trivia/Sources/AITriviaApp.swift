import SwiftUI
import AppKit

class AppDelegate: NSObject, NSApplicationDelegate {
    func applicationDidFinishLaunching(_ notification: Notification) {
        NSApp.setActivationPolicy(.regular)
        NSApp.activate(ignoringOtherApps: true)
    }
    
    func applicationShouldTerminateAfterLastWindowClosed(_ sender: NSApplication) -> Bool {
        true
    }
}

@main
struct AITriviaApp: App {
    @NSApplicationDelegateAdaptor(AppDelegate.self) var appDelegate
    @State private var showSettings = false
    @State private var showAbout = false
    @State private var settings = GameSettings()
    
    var body: some Scene {
        WindowGroup {
            ContentView(settings: settings, showSettings: $showSettings, showAbout: $showAbout)
                .frame(minWidth: 800, minHeight: 600)
        }
        .windowStyle(.hiddenTitleBar)
        .defaultSize(width: 900, height: 700)
        .commands {
            // Remove unwanted default menus
            CommandGroup(replacing: .newItem) { }
            CommandGroup(replacing: .undoRedo) { }
            CommandGroup(replacing: .pasteboard) { }
            
            // Replace default About with our overlay
            CommandGroup(replacing: .appInfo) {
                Button("About AI Trivia") {
                    showAbout = true
                }
            }
            
            // Settings in app menu (⌘,)
            CommandGroup(replacing: .appSettings) {
                Button("Settings...") {
                    showSettings.toggle()
                }
                .keyboardShortcut(",", modifiers: .command)
            }
            
            // Remove Help menu entirely
            CommandGroup(replacing: .help) { }
            
            // Remove tab bar items from View
            CommandGroup(replacing: .windowArrangement) { }
        }
    }
}
