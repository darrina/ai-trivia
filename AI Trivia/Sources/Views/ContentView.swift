import SwiftUI

struct ContentView: View {
    var settings: GameSettings
    @Binding var showSettings: Bool
    @Binding var showAbout: Bool
    @State private var viewModel = GameViewModel()
    
    var body: some View {
        ZStack {
            // Background gradient
            LinearGradient(
                colors: [Color(hex: "1a1a2e"), Color(hex: "16213e"), Color(hex: "0f3460")],
                startPoint: .topLeading,
                endPoint: .bottomTrailing
            )
            .ignoresSafeArea()
            
            // Animated particles background
            ParticleView()
                .ignoresSafeArea()
            
            switch viewModel.phase {
            case .titleScreen:
                TitleScreenView(viewModel: viewModel)
                    .transition(.opacity.combined(with: .scale))
            case .countdown:
                CountdownView(value: viewModel.countdownValue)
                    .transition(.scale)
            case .question, .revealAnswer:
                QuestionView(viewModel: viewModel)
                    .transition(.asymmetric(
                        insertion: .move(edge: .trailing).combined(with: .opacity),
                        removal: .move(edge: .leading).combined(with: .opacity)
                    ))
            case .gameOver:
                GameOverView(viewModel: viewModel)
                    .transition(.opacity.combined(with: .scale))
            }
            
            // Settings gear button (top-right)
            VStack {
                HStack {
                    Spacer()
                    Button(action: { showSettings.toggle() }) {
                        Image(systemName: "gearshape.fill")
                            .font(.system(size: 18))
                            .foregroundColor(.white.opacity(0.5))
                            .padding(10)
                            .background(Circle().fill(Color.white.opacity(0.08)))
                    }
                    .buttonStyle(.plain)
                    .help("Settings (⌘,)")
                }
                .padding(.trailing, 16)
                .padding(.top, 12)
                Spacer()
            }
            
            // Settings overlay
            if showSettings {
                Color.black.opacity(0.4)
                    .ignoresSafeArea()
                    .onTapGesture { showSettings = false }
                
                SettingsView(settings: settings, onClose: { showSettings = false })
            }
            
            // About overlay
            if showAbout {
                Color.black.opacity(0.4)
                    .ignoresSafeArea()
                    .onTapGesture { showAbout = false }
                
                AboutView(onClose: { showAbout = false })
            }
        }
        .animation(.easeInOut(duration: 0.4), value: viewModel.phase == .titleScreen)
        .animation(.easeInOut(duration: 0.3), value: viewModel.phase == .gameOver)
        .animation(.easeOut(duration: 0.25), value: showSettings)
        .onAppear {
            viewModel.applySettings(settings)
        }
        .onChange(of: settings.questionTime) { _, _ in viewModel.applySettings(settings) }
        .onChange(of: settings.countdownDuration) { _, _ in viewModel.applySettings(settings) }
        .onChange(of: settings.maxComboMultiplier) { _, _ in viewModel.applySettings(settings) }
        .onChange(of: settings.streakBonusIncrement) { _, _ in viewModel.applySettings(settings) }
        .onChange(of: settings.easyMultiplier) { _, _ in viewModel.applySettings(settings) }
        .onChange(of: settings.mediumMultiplier) { _, _ in viewModel.applySettings(settings) }
        .onChange(of: settings.hardMultiplier) { _, _ in viewModel.applySettings(settings) }
        .onChange(of: settings.enabledCategories) { _, _ in viewModel.applySettings(settings) }
    }
}

// MARK: - Color Extension
extension Color {
    init(hex: String) {
        let hex = hex.trimmingCharacters(in: CharacterSet.alphanumerics.inverted)
        var int: UInt64 = 0
        Scanner(string: hex).scanHexInt64(&int)
        let a, r, g, b: UInt64
        switch hex.count {
        case 6:
            (a, r, g, b) = (255, int >> 16, int >> 8 & 0xFF, int & 0xFF)
        case 8:
            (a, r, g, b) = (int >> 24, int >> 16 & 0xFF, int >> 8 & 0xFF, int & 0xFF)
        default:
            (a, r, g, b) = (255, 0, 0, 0)
        }
        self.init(
            .sRGB,
            red: Double(r) / 255,
            green: Double(g) / 255,
            blue: Double(b) / 255,
            opacity: Double(a) / 255
        )
    }
}
