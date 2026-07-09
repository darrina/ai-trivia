using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace AITrivia.Controls;

public sealed partial class SettingsControl : UserControl
{
    public event EventHandler? CloseRequested;

    private bool _loading;

    public SettingsControl()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loading = true;
        var s = App.GameSettings;

        QuestionTimeSlider.Value = s.QuestionTime;
        MaxComboSlider.Value = s.MaxComboMultiplier;
        StreakBonusSlider.Value = s.StreakBonusIncrement;

        // Initialize labels (ValueChanged is suppressed during _loading)
        QuestionTimeLabel.Text = $"{(int)s.QuestionTime} sec";
        MaxComboLabel.Text = $"{s.MaxComboMultiplier:F1}x";
        StreakBonusLabel.Text = $"{s.StreakBonusIncrement:F2}x per streak";

        UpdateCountdownLabel(s.CountdownDuration);
        UpdateDifficultyLabels();
        BuildCategoryToggles();

        _loading = false;
    }

    // ── Slider handlers ───────────────────────────────────────────────────────
    private void QuestionTimeSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (_loading) return;
        var v = (int)QuestionTimeSlider.Value;
        QuestionTimeLabel.Text = $"{v} sec";
        App.GameSettings.QuestionTime = v;
    }

    private void MaxComboSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (_loading) return;
        var v = MaxComboSlider.Value;
        MaxComboLabel.Text = $"{v:F1}x";
        App.GameSettings.MaxComboMultiplier = v;
    }

    private void StreakBonusSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (_loading) return;
        var v = StreakBonusSlider.Value;
        StreakBonusLabel.Text = $"{v:F2}x per streak";
        App.GameSettings.StreakBonusIncrement = v;
    }

    // ── Countdown stepper ─────────────────────────────────────────────────────
    private void CountdownMinus_Click(object sender, RoutedEventArgs e)
    {
        if (App.GameSettings.CountdownDuration <= 1) return;
        App.GameSettings.CountdownDuration--;
        UpdateCountdownLabel(App.GameSettings.CountdownDuration);
    }

    private void CountdownPlus_Click(object sender, RoutedEventArgs e)
    {
        if (App.GameSettings.CountdownDuration >= 5) return;
        App.GameSettings.CountdownDuration++;
        UpdateCountdownLabel(App.GameSettings.CountdownDuration);
    }

    private void UpdateCountdownLabel(int v)
    {
        if (CountdownLabel != null) CountdownLabel.Text = $"{v} sec";
    }

    // ── Difficulty steppers ────────────────────────────────────────────────────
    private void EasyMinus_Click(object sender, RoutedEventArgs e)
    {
        if (App.GameSettings.EasyMultiplier <= 0.5) return;
        App.GameSettings.EasyMultiplier -= 0.5;
        UpdateDifficultyLabels();
    }
    private void EasyPlus_Click(object sender, RoutedEventArgs e)
    {
        if (App.GameSettings.EasyMultiplier >= 5.0) return;
        App.GameSettings.EasyMultiplier += 0.5;
        UpdateDifficultyLabels();
    }
    private void MediumMinus_Click(object sender, RoutedEventArgs e)
    {
        if (App.GameSettings.MediumMultiplier <= 0.5) return;
        App.GameSettings.MediumMultiplier -= 0.5;
        UpdateDifficultyLabels();
    }
    private void MediumPlus_Click(object sender, RoutedEventArgs e)
    {
        if (App.GameSettings.MediumMultiplier >= 5.0) return;
        App.GameSettings.MediumMultiplier += 0.5;
        UpdateDifficultyLabels();
    }
    private void HardMinus_Click(object sender, RoutedEventArgs e)
    {
        if (App.GameSettings.HardMultiplier <= 0.5) return;
        App.GameSettings.HardMultiplier -= 0.5;
        UpdateDifficultyLabels();
    }
    private void HardPlus_Click(object sender, RoutedEventArgs e)
    {
        if (App.GameSettings.HardMultiplier >= 5.0) return;
        App.GameSettings.HardMultiplier += 0.5;
        UpdateDifficultyLabels();
    }

    private void UpdateDifficultyLabels()
    {
        var s = App.GameSettings;
        if (EasyMultiplierLabel != null)   EasyMultiplierLabel.Text   = $"{s.EasyMultiplier:F1}x";
        if (MediumMultiplierLabel != null) MediumMultiplierLabel.Text = $"{s.MediumMultiplier:F1}x";
        if (HardMultiplierLabel != null)   HardMultiplierLabel.Text   = $"{s.HardMultiplier:F1}x";
    }

    // ── Categories ────────────────────────────────────────────────────────────
    private void BuildCategoryToggles()
    {
        CategoriesPanel.Children.Clear();
        foreach (var cat in GameSettings.AllCategories)
        {
            var row = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var enabled = App.GameSettings.EnabledCategories.Contains(cat);

            var icon = new TextBlock
            {
                Text = enabled ? "✓" : "○",
                FontSize = 16,
                Foreground = new SolidColorBrush(enabled
                    ? Color.FromArgb(255, 0, 191, 255)
                    : Color.FromArgb(77, 255, 255, 255)),
                Margin = new Thickness(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var label = new TextBlock
            {
                Text = cat,
                FontSize = 13,
                FontWeight = Microsoft.UI.Text.FontWeights.Medium,
                Foreground = new SolidColorBrush(enabled
                    ? Color.FromArgb(230, 255, 255, 255)
                    : Color.FromArgb(102, 255, 255, 255)),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetColumn(icon, 0);
            Grid.SetColumn(label, 1);
            row.Children.Add(icon);
            row.Children.Add(label);

            string category = cat;
            row.Tapped += (_, _) => ToggleCategory(category);
            row.IsHitTestVisible = true;

            CategoriesPanel.Children.Add(row);
        }
    }

    private void ToggleCategory(string category)
    {
        var cats = new HashSet<string>(App.GameSettings.EnabledCategories);
        if (cats.Contains(category))
        {
            if (cats.Count <= 1) return; // keep at least one
            cats.Remove(category);
        }
        else
        {
            cats.Add(category);
        }
        App.GameSettings.EnabledCategories = cats;
        BuildCategoryToggles();
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        App.GameSettings.EnabledCategories = new HashSet<string>(GameSettings.AllCategories);
        BuildCategoryToggles();
    }

    private void DeselectAll_Click(object sender, RoutedEventArgs e)
    {
        App.GameSettings.EnabledCategories = new HashSet<string> { GameSettings.AllCategories[0] };
        BuildCategoryToggles();
    }

    private void ResetDefaults_Click(object sender, RoutedEventArgs e)
    {
        App.GameSettings.ResetToDefaults();
        OnLoaded(this, new RoutedEventArgs());
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
        => CloseRequested?.Invoke(this, EventArgs.Empty);
}
