using Microsoft.UI.Xaml.Controls;

namespace AITrivia.Views;

public sealed partial class TitleScreenPage : Page
{
    public TitleScreenPage()
    {
        InitializeComponent();
        AppLog.Info("TitleScreenPage ctor");
    }

    private void StartButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        AppLog.Info("TitleScreenPage Start clicked");
        App.GameViewModel.ApplySettings(App.GameSettings);
        AppLog.Info("TitleScreenPage applied settings");
        App.GameViewModel.StartGame();
        AppLog.Info("TitleScreenPage StartGame returned");
    }
}
