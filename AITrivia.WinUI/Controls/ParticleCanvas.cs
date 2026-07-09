using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

namespace AITrivia.Controls;

public sealed class ParticleCanvas : UserControl
{
    private readonly Canvas _canvas = new();
    private readonly List<(Ellipse El, ParticleData Data)> _particles = new();
    private readonly DispatcherTimer _timer;
    private double _phase;

    private record struct ParticleData(double NormX, double NormY, double Size, double Opacity, double Speed, double Hue);

    public ParticleCanvas()
    {
        Content = _canvas;
        IsHitTestVisible = false;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _timer.Tick += OnTick;

        Loaded += OnLoaded;
        Unloaded += (_, _) => _timer.Stop();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _canvas.Children.Clear();
        _particles.Clear();

        var rng = Random.Shared;
        for (int i = 0; i < 30; i++)
        {
            var data = new ParticleData(
                NormX: rng.NextDouble(),
                NormY: rng.NextDouble(),
                Size: rng.NextDouble() * 4 + 2,
                Opacity: rng.NextDouble() * 0.3 + 0.1,
                Speed: rng.NextDouble() * 1.2 + 0.3,
                Hue: rng.NextDouble() * 0.35 + 0.5 // cyan → purple range
            );

            var el = new Ellipse
            {
                Width = data.Size,
                Height = data.Size,
                Fill = new SolidColorBrush(HsvToColor(data.Hue, 0.7, 0.9, data.Opacity * 0.6))
            };

            _canvas.Children.Add(el);
            _particles.Add((el, data));
        }

        _timer.Start();
    }

    private void OnTick(object? sender, object e)
    {
        _phase += 0.016 * (Math.PI * 2 / 8.0); // full cycle in 8 seconds
        var width = ActualWidth;
        var height = ActualHeight;

        foreach (var (el, data) in _particles)
        {
            var yOffset = Math.Sin(_phase * data.Speed + data.NormX * Math.PI * 2) * 20;
            Canvas.SetLeft(el, data.NormX * width - data.Size / 2);
            Canvas.SetTop(el, data.NormY * height + yOffset - data.Size / 2);
        }
    }

    private static Color HsvToColor(double hue, double saturation, double value, double opacity)
    {
        int hi = (int)(hue * 6) % 6;
        double f = hue * 6 - Math.Floor(hue * 6);
        double p = value * (1 - saturation);
        double q = value * (1 - f * saturation);
        double t = value * (1 - (1 - f) * saturation);

        var (r, g, b) = hi switch
        {
            0 => (value, t, p),
            1 => (q, value, p),
            2 => (p, value, t),
            3 => (p, q, value),
            4 => (t, p, value),
            _ => (value, p, q)
        };

        return Color.FromArgb(
            (byte)(opacity * 255),
            (byte)(r * 255),
            (byte)(g * 255),
            (byte)(b * 255));
    }
}
