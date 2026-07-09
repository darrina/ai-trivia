import SwiftUI

struct AppIconView: View {
    var size: CGFloat = 120
    
    var body: some View {
        if let url = Bundle.module.url(forResource: "AppIcon", withExtension: "png"),
           let nsImage = NSImage(contentsOf: url) {
            Image(nsImage: nsImage)
                .resizable()
                .aspectRatio(contentMode: .fit)
                .frame(width: size, height: size)
                .clipShape(RoundedRectangle(cornerRadius: size * 0.2))
                .shadow(color: .cyan.opacity(0.4), radius: 15)
        } else {
            // Fallback
            Image(systemName: "brain.head.profile")
                .font(.system(size: size * 0.6))
                .foregroundColor(.cyan)
        }
    }
}
