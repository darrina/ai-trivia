import Foundation
import SwiftUI

enum GamePhase {
    case titleScreen
    case countdown
    case question
    case revealAnswer
    case gameOver
}

@Observable
class GameViewModel {
    // MARK: - Game State
    var phase: GamePhase = .titleScreen
    var currentQuestion: TriviaQuestion?
    var selectedAnswerIndex: Int?
    var score: Int = 0
    var streak: Int = 0
    var bestStreak: Int = 0
    var questionsAnswered: Int = 0
    var correctAnswers: Int = 0
    var timeRemaining: Double = 10.0
    var countdownValue: Int = 3
    var showFunFact: Bool = false
    var lastPointsEarned: Int = 0
    var comboMultiplier: Double = 1.0
    
    // Snarky host messages
    var hostMessage: String = ""
    
    var maxTimeForDisplay: Double { maxTime }
    
    // MARK: - Private
    private var usedQuestionIds: Set<Int> = []
    private var triviaBank: TriviaBank
    private var timer: Timer?
    private var countdownTimer: Timer?
    private var maxTime: Double = 10.0
    private var countdownStart: Int = 3
    private var maxCombo: Double = 4.0
    private var streakIncrement: Double = 0.5
    private var difficultyMultipliers: [String: Double] = ["easy": 1.0, "medium": 1.5, "hard": 2.0]
    private var enabledCategories: Set<String> = Set(GameSettings.allCategories)
    
    // Snarky messages for correct answers
    private let correctMessages = [
        "Well, well, well... someone's been studying!",
        "Look at the big brain on you! 🧠",
        "Ding ding ding! We have a winner!",
        "Somebody paid attention in class!",
        "You're on FIRE! 🔥 (Metaphorically. Please don't actually be on fire.)",
        "Nailed it! Your neural network is clearly well-trained.",
        "Correct! Your gradient is definitely not vanishing.",
        "RIGHT! You're basically a fine-tuned model at this point.",
        "YES! That answer was more accurate than GPT-4's hallucinations.",
        "Boom! Knowledge bomb detonated successfully. 💣",
        "Are you cheating? Because that was suspiciously fast...",
        "Your accuracy is approaching 1.0! Keep it up!",
    ]
    
    // Snarky messages for wrong answers
    private let wrongMessages = [
        "Ooof. That one's gonna leave a mark. 😬",
        "Wrong! But hey, even GPT hallucinates sometimes.",
        "Nope! Your loss function just spiked dramatically.",
        "Incorrect! Time to retrain that mental model.",
        "Swing and a miss! Your confidence was high but your accuracy... not so much.",
        "WRONG! That answer had more errors than unhandled exceptions.",
        "Yikes. Even a random forest would've gotten that one right.",
        "Nah. That's what we call 'catastrophic forgetting' in the biz.",
        "Not even close! Were you trained on corrupted data?",
        "Wrong! But don't worry, this is just your validation set. 📉",
        "Oops! Your attention mechanism clearly wasn't paying attention.",
        "Incorrect! That's going straight into your error log.",
    ]
    
    // Timeout messages
    private let timeoutMessages = [
        "Time's up! Your inference speed needs work. ⏰",
        "Too slow! Even a CPU-only model is faster than you.",
        "Tick tock! The timer waited for no one.",
        "TIME OUT! Your latency is unacceptable in production.",
        "Gone! That question has left the chat. 👋",
        "Expired! Like a deprecated API endpoint.",
    ]
    
    init() {
        self.triviaBank = TriviaBank.load()
    }
    
    // MARK: - Settings
    
    func applySettings(_ settings: GameSettings) {
        maxTime = settings.questionTime
        countdownStart = settings.countdownDuration
        maxCombo = settings.maxComboMultiplier
        streakIncrement = settings.streakBonusIncrement
        difficultyMultipliers = [
            "easy": settings.easyMultiplier,
            "medium": settings.mediumMultiplier,
            "hard": settings.hardMultiplier
        ]
        enabledCategories = settings.enabledCategories
    }
    
    // MARK: - Game Flow
    
    func startGame() {
        score = 0
        streak = 0
        bestStreak = 0
        questionsAnswered = 0
        correctAnswers = 0
        usedQuestionIds = []
        comboMultiplier = 1.0
        hostMessage = "Let's see what you've got, hotshot! 🎯"
        startCountdown()
    }
    
