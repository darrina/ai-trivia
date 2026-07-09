import Foundation
import SwiftUI

// Codable DTO for persistence
private struct SettingsData: Codable {
    var questionTime: Double
    var countdownDuration: Int
    var maxComboMultiplier: Double
    var streakBonusIncrement: Double
    var easyMultiplier: Double
    var mediumMultiplier: Double
    var hardMultiplier: Double
    var enabledCategories: [String]
}

@Observable
class GameSettings {
    // Timer
    var questionTime: Double = 10.0 { didSet { save() } }
    var countdownDuration: Int = 3 { didSet { save() } }
    
    // Scoring
    var maxComboMultiplier: Double = 4.0 { didSet { save() } }
    var streakBonusIncrement: Double = 0.5 { didSet { save() } }
    var easyMultiplier: Double = 1.0 { didSet { save() } }
    var mediumMultiplier: Double = 1.5 { didSet { save() } }
    var hardMultiplier: Double = 2.0 { didSet { save() } }
    
    // Categories (all enabled by default)
    var enabledCategories: Set<String> = Set(GameSettings.allCategories) { didSet { save() } }
    
    static let allCategories: [String] = [
        "LLMs and AI Fundamentals",
        "Prompt and Context Engineering",
        "Measuring AI Performance / Metrics and Evaluation",
        "Data Handling & Preprocessing with Pandas",
        "Retrieval Augmented Generation (RAG)",
        "Building an AI Agent",
        "Model Tuning",
        "AI as a CoPilot",
        "DevOps, MLOps, LLMOps"
    ]
    
    private static let configDir = FileManager.default.homeDirectoryForCurrentUser
        .appendingPathComponent(".config/aitrivia")
    private static let configFile = configDir.appendingPathComponent("config.json")
    
    /// Suppress saving while loading from disk
    private var isSuppressingSave = false
    
    init() {
        load()
    }
    
    // MARK: - Persistence
    
    func save() {
        guard !isSuppressingSave else { return }
        let data = SettingsData(
            questionTime: questionTime,
            countdownDuration: countdownDuration,
            maxComboMultiplier: maxComboMultiplier,
            streakBonusIncrement: streakBonusIncrement,
            easyMultiplier: easyMultiplier,
            mediumMultiplier: mediumMultiplier,
            hardMultiplier: hardMultiplier,
            enabledCategories: Array(enabledCategories)
        )
        do {
            try FileManager.default.createDirectory(at: Self.configDir, withIntermediateDirectories: true)
            let json = try JSONEncoder().encode(data)
            try json.write(to: Self.configFile, options: .atomic)
        } catch {
            print("Failed to save settings: \(error)")
        }
    }
    
    func load() {
        guard FileManager.default.fileExists(atPath: Self.configFile.path) else { return }
        do {
            let json = try Data(contentsOf: Self.configFile)
            let data = try JSONDecoder().decode(SettingsData.self, from: json)
            isSuppressingSave = true
            questionTime = data.questionTime
            countdownDuration = data.countdownDuration
            maxComboMultiplier = data.maxComboMultiplier
            streakBonusIncrement = data.streakBonusIncrement
            easyMultiplier = data.easyMultiplier
            mediumMultiplier = data.mediumMultiplier
            hardMultiplier = data.hardMultiplier
            enabledCategories = Set(data.enabledCategories)
            isSuppressingSave = false
        } catch {
            print("Failed to load settings: \(error)")
        }
    }
    
    func resetToDefaults() {
        questionTime = 10.0
        countdownDuration = 3
        maxComboMultiplier = 4.0
        streakBonusIncrement = 0.5
        easyMultiplier = 1.0
        mediumMultiplier = 1.5
        hardMultiplier = 2.0
        enabledCategories = Set(GameSettings.allCategories)
    }
}
