import SwiftUI

struct GameOverView: View {
    let viewModel: GameViewModel
    @State private var showContent = false
    
    var accuracy: Int {
        viewModel.questionsAnswered > 0
            ? Int((Double(viewModel.correctAnswers) / Double(viewModel.questionsAnswered)) * 100)
            : 0
    }
    
    var grade: String {
        switch accuracy {
        case 90...100: return "🏆 AI Overlord"
        case 80..<90: return "🎓 ML Engineer"
        case 70..<80: return "💻 Data Scientist"
        case 60..<70: return "📊 Analyst"
        case 50..<60: return "📖 Student"
        default: return "🤔 Needs More Training Data"
        }
    }
    
    var body: some View {
        VStack(spacing: 30) {
            Spacer()
            
            Text("GAME OVER")
                .font(.system(size: 48, weight: .black, design: .rounded))
                .foregroundStyle(
                    LinearGradient(
                        colors: [.pink, .purple],
                        startPoint: .leading,
                        endPoint: .trailing
                    )
                )
                .opacity(showContent ? 1 : 0)
                .offset(y: showContent ? 0 : -20)
            
            // Grade
            Text(grade)
                .font(.system(size: 28, weight: .bold, design: .rounded))
                .foregroundColor(.white)
                .opacity(showContent ? 1 : 0)
            
            // Stats
            HStack(spacing: 30) {
                StatBox(title: "SCORE", value: "\(viewModel.score)", color: .cyan)
                StatBox(title: "ACCURACY", value: "\(accuracy)%", color: .green)
                StatBox(title: "BEST STREAK", value: "\(viewModel.bestStreak)x", color: .orange)
                StatBox(title: "QUESTIONS", value: "\(viewModel.questionsAnswered)", color: .purple)
            }
            .opacity(showContent ? 1 : 0)
            .offset(y: showContent ? 0 : 20)
            
            // Host message
            Text(viewModel.hostMessage)
                .font(.system(size: 16, weight: .medium, design: .rounded))
                .foregroundColor(.white.opacity(0.7))
                .multilineTextAlignment(.center)
                .padding(.horizontal, 60)
                .opacity(showContent ? 1 : 0)
            
            Spacer()
            
            // Buttons
            HStack(spacing: 20) {
                Button(action: { viewModel.startGame() }) {
                    HStack(spacing: 8) {
                        Image(systemName: "arrow.counterclockwise")
                        Text("PLAY AGAIN")
                    }
                    .font(.system(size: 16, weight: .bold, design: .rounded))
                    .foregroundColor(.white)
                    .padding(.horizontal, 30)
                    .padding(.vertical, 14)
                    .background(
                        RoundedRectangle(cornerRadius: 12)
                            .fill(
                                LinearGradient(
                                    colors: [Color(hex: "e94560"), Color(hex: "533483")],
                                    startPoint: .leading,
                                    endPoint: .trailing
                                )
                            )
                            .shadow(color: Color(hex: "e94560").opacity(0.4), radius: 10)
                    )
                }
                .buttonStyle(.plain)
                
                Button(action: { viewModel.returnToTitle() }) {
                    HStack(spacing: 8) {
                        Image(systemName: "house.fill")
                        Text("MAIN MENU")
                    }
                    .font(.system(size: 16, weight: .bold, design: .rounded))
                    .foregroundColor(.white.opacity(0.8))
                    .padding(.horizontal, 30)
                    .padding(.vertical, 14)
                    .background(
                        RoundedRectangle(cornerRadius: 12)
                            .fill(Color.white.opacity(0.1))
                            .overlay(RoundedRectangle(cornerRadius: 12).stroke(Color.white.opacity(0.2), lineWidth: 1))
                    )
                }
                .buttonStyle(.plain)
            }
            .opacity(showContent ? 1 : 0)
            
            Spacer()
        }
        .onAppear {
            withAnimation(.easeOut(duration: 0.6)) {
                showContent = true
            }
        }
        .onDisappear {
            showContent = false
        }
    }
}

struct StatBox: View {
    let title: String
    let value: String
    let color: Color
    
    var body: some View {
        VStack(spacing: 8) {
            Text(value)
                .font(.system(size: 32, weight: .black, design: .rounded))
                .foregroundColor(color)
            Text(title)
                .font(.system(size: 11, weight: .bold, design: .rounded))
                .foregroundColor(.white.opacity(0.5))
        }
        .padding(.horizontal, 20)
        .padding(.vertical, 16)
        .background(
            RoundedRectangle(cornerRadius: 12)
                .fill(color.opacity(0.1))
                .overlay(RoundedRectangle(cornerRadius: 12).stroke(color.opacity(0.3), lineWidth: 1))
        )
    }
}
