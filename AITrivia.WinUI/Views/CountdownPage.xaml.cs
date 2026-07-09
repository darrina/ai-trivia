using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.ComponentModel;
using Windows.UI;

namespace AITrivia.Views;

public sealed partial class CountdownPage : Page
{
    private Storyboard? _currentStoryboard;

    public CountdownPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        App.GameViewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdateDisplay(App.GameViewModel.CountdownValue);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        App.GameViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _currentStoryboard?.Stop();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameViewModel.CountdownValue))
            UpdateDisplay(App.GameViewModel.CountdownValue);
    }

    private void UpdateDisplay(int value)
    {
        var isGo = value <= 0;
        CountdownText.Text = isGo ? "GO!" : value.ToString();

        // Gradient: cyan/blue for numbers, green/cyan for GO
        GradStop1.Color = isGo
            ? Color.FromArgb(255, 52, 211, 153)   // green
            : Color.FromArgb(255, 0, 255, 255);    // cyan
        GradStop2.Color = isGo
            ? Color.FromArgb(255, 0, 191, 255)     // deep sky blue
            : Color.FromArgb(255, 0, 80, 255);     // blue

        AnimateIn();
    }

    private void AnimateIn()
    {
        _currentStoryboard?.Stop();

        var sb = new Storyboard();

        // Scale from 2x to 1x
        var scaleXAnim = new DoubleAnimation
        {
            From = 1.8, To = 1.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(450)),
            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 4 }
        };
        var scaleYAnim = new DoubleAnimation
        {
            From = 1.8, To = 1.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(450)),
            EasingFunction = new ExponentialEase { EasingMode = EasingMode.EaseOut, Exponent = 4 }
        };

        // Opacity in
        var opacityIn = new DoubleAnimation
        {
            From = 0, To = 1.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(300))
        };

        // Opacity out (delayed)
        var opacityOut = new DoubleAnimation
        {
            From = 1.0, To = 0,
            BeginTime = TimeSpan.FromMilliseconds(600),
            Duration = new Duration(TimeSpan.FromMilliseconds(250))
        };

        if (CountdownText.RenderTransform is not Microsoft.UI.Xaml.Media.ScaleTransform)
            CountdownText.RenderTransform = new Microsoft.UI.Xaml.Media.ScaleTransform { ScaleX = 1, ScaleY = 1 };
        CountdownText.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);

        Storyboard.SetTarget(scaleXAnim, CountdownText.RenderTransform);
        Storyboard.SetTargetProperty(scaleXAnim, "ScaleX");
        Storyboard.SetTarget(scaleYAnim, CountdownText.RenderTransform);
        Storyboard.SetTargetProperty(scaleYAnim, "ScaleY");
        Storyboard.SetTarget(opacityIn, CountdownText);
        Storyboard.SetTargetProperty(opacityIn, "Opacity");
        Storyboard.SetTarget(opacityOut, CountdownText);
        Storyboard.SetTargetProperty(opacityOut, "Opacity");

        sb.Children.Add(scaleXAnim);
        sb.Children.Add(scaleYAnim);
        sb.Children.Add(opacityIn);
        sb.Children.Add(opacityOut);

        _currentStoryboard = sb;
        sb.Begin();
    }
}
