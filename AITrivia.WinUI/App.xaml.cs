using Microsoft.UI.Xaml;

namespace AITrivia;

public partial class App : Application
{
    public static GameViewModel GameViewModel { get; private set; } = null!;
    public static GameSettings GameSettings { get; private set; } = null!;

    private MainWindow? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        GameSettings = new GameSettings();
        GameViewModel = new GameViewModel();

        _window = new MainWindow();
        _window.Activate();
    }
}
