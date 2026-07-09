using AITrivia.Controls;
using AITrivia.Views;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;

namespace AITrivia;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        AppLog.Info("MainWindow ctor: InitializeComponent complete");

        // Set window size
        if (AppWindow is { } appWindow)
        {
            appWindow.Resize(new Windows.Graphics.SizeInt32(900, 700));
            appWindow.Title = "AI Trivia";
            AppLog.Info("MainWindow ctor: AppWindow sized and titled");
        }

        SettingsPanel.CloseRequested += (_, _) => SettingsOverlay.Visibility = Visibility.Collapsed;
        AboutPanel.CloseRequested += (_, _) => AboutOverlay.Visibility = Visibility.Collapsed;

        App.GameViewModel.PropertyChanged += OnViewModelPropertyChanged;
        App.GameSettings.PropertyChanged += OnSettingsChanged;

        // Start on title screen
        ContentFrame.Navigate(typeof(TitleScreenPage));
        AppLog.Info("MainWindow ctor: navigated to TitleScreenPage");
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(GameViewModel.Phase)) return;

        AppLog.Info($"MainWindow phase change -> {App.GameViewModel.Phase}");

        switch (App.GameViewModel.Phase)
        {
            case GamePhase.TitleScreen:
                ContentFrame.Navigate(typeof(TitleScreenPage));
                AppLog.Info("MainWindow navigated to TitleScreenPage");
                break;
            case GamePhase.Countdown:
                ContentFrame.Navigate(typeof(CountdownPage));
                AppLog.Info("MainWindow navigated to CountdownPage");
                break;
            case GamePhase.Question:
                // Only navigate if not already on the question page (RevealAnswer stays there)
                if (ContentFrame.Content is not QuestionPage)
                {
                    ContentFrame.Navigate(typeof(QuestionPage));
                    AppLog.Info("MainWindow navigated to QuestionPage");
                }
                break;
            case GamePhase.RevealAnswer:
                // Stay on QuestionPage — it handles this phase
                AppLog.Info("MainWindow staying on QuestionPage for RevealAnswer");
                break;
            case GamePhase.GameOver:
                ContentFrame.Navigate(typeof(GameOverPage));
                AppLog.Info("MainWindow navigated to GameOverPage");
                break;
        }
    }

    private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        AppLog.Info($"MainWindow settings changed: {e.PropertyName}");
        App.GameViewModel.ApplySettings(App.GameSettings);
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        SettingsOverlay.Visibility = Visibility.Visible;
        AppLog.Info("Settings overlay opened");
    }

    private void DimOverlay_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        SettingsOverlay.Visibility = Visibility.Collapsed;
        AboutOverlay.Visibility = Visibility.Collapsed;
    }

    public void ShowAbout()
    {
        AboutOverlay.Visibility = Visibility.Visible;
    }
}
