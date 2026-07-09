using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AITrivia;

public partial class GameSettings : ObservableObject
{
    private const double DefaultQuestionTime = 10.0;
    private const int DefaultCountdownDuration = 3;
    private const double DefaultMaxComboMultiplier = 4.0;
    private const double DefaultStreakBonusIncrement = 0.5;
    private const double DefaultEasyMultiplier = 1.0;
    private const double DefaultMediumMultiplier = 1.5;
    private const double DefaultHardMultiplier = 2.0;

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

    [ObservableProperty] private double _questionTime = DefaultQuestionTime;
    [ObservableProperty] private int _countdownDuration = DefaultCountdownDuration;
    [ObservableProperty] private double _maxComboMultiplier = DefaultMaxComboMultiplier;
    [ObservableProperty] private double _streakBonusIncrement = DefaultStreakBonusIncrement;
    [ObservableProperty] private double _easyMultiplier = DefaultEasyMultiplier;
    [ObservableProperty] private double _mediumMultiplier = DefaultMediumMultiplier;
    [ObservableProperty] private double _hardMultiplier = DefaultHardMultiplier;
    [ObservableProperty] private HashSet<string> _enabledCategories = new(AllCategories);

    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "aitrivia");
    private static readonly string ConfigFile = Path.Combine(ConfigDir, "config.json");
    private static readonly string LegacyConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "aitrivia");
    private static readonly string LegacyConfigFile = Path.Combine(LegacyConfigDir, "config.json");

    private bool _suppressSave;

    public GameSettings()
    {
        AppLog.Info("GameSettings ctor");
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
            AppLog.Info($"GameSettings saved to {ConfigFile}");
        }
        catch (Exception ex)
        {
            AppLog.Error("GameSettings.Save failed", ex);
        }
    }

    private void Load()
    {
        var sourceFile = ResolveConfigFileToLoad();
        if (sourceFile is null)
        {
            AppLog.Info($"GameSettings.Load: config not found at {ConfigFile}");
            return;
        }
        try
        {
            var dto = JsonSerializer.Deserialize<SettingsDto>(File.ReadAllText(sourceFile));
            if (dto is null) return;
            _suppressSave = true;
            QuestionTime = dto.QuestionTime > 0 ? dto.QuestionTime : DefaultQuestionTime;
            CountdownDuration = dto.CountdownDuration > 0 ? dto.CountdownDuration : DefaultCountdownDuration;
            MaxComboMultiplier = dto.MaxComboMultiplier > 0 ? dto.MaxComboMultiplier : DefaultMaxComboMultiplier;
            StreakBonusIncrement = dto.StreakBonusIncrement > 0 ? dto.StreakBonusIncrement : DefaultStreakBonusIncrement;
            EasyMultiplier = dto.EasyMultiplier > 0 ? dto.EasyMultiplier : DefaultEasyMultiplier;
            MediumMultiplier = dto.MediumMultiplier > 0 ? dto.MediumMultiplier : DefaultMediumMultiplier;
            HardMultiplier = dto.HardMultiplier > 0 ? dto.HardMultiplier : DefaultHardMultiplier;
            EnabledCategories = new HashSet<string>(dto.EnabledCategories);
            _suppressSave = false;
            AppLog.Info($"GameSettings loaded from {sourceFile}");

            if (!string.Equals(sourceFile, ConfigFile, StringComparison.OrdinalIgnoreCase))
            {
                AppLog.Info($"GameSettings migrating legacy config to {ConfigFile}");
                Save();
            }
        }
        catch (Exception ex)
        {
            AppLog.Error("GameSettings.Load failed; using defaults", ex);
        }
    }

    public void ResetToDefaults()
    {
        QuestionTime = DefaultQuestionTime;
        CountdownDuration = DefaultCountdownDuration;
        MaxComboMultiplier = DefaultMaxComboMultiplier;
        StreakBonusIncrement = DefaultStreakBonusIncrement;
        EasyMultiplier = DefaultEasyMultiplier;
        MediumMultiplier = DefaultMediumMultiplier;
        HardMultiplier = DefaultHardMultiplier;
        EnabledCategories = new HashSet<string>(AllCategories);
    }

    private static string? ResolveConfigFileToLoad()
    {
        if (File.Exists(ConfigFile))
        {
            return ConfigFile;
        }

        if (File.Exists(LegacyConfigFile))
        {
            return LegacyConfigFile;
        }

        return null;
    }

    private record SettingsDto(
        double QuestionTime, int CountdownDuration, double MaxComboMultiplier,
        double StreakBonusIncrement, double EasyMultiplier, double MediumMultiplier,
        double HardMultiplier, List<string> EnabledCategories);
}
