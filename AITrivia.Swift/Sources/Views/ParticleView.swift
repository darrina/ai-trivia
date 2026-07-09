import SwiftUI

struct Particle: Identifiable {
    let id = UUID()
    var x: CGFloat
    var y: CGFloat
    var size: CGFloat
    var opacity: Double
    var speed: CGFloat
    var hue: Double
}

struct ParticleView: View {
    @State private var particles: [Particle] = []
    @State private var animationPhase: Double = 0
    
    var body: some View {
        Canvas { context, size in
            for particle in particles {
                let yOffset = sin(animationPhase * particle.speed + Double(particle.x)) * 20
                let point = CGPoint(
                    x: particle.x * size.width,
                    y: (particle.y * size.height) + yOffset
                )
                
                context.opacity = particle.opacity * 0.6
                context.fill(
                    Circle().path(in: CGRect(
                        x: point.x - particle.size / 2,
                        y: point.y - particle.size / 2,
                        width: particle.size,
                        height: particle.size
                    )),
                    with: .color(Color(hue: particle.hue, saturation: 0.7, brightness: 0.9))
                )
            }
        }
        .onAppear {
            // Generate particles
            particles = (0..<30).map { _ in
                Particle(
                    x: CGFloat.random(in: 0...1),
                    y: CGFloat.random(in: 0...1),
                    size: CGFloat.random(in: 2...6),
                    opacity: Double.random(in: 0.1...0.4),
                    speed: CGFloat.random(in: 0.3...1.5),
                    hue: Double.random(in: 0.5...0.85) // Cyan to purple range
                )
            }
            
            // Animate
            withAnimation(.linear(duration: 8).repeatForever(autoreverses: false)) {
                animationPhase = .pi * 2
            }
        }
    }
}
