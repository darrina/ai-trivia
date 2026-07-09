using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using Windows.UI;

namespace AITrivia.Views;

public sealed partial class CountdownPage : Page
{
    public CountdownPage()
    {
        InitializeComponent();
        AppLog.Info("CountdownPage ctor");
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AppLog.Info($"CountdownPage loaded with CountdownValue={App.GameViewModel.CountdownValue}");
        App.GameViewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdateDisplay(App.GameViewModel.CountdownValue);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        AppLog.Info("CountdownPage unloaded");
        App.GameViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameViewModel.CountdownValue))
            UpdateDisplay(App.GameViewModel.CountdownValue);
    }

    private void UpdateDisplay(int value)
    {
        AppLog.Info($"CountdownPage UpdateDisplay value={value}");
        var isGo = value <= 0;
        CountdownText.Text = isGo ? "GO!" : value.ToString();

        // Gradient: cyan/blue for numbers, green/cyan for GO
        GradStop1.Color = isGo
            ? Color.FromArgb(255, 52, 211, 153)   // green
            : Color.FromArgb(255, 0, 255, 255);    // cyan
        GradStop2.Color = isGo
            ? Color.FromArgb(255, 0, 191, 255)     // deep sky blue
            : Color.FromArgb(255, 0, 80, 255);     // blue
        CountdownText.Opacity = 1.0;
    }
}
