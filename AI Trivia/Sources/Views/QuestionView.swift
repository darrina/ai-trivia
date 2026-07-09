import SwiftUI

struct QuestionView: View {
    @Bindable var viewModel: GameViewModel
    
    var body: some View {
        VStack(spacing: 0) {
            // Top bar - Score and Timer
            HStack {
                // Score
                VStack(alignment: .leading, spacing: 2) {
                    Text("SCORE")
                        .font(.system(size: 11, weight: .bold, design: .rounded))
                        .foregroundColor(.white.opacity(0.5))
                    Text("\(viewModel.score)")
                        .font(.system(size: 28, weight: .black, design: .rounded))
                        .foregroundColor(.white)
                        .contentTransition(.numericText())
                }
                
                Spacer()
                
                // Streak indicator
                if viewModel.streak >= 2 {
                    HStack(spacing: 4) {
                        Image(systemName: "flame.fill")
                            .foregroundColor(.orange)
                        Text("\(viewModel.streak)x")
                            .font(.system(size: 18, weight: .black, design: .rounded))
                            .foregroundColor(.orange)
                        Text("STREAK")
                            .font(.system(size: 11, weight: .bold, design: .rounded))
                            .foregroundColor(.orange.opacity(0.7))
                    }
                    .padding(.horizontal, 12)
                    .padding(.vertical, 6)
                    .background(
                        Capsule()
                            .fill(Color.orange.opacity(0.2))
                            .overlay(Capsule().stroke(Color.orange.opacity(0.5), lineWidth: 1))
                    )
                }
                
                Spacer()
                
                // Stats
                VStack(alignment: .trailing, spacing: 2) {
                    Text("Q\(viewModel.questionsAnswered + 1)")
                        .font(.system(size: 11, weight: .bold, design: .rounded))
                        .foregroundColor(.white.opacity(0.5))
                    Text("\(viewModel.correctAnswers)/\(viewModel.questionsAnswered)")
                        .font(.system(size: 16, weight: .bold, design: .rounded))
                        .foregroundColor(.white.opacity(0.8))
                }
                
                // Quit button
                Button(action: { viewModel.endGame() }) {
                    Image(systemName: "xmark.circle.fill")
                        .font(.title2)
                        .foregroundColor(.white.opacity(0.4))
                }
                .buttonStyle(.plain)
                .padding(.leading, 12)
            }
            .padding(.horizontal, 30)
            .padding(.top, 20)
            .padding(.bottom, 10)
            
            // Timer bar
            TimerBarView(timeRemaining: viewModel.timeRemaining, maxTime: viewModel.maxTimeForDisplay)
                .padding(.horizontal, 30)
                .padding(.bottom, 20)
            
            Spacer()
            
            // Category badge
            if let question = viewModel.currentQuestion {
                Text(question.category.uppercased())
                    .font(.system(size: 12, weight: .bold, design: .rounded))
                    .foregroundColor(.cyan.opacity(0.8))
                    .padding(.horizontal, 16)
                    .padding(.vertical, 6)
                    .background(
                        Capsule()
                            .fill(Color.cyan.opacity(0.1))
                            .overlay(Capsule().stroke(Color.cyan.opacity(0.3), lineWidth: 1))
                    )
                
                // Difficulty badge
                DifficultyBadge(difficulty: question.difficulty)
                    .padding(.top, 4)
                
                // Question text
                Text(question.question)
                    .font(.system(size: 24, weight: .bold, design: .rounded))
                    .foregroundColor(.white)
                    .multilineTextAlignment(.center)
                    .padding(.horizontal, 40)
                    .padding(.top, 20)
                    .padding(.bottom, 30)
                
                // Answer buttons
                VStack(spacing: 12) {
                    ForEach(Array(question.answers.enumerated()), id: \.offset) { index, answer in
                        AnswerButton(
                            text: answer,
                            index: index,
                            isSelected: viewModel.selectedAnswerIndex == index,
                            isCorrect: index == question.correctIndex,
                            showResult: viewModel.phase == .revealAnswer,
                            action: { viewModel.selectAnswer(index) }
                        )
                    }
                }
                .padding(.horizontal, 40)
            }
            
            Spacer()
            
            // Host message & fun fact
            if viewModel.phase == .revealAnswer {
                VStack(spacing: 12) {
                    // Points earned
                    if viewModel.lastPointsEarned > 0 {
                        Text("+\(viewModel.lastPointsEarned) points!")
                            .font(.system(size: 18, weight: .black, design: .rounded))
                            .foregroundColor(.green)
                            .transition(.scale.combined(with: .opacity))
                    }
                    
                    // Host message
                    Text(viewModel.hostMessage)
                        .font(.system(size: 16, weight: .medium, design: .rounded))
                        .foregroundColor(.white.opacity(0.9))
                        .multilineTextAlignment(.center)
                        .padding(.horizontal, 40)
                        .transition(.opacity)
                    
                    // Fun fact
                    if viewModel.showFunFact, let fact = viewModel.currentQuestion?.funFact {
                        HStack(spacing: 8) {
                            Image(systemName: "lightbulb.fill")
                                .foregroundColor(.yellow)
                            Text(fact)
                                .font(.system(size: 13, weight: .medium, design: .rounded))
                                .foregroundColor(.white.opacity(0.7))
                        }
                        .padding(.horizontal, 20)
                        .padding(.vertical, 10)
                        .background(
                            RoundedRectangle(cornerRadius: 10)
                                .fill(Color.yellow.opacity(0.1))
                                .overlay(RoundedRectangle(cornerRadius: 10).stroke(Color.yellow.opacity(0.2), lineWidth: 1))
                        )
                        .transition(.move(edge: .bottom).combined(with: .opacity))
                    }
                    
                    // Next button
                    Button(action: { viewModel.nextQuestion() }) {
                        HStack {
                            Text("NEXT QUESTION")
                                .font(.system(size: 14, weight: .bold, design: .rounded))
                            Image(systemName: "arrow.right")
                        }
                        .foregroundColor(.white)
                        .padding(.horizontal, 24)
                        .padding(.vertical, 10)
                        .background(
                            Capsule()
                                .fill(Color(hex: "533483"))
                        )
                    }
                    .buttonStyle(.plain)
                    .padding(.top, 8)
                }
                .animation(.easeOut(duration: 0.3), value: viewModel.showFunFact)
                .padding(.bottom, 20)
            }
        }
    }
}

