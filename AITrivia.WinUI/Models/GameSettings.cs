using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AITrivia;

public partial class GameSettings : ObservableObject
{
    public static readonly IReadOnlyList<string> AllCategories = new[]
    {
        "LLMs and AI Fundamentals",
        "Prompt and Context Engineering",
        "Measuring AI Performance / Metrics and Evaluation",
        "Data Handling & Preprocessing with Pandas",
        "Retrieval Augmented Generation (RAG)",
        "Building an AI Agent",
        "Model Tuning",
        "AI as a CoPilot",
        "DevOps, MLOps, LLMOps"
    };

    [ObservableProperty] private double _questionTime = 10.0;
    [ObservableProperty] private int _countdownDuration = 3;
    [ObservableProperty] private double _maxComboMultiplier = 4.0;
    [ObservableProperty] private double _streakBonusIncrement = 0.5;
    [ObservableProperty] private double _easyMultiplier = 1.0;
    [ObservableProperty] private double _mediumMultiplier = 1.5;
    [ObservableProperty] private double _hardMultiplier = 2.0;
    [ObservableProperty] private HashSet<string> _enabledCategories = new(AllCategories);

    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "aitrivia");
    private static readonly string ConfigFile = Path.Combine(ConfigDir, "config.json");

    private bool _suppressSave;

    public GameSettings()
    {
        Load();
    }

    partial void OnQuestionTimeChanged(double value) => Save();
    partial void OnCountdownDurationChanged(int value) => Save();
    partial void OnMaxComboMultiplierChanged(double value) => Save();
    partial void OnStreakBonusIncrementChanged(double value) => Save();
    partial void OnEasyMultiplierChanged(double value) => Save();
    partial void OnMediumMultiplierChanged(double value) => Save();
    partial void OnHardMultiplierChanged(double value) => Save();
    partial void OnEnabledCategoriesChanged(HashSet<string> value) => Save();

    private void Save()
    {
        if (_suppressSave) return;
        try
        {
            Directory.CreateDirectory(ConfigDir);
            var dto = new SettingsDto(QuestionTime, CountdownDuration, MaxComboMultiplier,
                StreakBonusIncrement, EasyMultiplier, MediumMultiplier, HardMultiplier,
                EnabledCategories.ToList());
            File.WriteAllText(ConfigFile, JsonSerializer.Serialize(dto));
        }
        catch { /* non-critical */ }
    }

    private void Load()
    {
        if (!File.Exists(ConfigFile)) return;
        try
        {
            var dto = JsonSerializer.Deserialize<SettingsDto>(File.ReadAllText(ConfigFile));
            if (dto is null) return;
            _suppressSave = true;
            QuestionTime = dto.QuestionTime;
            CountdownDuration = dto.CountdownDuration;
            MaxComboMultiplier = dto.MaxComboMultiplier;
            StreakBonusIncrement = dto.StreakBonusIncrement;
            EasyMultiplier = dto.EasyMultiplier;
            MediumMultiplier = dto.MediumMultiplier;
            HardMultiplier = dto.HardMultiplier;
            EnabledCategories = new HashSet<string>(dto.EnabledCategories);
            _suppressSave = false;
        }
        catch { /* use defaults */ }
    }

    public void ResetToDefaults()
    {
        QuestionTime = 10.0;
        CountdownDuration = 3;
        MaxComboMultiplier = 4.0;
        StreakBonusIncrement = 0.5;
        EasyMultiplier = 1.0;
        MediumMultiplier = 1.5;
        HardMultiplier = 2.0;
        EnabledCategories = new HashSet<string>(AllCategories);
    }

    private record SettingsDto(
        double QuestionTime, int CountdownDuration, double MaxComboMultiplier,
        double StreakBonusIncrement, double EasyMultiplier, double MediumMultiplier,
        double HardMultiplier, List<string> EnabledCategories);
}
