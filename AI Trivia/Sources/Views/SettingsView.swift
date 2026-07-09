import SwiftUI

struct SettingsView: View {
    @Bindable var settings: GameSettings
    var onClose: () -> Void
    
    var body: some View {
        VStack(spacing: 0) {
            // Header
            HStack {
                Text("⚙️ Settings")
                    .font(.system(size: 20, weight: .bold, design: .rounded))
                    .foregroundColor(.white)
                Spacer()
                Button(action: { onClose() }) {
                    Image(systemName: "xmark.circle.fill")
                        .font(.title2)
                        .foregroundColor(.white.opacity(0.5))
                }
                .buttonStyle(.plain)
                .keyboardShortcut(.escape, modifiers: [])
            }
            .padding(.horizontal, 24)
            .padding(.top, 20)
            .padding(.bottom, 16)
            
            Divider().overlay(Color.white.opacity(0.1))
            
            ScrollView {
                VStack(alignment: .leading, spacing: 24) {
                    // Timer Section
                    SettingsSection(title: "⏱ Timer", icon: "timer") {
                        SettingsSlider(
                            label: "Question Time",
                            value: $settings.questionTime,
                            range: 3...30,
                            step: 1,
                            unit: "sec",
                            description: "How long you have to answer each question"
                        )
                        
                        SettingsStepper(
                            label: "Countdown Duration",
                            value: $settings.countdownDuration,
                            range: 1...5,
                            unit: "sec",
                            description: "The 3-2-1 countdown before questions start"
                        )
                    }
                    
                    // Scoring Section
                    SettingsSection(title: "🏆 Scoring", icon: "star.fill") {
                        SettingsSlider(
                            label: "Max Combo Multiplier",
                            value: $settings.maxComboMultiplier,
                            range: 1.5...10.0,
                            step: 0.5,
                            unit: "x",
                            description: "Maximum multiplier from answer streaks"
                        )
                        
                        SettingsSlider(
                            label: "Streak Bonus",
                            value: $settings.streakBonusIncrement,
                            range: 0.25...2.0,
                            step: 0.25,
                            unit: "x per streak",
                            description: "How much the multiplier grows per correct answer"
                        )
                        
                        HStack(spacing: 16) {
                            DifficultyMultiplierField(
                                label: "Easy",
                                color: .green,
                                value: $settings.easyMultiplier
                            )
                            DifficultyMultiplierField(
                                label: "Medium",
                                color: .yellow,
                                value: $settings.mediumMultiplier
                            )
                            DifficultyMultiplierField(
                                label: "Hard",
                                color: .red,
                                value: $settings.hardMultiplier
                            )
                        }
                        .padding(.top, 4)
                    }
                    
                    // Categories Section
                    SettingsSection(title: "📚 Categories", icon: "folder.fill") {
                        VStack(alignment: .leading, spacing: 4) {
                            Text("Select which topics to include in the quiz")
                                .font(.system(size: 12, design: .rounded))
                                .foregroundColor(.white.opacity(0.5))
                            
                            HStack(spacing: 12) {
                                Button("Select All") {
                                    settings.enabledCategories = Set(GameSettings.allCategories)
                                }
                                .buttonStyle(MiniButtonStyle())
                                
                                Button("Deselect All") {
                                    // Keep at least one
                                    settings.enabledCategories = [GameSettings.allCategories.first!]
                                }
                                .buttonStyle(MiniButtonStyle())
                            }
                            .padding(.bottom, 8)
                        }
                        
                        ForEach(GameSettings.allCategories, id: \.self) { category in
                            CategoryToggle(
                                category: category,
                                isEnabled: Binding(
                                    get: { settings.enabledCategories.contains(category) },
                                    set: { enabled in
                                        if enabled {
                                            settings.enabledCategories.insert(category)
                                        } else if settings.enabledCategories.count > 1 {
                                            settings.enabledCategories.remove(category)
                                        }
                                    }
                                )
                            )
                        }
                    }
                    
                    // Reset
                    HStack {
                        Spacer()
                        Button(action: { settings.resetToDefaults() }) {
                            HStack(spacing: 6) {
                                Image(systemName: "arrow.counterclockwise")
                                Text("Reset to Defaults")
                            }
                            .font(.system(size: 13, weight: .medium, design: .rounded))
                            .foregroundColor(.orange)
                            .padding(.horizontal, 16)
                            .padding(.vertical, 8)
                            .background(
                                RoundedRectangle(cornerRadius: 8)
                                    .fill(Color.orange.opacity(0.1))
                                    .overlay(RoundedRectangle(cornerRadius: 8).stroke(Color.orange.opacity(0.3), lineWidth: 1))
                            )
                        }
                        .buttonStyle(.plain)
                        Spacer()
                    }
                    .padding(.top, 8)
                }
                .padding(24)
            }
        }
        .frame(width: 480, height: 560)
        .background(
            RoundedRectangle(cornerRadius: 16)
                .fill(Color(hex: "1a1a2e").opacity(0.98))
                .overlay(RoundedRectangle(cornerRadius: 16).stroke(Color.white.opacity(0.1), lineWidth: 1))
                .shadow(color: .black.opacity(0.5), radius: 30)
        )
    }
}

// MARK: - Settings Section

struct SettingsSection<Content: View>: View {
    let title: String
    let icon: String
    @ViewBuilder let content: Content
    