// MARK: - Timer Bar

struct TimerBarView: View {
    let timeRemaining: Double
    let maxTime: Double
    
    var progress: Double { timeRemaining / maxTime }
    
    var barColor: Color {
        if progress > 0.6 { return .green }
        if progress > 0.3 { return .yellow }
        return .red
    }
    
    var body: some View {
        GeometryReader { geo in
            ZStack(alignment: .leading) {
                RoundedRectangle(cornerRadius: 4)
                    .fill(Color.white.opacity(0.1))
                
                RoundedRectangle(cornerRadius: 4)
                    .fill(barColor)
                    .frame(width: geo.size.width * max(0, progress))
                    .shadow(color: barColor.opacity(0.5), radius: 5)
            }
        }
        .frame(height: 8)
    }
}

// MARK: - Difficulty Badge

struct DifficultyBadge: View {
    let difficulty: String
    
    var color: Color {
        switch difficulty {
        case "easy": return .green
        case "medium": return .yellow
        case "hard": return .red
        default: return .gray
        }
    }
    
    var body: some View {
        Text(difficulty.uppercased())
            .font(.system(size: 10, weight: .bold, design: .rounded))
            .foregroundColor(color)
            .padding(.horizontal, 8)
            .padding(.vertical, 3)
            .background(
                Capsule()
                    .fill(color.opacity(0.15))
                    .overlay(Capsule().stroke(color.opacity(0.4), lineWidth: 1))
            )
    }
}

// MARK: - Answer Button

struct AnswerButton: View {
    let text: String
    let index: Int
    let isSelected: Bool
    let isCorrect: Bool
    let showResult: Bool
    let action: () -> Void
    
    @State private var isHovered: Bool = false
    
    private let prefixes = ["A", "B", "C", "D"]
    
    var backgroundColor: Color {
        if showResult {
            if isCorrect { return Color.green.opacity(0.3) }
            if isSelected && !isCorrect { return Color.red.opacity(0.3) }
            return Color.white.opacity(0.05)
        }
        if isHovered { return Color.white.opacity(0.15) }
        return Color.white.opacity(0.08)
    }
    
    var borderColor: Color {
        if showResult {
            if isCorrect { return .green }
            if isSelected && !isCorrect { return .red }
            return Color.white.opacity(0.1)
        }
        if isHovered { return Color.cyan.opacity(0.5) }
        return Color.white.opacity(0.15)
    }
    
    var body: some View {
        Button(action: action) {
            HStack(spacing: 16) {
                Text(prefixes[index])
                    .font(.system(size: 14, weight: .black, design: .rounded))
                    .foregroundColor(showResult && isCorrect ? .green : .cyan)
                    .frame(width: 28, height: 28)
                    .background(
                        Circle()
                            .fill(showResult && isCorrect ? Color.green.opacity(0.2) : Color.cyan.opacity(0.15))
                    )
                
                Text(text)
                    .font(.system(size: 16, weight: .semibold, design: .rounded))
                    .foregroundColor(.white)
                    .multilineTextAlignment(.leading)
                
                Spacer()
                
                if showResult {
                    if isCorrect {
                        Image(systemName: "checkmark.circle.fill")
                            .foregroundColor(.green)
                            .font(.title3)
                    } else if isSelected {
                        Image(systemName: "xmark.circle.fill")
                            .foregroundColor(.red)
                            .font(.title3)
                    }
                }
            }
            .padding(.horizontal, 20)
            .padding(.vertical, 14)
            .background(
                RoundedRectangle(cornerRadius: 12)
                    .fill(backgroundColor)
                    .overlay(
                        RoundedRectangle(cornerRadius: 12)
                            .stroke(borderColor, lineWidth: showResult && (isCorrect || isSelected) ? 2 : 1)
                    )
            )
        }
        .buttonStyle(.plain)
        .disabled(showResult)
        .onHover { hovering in
            withAnimation(.easeOut(duration: 0.15)) {
                isHovered = hovering
            }
        }
        .scaleEffect(isHovered && !showResult ? 1.02 : 1.0)
        .animation(.easeOut(duration: 0.15), value: isHovered)
    }
}
