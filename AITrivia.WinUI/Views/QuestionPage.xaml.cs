using AITrivia.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using Windows.UI;

namespace AITrivia.Views;

public sealed partial class QuestionPage : Page
{
    private readonly AnswerButton[] _answerButtons = new AnswerButton[4];
    private double _timerBarMaxWidth;

    public QuestionPage()
    {
        InitializeComponent();
        BuildAnswerButtons();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += (_, _) => UpdateTimerBarWidth();
    }

    private void BuildAnswerButtons()
    {
        for (int i = 0; i < 4; i++)
        {
            int idx = i;
            var btn = new AnswerButton { Index = i };
            btn.ButtonTapped += (_, _) => App.GameViewModel.SelectAnswer(idx);
            _answerButtons[i] = btn;
            AnswersPanel.Children.Add(btn);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        App.GameViewModel.PropertyChanged += OnViewModelPropertyChanged;
        _timerBarMaxWidth = TimerBar.ActualWidth > 0 ? TimerBar.ActualWidth : ActualWidth - 60;
        RefreshAll();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        App.GameViewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(GameViewModel.Phase):
                RefreshRevealState();
                break;
            case nameof(GameViewModel.Score):
                ScoreText.Text = App.GameViewModel.Score.ToString("N0");
                break;
            case nameof(GameViewModel.Streak):
                RefreshStreak();
                break;
            case nameof(GameViewModel.TimeRemaining):
                UpdateTimerBarWidth();
                break;
            case nameof(GameViewModel.CurrentQuestion):
                RefreshQuestion();
                break;
            case nameof(GameViewModel.SelectedAnswerIndex):
                RefreshAnswerButtons();
                break;
            case nameof(GameViewModel.QuestionsAnswered):
            case nameof(GameViewModel.CorrectAnswers):
                RefreshStats();
                break;
        }
    }

    private void RefreshAll()
    {
        var vm = App.GameViewModel;
        ScoreText.Text = vm.Score.ToString("N0");
        RefreshStreak();
        RefreshStats();
        RefreshQuestion();
        RefreshAnswerButtons();
        UpdateTimerBarWidth();
        RefreshRevealState();
    }

    private void RefreshStreak()
    {
        var streak = App.GameViewModel.Streak;
        if (streak >= 2)
        {
            StreakBadge.Visibility = Visibility.Visible;
            StreakText.Text = $"{streak}x";
        }
        else
        {
            StreakBadge.Visibility = Visibility.Collapsed;
        }
    }

    private void RefreshStats()
    {
        var vm = App.GameViewModel;
        QuestionNumberText.Text = $"Q{vm.QuestionsAnswered + 1}";
        AccuracyText.Text = $"{vm.CorrectAnswers}/{vm.QuestionsAnswered}";
    }

    private void RefreshQuestion()
    {
        var q = App.GameViewModel.CurrentQuestion;
        if (q is null) return;

        CategoryText.Text = q.Category.ToUpperInvariant();
        QuestionText.Text = q.Question;

        // Difficulty badge
        var (diffColor, diffBg) = q.Difficulty switch
        {
            "easy"   => (Color.FromArgb(255, 52, 211, 153), Color.FromArgb(38, 52, 211, 153)),
            "medium" => (Color.FromArgb(255, 251, 191, 36), Color.FromArgb(38, 251, 191, 36)),
            "hard"   => (Color.FromArgb(255, 239, 68, 68),  Color.FromArgb(38, 239, 68, 68)),
            _        => (Color.FromArgb(255, 156, 163, 175), Color.FromArgb(38, 156, 163, 175))
        };
        DifficultyText.Text = q.Difficulty.ToUpperInvariant();
        DifficultyText.Foreground = new SolidColorBrush(diffColor);
        DifficultyBadge.Background = new SolidColorBrush(diffBg);

        for (int i = 0; i < 4; i++)
        {
            _answerButtons[i].AnswerText = i < q.Answers.Count ? q.Answers[i] : "";
            _answerButtons[i].IsCorrectAnswer = i == q.CorrectIndex;
            _answerButtons[i].IsAnswerSelected = false;
            _answerButtons[i].ShowResult = false;
        }
    }

    private void RefreshAnswerButtons()
    {
        var vm = App.GameViewModel;
        var selected = vm.SelectedAnswerIndex;
        var showResult = vm.Phase == GamePhase.RevealAnswer;
        for (int i = 0; i < 4; i++)
        {
            _answerButtons[i].IsAnswerSelected = selected.HasValue && selected.Value == i;
            _answerButtons[i].ShowResult = showResult;
        }
    }

    private void RefreshRevealState()
    {
        var vm = App.GameViewModel;
        if (vm.Phase == GamePhase.RevealAnswer)
        {
            RevealPanel.Visibility = Visibility.Visible;
            HostMessageText.Text = vm.HostMessage;

            if (vm.LastPointsEarned > 0)
            {
                PointsText.Text = $"+{vm.LastPointsEarned} points!";
                PointsText.Visibility = Visibility.Visible;
            }
            else
            {
                PointsText.Visibility = Visibility.Collapsed;
            }

            if (vm.ShowFunFact && vm.CurrentQuestion is { } q)
            {
                FunFactText.Text = q.FunFact;
                FunFactBorder.Visibility = Visibility.Visible;
            }
            else
            {
                FunFactBorder.Visibility = Visibility.Collapsed;
            }

            RefreshAnswerButtons();
        }
        else
        {
            RevealPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void UpdateTimerBarWidth()
    {
        var parentWidth = ((FrameworkElement)TimerBar.Parent).ActualWidth;
        if (parentWidth <= 0) return;

        var vm = App.GameViewModel;
        var progress = vm.MaxTimeForDisplay > 0
            ? Math.Max(0, Math.Min(1, vm.TimeRemaining / vm.MaxTimeForDisplay))
            : 0;

        TimerBar.Width = parentWidth * progress;

        // Color: green → yellow → red
        var color = progress > 0.6
            ? Color.FromArgb(255, 52, 211, 153)   // green
            : progress > 0.3
                ? Color.FromArgb(255, 251, 191, 36) // yellow
                : Color.FromArgb(255, 239, 68, 68); // red
        TimerBarBrush.Color = color;
    }

    private void QuitButton_Click(object sender, RoutedEventArgs e)
        => App.GameViewModel.EndGame();

    private void NextButton_Click(object sender, RoutedEventArgs e)
        => App.GameViewModel.NextQuestion();
}
