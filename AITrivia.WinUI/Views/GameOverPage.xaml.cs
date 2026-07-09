using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AITrivia.Views;

public sealed partial class GameOverPage : Page
{
    public GameOverPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var vm = App.GameViewModel;

        var accuracy = vm.QuestionsAnswered > 0
            ? (int)(vm.CorrectAnswers * 100.0 / vm.QuestionsAnswered) : 0;

        GradeText.Text = accuracy switch
        {
            >= 90 => "🏆 AI Overlord",
            >= 80 => "🎓 ML Engineer",
            >= 70 => "💻 Data Scientist",
            >= 60 => "📊 Analyst",
            >= 50 => "📖 Student",
            _     => "🤔 Needs More Training Data"
        };

        ScoreValue.Text    = vm.Score.ToString("N0");
        AccuracyValue.Text = $"{accuracy}%";
        StreakValue.Text   = $"{vm.BestStreak}x";
        QuestionsValue.Text = vm.QuestionsAnswered.ToString();
        HostMessage.Text   = vm.HostMessage;
    }

    private void PlayAgain_Click(object sender, RoutedEventArgs e)
    {
        App.GameViewModel.ApplySettings(App.GameSettings);
        App.GameViewModel.StartGame();
    }

    private void MainMenu_Click(object sender, RoutedEventArgs e)
        => App.GameViewModel.ReturnToTitle();
}