    private func startCountdown() {
        phase = .countdown
        countdownValue = countdownStart
        countdownTimer?.invalidate()
        countdownTimer = Timer.scheduledTimer(withTimeInterval: 1.0, repeats: true) { [weak self] timer in
            guard let self else { timer.invalidate(); return }
            DispatchQueue.main.async {
                self.countdownValue -= 1
                if self.countdownValue <= 0 {
                    timer.invalidate()
                    self.nextQuestion()
                }
            }
        }
    }
    
    func nextQuestion() {
        selectedAnswerIndex = nil
        showFunFact = false
        lastPointsEarned = 0
        
        if let question = triviaBank.randomQuestion(excluding: usedQuestionIds, categories: enabledCategories) {
            currentQuestion = question
            usedQuestionIds.insert(question.id)
            timeRemaining = maxTime
            phase = .question
            startTimer()
        } else {
            // All questions exhausted, reset and continue
            usedQuestionIds = []
            if let question = triviaBank.randomQuestion(excluding: usedQuestionIds, categories: enabledCategories) {
                currentQuestion = question
                usedQuestionIds.insert(question.id)
                timeRemaining = maxTime
                phase = .question
                startTimer()
            }
        }
    }
    
    func selectAnswer(_ index: Int) {
        guard phase == .question, selectedAnswerIndex == nil else { return }
        
        stopTimer()
        selectedAnswerIndex = index
        questionsAnswered += 1
        
        let isCorrect = index == currentQuestion?.correctIndex
        
        if isCorrect {
            // Score based on time remaining - faster = more points
            let timeBonus = Int(timeRemaining * 100)
            let difficultyMultiplier = difficultyMultipliers[currentQuestion?.difficulty ?? "easy"] ?? 1.0
            
            streak += 1
            if streak > bestStreak { bestStreak = streak }
            
            // Combo multiplier increases with streak
            comboMultiplier = min(maxCombo, 1.0 + Double(streak - 1) * streakIncrement)
            
            let points = Int(Double(timeBonus) * difficultyMultiplier * comboMultiplier)
            lastPointsEarned = points
            score += points
            correctAnswers += 1
            
            hostMessage = correctMessages.randomElement() ?? "Correct!"
            if streak >= 3 {
                hostMessage += " 🔥 \(streak)x STREAK!"
            }
        } else {
            streak = 0
            comboMultiplier = 1.0
            lastPointsEarned = 0
            hostMessage = wrongMessages.randomElement() ?? "Wrong!"
        }
        
        phase = .revealAnswer
        showFunFact = true
    }
    
    func timeExpired() {
        guard phase == .question else { return }
        
        stopTimer()
        questionsAnswered += 1
        streak = 0
        comboMultiplier = 1.0
        lastPointsEarned = 0
        hostMessage = timeoutMessages.randomElement() ?? "Time's up!"
        phase = .revealAnswer
        showFunFact = true
    }
    
    func endGame() {
        stopTimer()
        countdownTimer?.invalidate()
        phase = .gameOver
        
        let accuracy = questionsAnswered > 0 ? Int((Double(correctAnswers) / Double(questionsAnswered)) * 100) : 0
        if accuracy >= 80 {
            hostMessage = "Impressive! You clearly know your stuff. Final score: \(score) 🏆"
        } else if accuracy >= 50 {
            hostMessage = "Not bad! Room for improvement though. Final score: \(score)"
        } else {
            hostMessage = "Oof. Maybe review those lecture notes? Final score: \(score) 📚"
        }
    }
    
    func returnToTitle() {
        stopTimer()
        countdownTimer?.invalidate()
        phase = .titleScreen
        hostMessage = ""
    }
    
    // MARK: - Timer
    
    private func startTimer() {
        timer?.invalidate()
        timer = Timer.scheduledTimer(withTimeInterval: 0.05, repeats: true) { [weak self] timer in
            guard let self else { timer.invalidate(); return }
            DispatchQueue.main.async {
                self.timeRemaining -= 0.05
                if self.timeRemaining <= 0 {
                    self.timeRemaining = 0
                    self.timeExpired()
                }
            }
        }
    }
    
    private func stopTimer() {
        timer?.invalidate()
        timer = nil
    }
}
