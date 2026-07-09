import SwiftUI

struct CountdownView: View {
    let value: Int
    @State private var scale: CGFloat = 2.0
    @State private var opacity: Double = 0
    
    var body: some View {
        Text(value > 0 ? "\(value)" : "GO!")
            .font(.system(size: 120, weight: .black, design: .rounded))
            .foregroundStyle(
                LinearGradient(
                    colors: value > 0 ? [.cyan, .blue] : [.green, .cyan],
                    startPoint: .top,
                    endPoint: .bottom
                )
            )
            .shadow(color: .cyan.opacity(0.8), radius: 30)
            .scaleEffect(scale)
            .opacity(opacity)
            .onChange(of: value) { _, _ in
                scale = 2.0
                opacity = 0
                withAnimation(.easeOut(duration: 0.5)) {
                    scale = 1.0
                    opacity = 1.0
                }
                withAnimation(.easeIn(duration: 0.3).delay(0.6)) {
                    opacity = 0
                }
            }
            .onAppear {
                withAnimation(.easeOut(duration: 0.5)) {
                    scale = 1.0
                    opacity = 1.0
                }
                withAnimation(.easeIn(duration: 0.3).delay(0.6)) {
                    opacity = 0
                }
            }
    }
}
