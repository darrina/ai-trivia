using AITrivia.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace AITrivia;

public enum GamePhase { TitleScreen, Countdown, Question, RevealAnswer, GameOver }

public partial class GameViewModel : ObservableObject
{
    // ── Game State ──────────────────────────────────────────────────────────
    [ObservableProperty] private GamePhase _phase = GamePhase.TitleScreen;
    [ObservableProperty] private TriviaQuestion? _currentQuestion;
    [ObservableProperty] private int? _selectedAnswerIndex;
    [ObservableProperty] private int _score;
    [ObservableProperty] private int _streak;
    [ObservableProperty] private int _bestStreak;
    [ObservableProperty] private int _questionsAnswered;
    [ObservableProperty] private int _correctAnswers;
    [ObservableProperty] private double _timeRemaining = 10.0;
    [ObservableProperty] private int _countdownValue = 3;
    [ObservableProperty] private bool _showFunFact;
    [ObservableProperty] private int _lastPointsEarned;
    [ObservableProperty] private double _comboMultiplier = 1.0;
    [ObservableProperty] private string _hostMessage = "";

    public double MaxTimeForDisplay => _maxTime;

    // ── Private ─────────────────────────────────────────────────────────────
    private HashSet<int> _usedQuestionIds = new();
    private readonly TriviaBank _triviaBank;
    private DispatcherTimer _questionTimer;
    private DispatcherTimer _countdownTimer;

    private double _maxTime = 10.0;
    private int _countdownStart = 3;
    private double _maxCombo = 4.0;
    private double _streakIncrement = 0.5;
    private Dictionary<string, double> _difficultyMultipliers = new() { ["easy"] = 1.0, ["medium"] = 1.5, ["hard"] = 2.0 };
    private HashSet<string> _enabledCategories = new(GameSettings.AllCategories);

    private static readonly string[] CorrectMessages =
    [
        "Well, well, well... someone's been studying!",
        "Look at the big brain on you! 🧠",
        "Ding ding ding! We have a winner!",
        "Somebody paid attention in class!",
        "You're on FIRE! 🔥 (Metaphorically. Please don't actually be on fire.)",
        "Nailed it! Your neural network is clearly well-trained.",
        "Correct! Your gradient is definitely not vanishing.",
        "RIGHT! You're basically a fine-tuned model at this point.",
        "YES! That answer was more accurate than GPT-4's hallucinations.",
        "Boom! Knowledge bomb detonated successfully. 💣",
        "Are you cheating? Because that was suspiciously fast...",
        "Your accuracy is approaching 1.0! Keep it up!",
    ];

    private static readonly string[] WrongMessages =
    [
        "Ooof. That one's gonna leave a mark. 😬",
        "Wrong! But hey, even GPT hallucinates sometimes.",
        "Nope! Your loss function just spiked dramatically.",
        "Incorrect! Time to retrain that mental model.",
        "Swing and a miss! Your confidence was high but your accuracy... not so much.",
        "WRONG! That answer had more errors than unhandled exceptions.",
        "Yikes. Even a random forest would've gotten that one right.",
        "Nah. That's what we call 'catastrophic forgetting' in the biz.",
        "Not even close! Were you trained on corrupted data?",
        "Wrong! But don't worry, this is just your validation set. 📉",
        "Oops! Your attention mechanism clearly wasn't paying attention.",
        "Incorrect! That's going straight into your error log.",
    ];

    private static readonly string[] TimeoutMessages =
    [
        "Time's up! Your inference speed needs work. ⏰",
        "Too slow! Even a CPU-only model is faster than you.",
        "Tick tock! The timer waited for no one.",
        "TIME OUT! Your latency is unacceptable in production.",
        "Gone! That question has left the chat. 👋",
        "Expired! Like a deprecated API endpoint.",
    ];

    public GameViewModel()
    {
        AppLog.Info("GameViewModel ctor: begin");
        _triviaBank = TriviaBank.Load();

        _questionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        _questionTimer.Tick += OnQuestionTimerTick;

        _countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _countdownTimer.Tick += OnCountdownTimerTick;
        AppLog.Info("GameViewModel ctor: timers created");
    }

    // ── Settings ─────────────────────────────────────────────────────────────
    public void ApplySettings(GameSettings settings)
    {
        AppLog.Info($"GameViewModel.ApplySettings questionTime={settings.QuestionTime} countdown={settings.CountdownDuration} categories={settings.EnabledCategories.Count}");
        _maxTime = settings.QuestionTime;
        _countdownStart = settings.CountdownDuration;
        _maxCombo = settings.MaxComboMultiplier;
        _streakIncrement = settings.StreakBonusIncrement;
        _difficultyMultipliers = new Dictionary<string, double>
        {
            ["easy"] = settings.EasyMultiplier,
            ["medium"] = settings.MediumMultiplier,
            ["hard"] = settings.HardMultiplier
        };
        _enabledCategories = new HashSet<string>(settings.EnabledCategories);
    }

