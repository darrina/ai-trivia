using Microsoft.UI.Xaml.Controls;

namespace AITrivia.Views;

public sealed partial class TitleScreenPage : Page
{
    public TitleScreenPage()
    {
        InitializeComponent();
    }

    private void StartButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        App.GameViewModel.ApplySettings(App.GameSettings);
        App.GameViewModel.StartGame();
    }
}
