using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

namespace AITrivia.Controls;

/// <summary>
/// Animated particle background. Uses TranslateTransform for per-frame Y movement so no
/// XAML layout passes are triggered after initial positioning — only cheap render transforms.
/// </summary>
public sealed class ParticleCanvas : UserControl
{
    private readonly Canvas _canvas = new();
    private readonly List<(Ellipse El, ParticleData Data)> _particles = new();
    private readonly DispatcherTimer _timer;
    private double _phase;

    private record struct ParticleData(double X, double BaseY, double Size, double Speed, TranslateTransform Transform);

    public ParticleCanvas()
    {
        Content = _canvas;
        IsHitTestVisible = false;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) }; // ~30fps
        _timer.Tick += OnTick;

        Loaded += OnLoaded;
        Unloaded += (_, _) => _timer.Stop();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _canvas.Children.Clear();
        _particles.Clear();

        var rng = Random.Shared;
        var w = Math.Max(ActualWidth, 800);
        var h = Math.Max(ActualHeight, 600);

        for (int i = 0; i < 20; i++) // reduced from 30 to 20
        {
            var size    = rng.NextDouble() * 4 + 2;
            var normX   = rng.NextDouble();
            var normY   = rng.NextDouble();
            var speed   = rng.NextDouble() * 1.2 + 0.3;
            var opacity = rng.NextDouble() * 0.3 + 0.1;
            var hue     = rng.NextDouble() * 0.35 + 0.5;   // cyan → purple

            var transform = new TranslateTransform();

            var el = new Ellipse
            {
                Width            = size,
                Height           = size,
                Opacity          = opacity * 0.6,
                Fill             = new SolidColorBrush(HsvToColor(hue, 0.7, 0.9, 1.0)),
                RenderTransform  = transform,
                IsHitTestVisible = false
            };

            // Set absolute position once — only Y will move via TranslateTransform
            Canvas.SetLeft(el, normX * w - size / 2);
            Canvas.SetTop(el,  normY * h - size / 2);

            _canvas.Children.Add(el);
            _particles.Add((el, new ParticleData(normX * w, normY * h, size, speed, transform)));
        }

        _timer.Start();
    }

    private void OnTick(object? sender, object e)
    {
        _phase += 0.033 * (Math.PI * 2 / 8.0); // full sine cycle in ~8 s
        foreach (var (_, data) in _particles)
            data.Transform.Y = Math.Sin(_phase * data.Speed + data.X * 0.01) * 20;
    }

    private static Color HsvToColor(double hue, double sat, double val, double alpha)
    {
        int hi = (int)(hue * 6) % 6;
        double f = hue * 6 - Math.Floor(hue * 6);
        double p = val * (1 - sat), q = val * (1 - f * sat), t = val * (1 - (1 - f) * sat);
        var (r, g, b) = hi switch
        {
            0 => (val, t, p), 1 => (q, val, p), 2 => (p, val, t),
            3 => (p, q, val), 4 => (t, p, val), _ => (val, p, q)
        };
        return Color.FromArgb((byte)(alpha * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }
}
