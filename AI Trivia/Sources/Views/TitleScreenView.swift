import SwiftUI

struct TitleScreenView: View {
    let viewModel: GameViewModel
    @State private var titleScale: CGFloat = 0.8
    @State private var titleOpacity: Double = 0
    @State private var glowAmount: Double = 0
    @State private var subtitleOffset: CGFloat = 20
    
    var body: some View {
        VStack(spacing: 40) {
            Spacer()
            
            // Title with glow effect
            VStack(spacing: 8) {
                AppIconView(size: 120)
                    .shadow(color: .cyan.opacity(glowAmount), radius: 20)
                
                Text("AI TRIVIA")
                    .font(.system(size: 64, weight: .black, design: .rounded))
                    .foregroundStyle(
                        LinearGradient(
                            colors: [.cyan, .purple, .pink],
                            startPoint: .leading,
                            endPoint: .trailing
                        )
                    )
                    .shadow(color: .cyan.opacity(0.5), radius: 10)
                    .shadow(color: .purple.opacity(0.3), radius: 20)
                
                Text("Do You Really Know AI?")
                    .font(.system(size: 22, weight: .medium, design: .rounded))
                    .foregroundColor(.white.opacity(0.7))
                    .offset(y: subtitleOffset)
                    .opacity(titleOpacity)
            }
            .scaleEffect(titleScale)
            
            Spacer()
            
            // Start button
            Button(action: { viewModel.startGame() }) {
                HStack(spacing: 12) {
                    Image(systemName: "play.fill")
                        .font(.title2)
                    Text("START GAME")
                        .font(.system(size: 20, weight: .bold, design: .rounded))
                }
                .foregroundColor(.white)
                .padding(.horizontal, 40)
                .padding(.vertical, 16)
                .background(
                    RoundedRectangle(cornerRadius: 16)
                        .fill(
                            LinearGradient(
                                colors: [Color(hex: "e94560"), Color(hex: "533483")],
                                startPoint: .leading,
                                endPoint: .trailing
                            )
                        )
                        .shadow(color: Color(hex: "e94560").opacity(0.5), radius: 15, y: 5)
                )
            }
            .buttonStyle(.plain)
            .scaleEffect(titleScale)
            
            // Instructions
            VStack(spacing: 8) {
                Text("⚡ Answer fast for maximum points")
                Text("🔥 Build streaks for combo multipliers")
                Text("🧠 Questions from your AI/ML lectures")
            }
            .font(.system(size: 14, weight: .medium, design: .rounded))
            .foregroundColor(.white.opacity(0.5))
            .opacity(titleOpacity)
            
            Spacer()
        }
        .onAppear {
            withAnimation(.easeOut(duration: 0.8)) {
                titleScale = 1.0
                titleOpacity = 1.0
                subtitleOffset = 0
            }
            withAnimation(.easeInOut(duration: 2.0).repeatForever(autoreverses: true)) {
                glowAmount = 0.8
            }
        }
    }
}
