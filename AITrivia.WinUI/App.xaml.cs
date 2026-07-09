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
        UnhandledException += OnUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        AppLog.Info($"App constructed. Log file: {AppLog.LogPath}");
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppLog.Info("OnLaunched: begin");
        GameSettings = new GameSettings();
        AppLog.Info("OnLaunched: GameSettings created");
        GameViewModel = new GameViewModel();
        AppLog.Info("OnLaunched: GameViewModel created");

        _window = new MainWindow();
        AppLog.Info("OnLaunched: MainWindow created");
        _window.Activate();
        AppLog.Info("OnLaunched: MainWindow activated");
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        AppLog.Error($"Application.UnhandledException handled={e.Handled}", e.Exception);
    }

    private void OnCurrentDomainUnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        AppLog.Error($"AppDomain.CurrentDomain.UnhandledException terminating={e.IsTerminating}", e.ExceptionObject as Exception);
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        AppLog.Error("TaskScheduler.UnobservedTaskException", e.Exception);
    }
}
