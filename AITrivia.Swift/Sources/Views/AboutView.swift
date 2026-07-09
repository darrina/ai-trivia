import SwiftUI

struct AboutView: View {
    var onClose: () -> Void
    @State private var appeared = false
    
    var body: some View {
        VStack(spacing: 20) {
            // App icon
            AppIconView(size: 80)
                .padding(.top, 24)
            
            Text("AI Trivia")
                .font(.system(size: 24, weight: .bold, design: .rounded))
                .foregroundColor(.white)
            
            Text("Version 1.0")
                .font(.system(size: 13, weight: .medium, design: .rounded))
                .foregroundColor(.white.opacity(0.5))
            
            Divider().overlay(Color.white.opacity(0.1)).padding(.horizontal, 30)
            
            VStack(spacing: 10) {
                Text("A fast-paced trivia game covering AI & Machine Learning topics.")
                    .multilineTextAlignment(.center)
                
                Text("Answer fast. Build streaks. Earn combos. Prove you know AI.")
                    .multilineTextAlignment(.center)
            }
            .font(.system(size: 13, weight: .medium, design: .rounded))
            .foregroundColor(.white.opacity(0.7))
            .padding(.horizontal, 30)
            
            Divider().overlay(Color.white.opacity(0.1)).padding(.horizontal, 30)
            
            VStack(spacing: 4) {
                Text("Built with SwiftUI")
                    .font(.system(size: 11, design: .rounded))
                    .foregroundColor(.white.opacity(0.4))
                Text("© 2026")
                    .font(.system(size: 11, design: .rounded))
                    .foregroundColor(.white.opacity(0.4))
            }
            
            Button(action: onClose) {
                Text("OK")
                    .font(.system(size: 13, weight: .semibold, design: .rounded))
                    .foregroundColor(.white)
                    .padding(.horizontal, 30)
                    .padding(.vertical, 8)
                    .background(
                        RoundedRectangle(cornerRadius: 8)
                            .fill(Color(hex: "533483"))
                    )
            }
            .buttonStyle(.plain)
            .keyboardShortcut(.return, modifiers: [])
            .padding(.bottom, 20)
        }
        .frame(width: 360)
        .background(
            RoundedRectangle(cornerRadius: 16)
                .fill(Color(hex: "1a1a2e").opacity(0.98))
                .overlay(RoundedRectangle(cornerRadius: 16).stroke(Color.white.opacity(0.1), lineWidth: 1))
                .shadow(color: .black.opacity(0.5), radius: 30)
        )
        .scaleEffect(appeared ? 1.0 : 0.9)
        .opacity(appeared ? 1.0 : 0)
        .onAppear {
            withAnimation(.easeOut(duration: 0.2)) {
                appeared = true
            }
        }
        .onDisappear { appeared = false }
    }
}