    // ── Game Flow ─────────────────────────────────────────────────────────────
    [RelayCommand]
    public void StartGame()
    {
        AppLog.Info("GameViewModel.StartGame begin");
        Score = 0; Streak = 0; BestStreak = 0;
        QuestionsAnswered = 0; CorrectAnswers = 0;
        _usedQuestionIds = new();
        ComboMultiplier = 1.0;
        HostMessage = "Let's see what you've got, hotshot! 🎯";
        StartCountdown();
        AppLog.Info("GameViewModel.StartGame end");
    }

    private void StartCountdown()
    {
        AppLog.Info($"GameViewModel.StartCountdown startValue={_countdownStart}");
        Phase = GamePhase.Countdown;
        CountdownValue = _countdownStart;
        _countdownTimer.Stop();
        _countdownTimer.Start();
    }

    private void OnCountdownTimerTick(object? sender, object e)
    {
        AppLog.Info($"GameViewModel.OnCountdownTimerTick before={CountdownValue}");
        CountdownValue--;
        if (CountdownValue <= 0)
        {
            _countdownTimer.Stop();
            AppLog.Info("GameViewModel.OnCountdownTimerTick transitioning to NextQuestion");
            NextQuestion();
        }
    }

    [RelayCommand]
    public void NextQuestion()
    {
        AppLog.Info("GameViewModel.NextQuestion begin");
        SelectedAnswerIndex = null;
        ShowFunFact = false;
        LastPointsEarned = 0;

        var question = _triviaBank.RandomQuestion(_usedQuestionIds, _enabledCategories);
        if (question is null) { _usedQuestionIds.Clear(); question = _triviaBank.RandomQuestion(_usedQuestionIds, _enabledCategories); }
        if (question is null)
        {
            AppLog.Info("GameViewModel.NextQuestion no question available");
            return;
        }

        CurrentQuestion = question;
        _usedQuestionIds.Add(question.Id);
        TimeRemaining = _maxTime;
        Phase = GamePhase.Question;
        StartTimer();
        AppLog.Info($"GameViewModel.NextQuestion end id={question.Id}");
    }

    [RelayCommand]
    public void SelectAnswer(int index)
    {
        if (Phase != GamePhase.Question || SelectedAnswerIndex.HasValue) return;

        StopTimer();
        SelectedAnswerIndex = index;
        QuestionsAnswered++;

        var isCorrect = index == CurrentQuestion?.CorrectIndex;
        if (isCorrect)
        {
            var timeBonus = (int)(TimeRemaining * 100);
            var diffMult = _difficultyMultipliers.GetValueOrDefault(CurrentQuestion?.Difficulty ?? "easy", 1.0);
            Streak++;
            if (Streak > BestStreak) BestStreak = Streak;
            ComboMultiplier = Math.Min(_maxCombo, 1.0 + (Streak - 1) * _streakIncrement);
            var points = (int)(timeBonus * diffMult * ComboMultiplier);
            LastPointsEarned = points;
            Score += points;
            CorrectAnswers++;
            HostMessage = CorrectMessages[Random.Shared.Next(CorrectMessages.Length)];
            if (Streak >= 3) HostMessage += $" 🔥 {Streak}x STREAK!";
        }
        else
        {
            Streak = 0;
            ComboMultiplier = 1.0;
            LastPointsEarned = 0;
            HostMessage = WrongMessages[Random.Shared.Next(WrongMessages.Length)];
        }

        Phase = GamePhase.RevealAnswer;
        ShowFunFact = true;
    }

    private void TimeExpired()
    {
        if (Phase != GamePhase.Question) return;
        StopTimer();
        QuestionsAnswered++;
        Streak = 0;
        ComboMultiplier = 1.0;
        LastPointsEarned = 0;
        HostMessage = TimeoutMessages[Random.Shared.Next(TimeoutMessages.Length)];
        Phase = GamePhase.RevealAnswer;
        ShowFunFact = true;
    }

    [RelayCommand]
    public void EndGame()
    {
        StopTimer();
        _countdownTimer.Stop();
        Phase = GamePhase.GameOver;
        var accuracy = QuestionsAnswered > 0
            ? (int)(CorrectAnswers * 100.0 / QuestionsAnswered) : 0;
        HostMessage = accuracy >= 80
            ? $"Impressive! You clearly know your stuff. Final score: {Score} 🏆"
            : accuracy >= 50
                ? $"Not bad! Room for improvement though. Final score: {Score}"
                : $"Oof. Maybe review those lecture notes? Final score: {Score} 📚";
    }

    [RelayCommand]
    public void ReturnToTitle()
    {
        StopTimer();
        _countdownTimer.Stop();
        Phase = GamePhase.TitleScreen;
        HostMessage = "";
    }

    // ── Timer ─────────────────────────────────────────────────────────────────
    private void StartTimer()
    {
        _questionTimer.Stop();
        _questionTimer.Start();
        AppLog.Info("GameViewModel.StartTimer");
    }

    private void StopTimer() => _questionTimer.Stop();

    private void OnQuestionTimerTick(object? sender, object e)
    {
        TimeRemaining -= 0.05;
        if (TimeRemaining <= 0)
        {
            TimeRemaining = 0;
            TimeExpired();
        }
    }
}