    var body: some View {
        VStack(alignment: .leading, spacing: 12) {
            Text(title)
                .font(.system(size: 15, weight: .bold, design: .rounded))
                .foregroundColor(.cyan)
            
            VStack(alignment: .leading, spacing: 14) {
                content
            }
            .padding(16)
            .background(
                RoundedRectangle(cornerRadius: 12)
                    .fill(Color.white.opacity(0.03))
                    .overlay(RoundedRectangle(cornerRadius: 12).stroke(Color.white.opacity(0.08), lineWidth: 1))
            )
        }
    }
}

// MARK: - Settings Slider

struct SettingsSlider: View {
    let label: String
    @Binding var value: Double
    let range: ClosedRange<Double>
    let step: Double
    let unit: String
    let description: String
    
    var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            HStack {
                Text(label)
                    .font(.system(size: 13, weight: .semibold, design: .rounded))
                    .foregroundColor(.white.opacity(0.9))
                Spacer()
                Text("\(formatValue(value)) \(unit)")
                    .font(.system(size: 13, weight: .bold, design: .monospaced))
                    .foregroundColor(.cyan)
            }
            
            Slider(value: $value, in: range, step: step)
                .tint(.cyan)
            
            Text(description)
                .font(.system(size: 11, design: .rounded))
                .foregroundColor(.white.opacity(0.4))
        }
    }
    
    private func formatValue(_ v: Double) -> String {
        if v == v.rounded() {
            return String(format: "%.0f", v)
        }
        return String(format: "%.2g", v)
    }
}

// MARK: - Settings Stepper

struct SettingsStepper: View {
    let label: String
    @Binding var value: Int
    let range: ClosedRange<Int>
    let unit: String
    let description: String
    
    var body: some View {
        VStack(alignment: .leading, spacing: 6) {
            HStack {
                VStack(alignment: .leading, spacing: 2) {
                    Text(label)
                        .font(.system(size: 13, weight: .semibold, design: .rounded))
                        .foregroundColor(.white.opacity(0.9))
                    Text(description)
                        .font(.system(size: 11, design: .rounded))
                        .foregroundColor(.white.opacity(0.4))
                }
                Spacer()
                HStack(spacing: 8) {
                    Button(action: { if value > range.lowerBound { value -= 1 } }) {
                        Image(systemName: "minus.circle.fill")
                            .foregroundColor(.white.opacity(0.6))
                    }
                    .buttonStyle(.plain)
                    
                    Text("\(value) \(unit)")
                        .font(.system(size: 13, weight: .bold, design: .monospaced))
                        .foregroundColor(.cyan)
                        .frame(minWidth: 50)
                    
                    Button(action: { if value < range.upperBound { value += 1 } }) {
                        Image(systemName: "plus.circle.fill")
                            .foregroundColor(.white.opacity(0.6))
                    }
                    .buttonStyle(.plain)
                }
            }
        }
    }
}

// MARK: - Difficulty Multiplier Field

struct DifficultyMultiplierField: View {
    let label: String
    let color: Color
    @Binding var value: Double
    
    var body: some View {
        VStack(spacing: 4) {
            Text(label)
                .font(.system(size: 11, weight: .bold, design: .rounded))
                .foregroundColor(color)
            
            HStack(spacing: 4) {
                Button(action: { if value > 0.5 { value -= 0.5 } }) {
                    Image(systemName: "minus")
                        .font(.system(size: 9, weight: .bold))
                        .foregroundColor(.white.opacity(0.5))
                }
                .buttonStyle(.plain)
                
                Text("\(String(format: "%.1f", value))x")
                    .font(.system(size: 13, weight: .bold, design: .monospaced))
                    .foregroundColor(.white)
                    .frame(minWidth: 36)
                
                Button(action: { if value < 5.0 { value += 0.5 } }) {
                    Image(systemName: "plus")
                        .font(.system(size: 9, weight: .bold))
                        .foregroundColor(.white.opacity(0.5))
                }
                .buttonStyle(.plain)
            }
            .padding(.horizontal, 8)
            .padding(.vertical, 6)
            .background(
                RoundedRectangle(cornerRadius: 6)
                    .fill(color.opacity(0.1))
                    .overlay(RoundedRectangle(cornerRadius: 6).stroke(color.opacity(0.3), lineWidth: 1))
            )
        }
    }
}

// MARK: - Category Toggle

struct CategoryToggle: View {
    let category: String
    @Binding var isEnabled: Bool
    
    var body: some View {
        HStack(spacing: 10) {
            Image(systemName: isEnabled ? "checkmark.circle.fill" : "circle")
                .foregroundColor(isEnabled ? .cyan : .white.opacity(0.3))
                .font(.system(size: 16))
            
            Text(category)
                .font(.system(size: 13, weight: .medium, design: .rounded))
                .foregroundColor(isEnabled ? .white.opacity(0.9) : .white.opacity(0.4))
            
            Spacer()
        }
        .contentShape(Rectangle())
        .onTapGesture {
            isEnabled.toggle()
        }
    }
}

// MARK: - Mini Button Style

struct MiniButtonStyle: ButtonStyle {
    func makeBody(configuration: Configuration) -> some View {
        configuration.label
            .font(.system(size: 11, weight: .medium, design: .rounded))
            .foregroundColor(.white.opacity(0.7))
            .padding(.horizontal, 10)
            .padding(.vertical, 5)
            .background(
                RoundedRectangle(cornerRadius: 6)
                    .fill(Color.white.opacity(configuration.isPressed ? 0.1 : 0.05))
                    .overlay(RoundedRectangle(cornerRadius: 6).stroke(Color.white.opacity(0.15), lineWidth: 1))
            )
            .scaleEffect(configuration.isPressed ? 0.95 : 1.0)
    }
}
