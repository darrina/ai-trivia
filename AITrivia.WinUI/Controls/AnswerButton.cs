using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace AITrivia.Controls;

/// <summary>
/// Custom answer button using UserControl to avoid Button chrome conflicts.
/// </summary>
public sealed class AnswerButton : UserControl
{
    public static readonly DependencyProperty AnswerTextProperty =
        DependencyProperty.Register(nameof(AnswerText), typeof(string), typeof(AnswerButton), new PropertyMetadata("", OnStateChanged));
    public static readonly DependencyProperty IndexProperty =
        DependencyProperty.Register(nameof(Index), typeof(int), typeof(AnswerButton), new PropertyMetadata(0, OnStateChanged));
    public static readonly DependencyProperty IsAnswerSelectedProperty =
        DependencyProperty.Register(nameof(IsAnswerSelected), typeof(bool), typeof(AnswerButton), new PropertyMetadata(false, OnStateChanged));
    public static readonly DependencyProperty IsCorrectAnswerProperty =
        DependencyProperty.Register(nameof(IsCorrectAnswer), typeof(bool), typeof(AnswerButton), new PropertyMetadata(false, OnStateChanged));
    public static readonly DependencyProperty ShowResultProperty =
        DependencyProperty.Register(nameof(ShowResult), typeof(bool), typeof(AnswerButton), new PropertyMetadata(false, OnStateChanged));

    public string AnswerText { get => (string)GetValue(AnswerTextProperty); set => SetValue(AnswerTextProperty, value); }
    public int Index { get => (int)GetValue(IndexProperty); set => SetValue(IndexProperty, value); }
    public bool IsAnswerSelected { get => (bool)GetValue(IsAnswerSelectedProperty); set => SetValue(IsAnswerSelectedProperty, value); }
    public bool IsCorrectAnswer { get => (bool)GetValue(IsCorrectAnswerProperty); set => SetValue(IsCorrectAnswerProperty, value); }
    public bool ShowResult { get => (bool)GetValue(ShowResultProperty); set => SetValue(ShowResultProperty, value); }

    public event TappedEventHandler? ButtonTapped;

    private static readonly string[] Prefixes = ["A", "B", "C", "D"];

    private Border? _outerBorder;
    private TextBlock? _prefixText;
    private Border? _prefixBorder;
    private TextBlock? _answerLabel;
    private TextBlock? _resultIcon;
    private bool _isHovered;

    public AnswerButton()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch;
        BuildContent();
        ApplyVisualState();

        base.Tapped += (_, e) => { if (!ShowResult) ButtonTapped?.Invoke(this, e); };
        PointerEntered += (_, _) => { _isHovered = true; ApplyVisualState(); };
        PointerExited += (_, _) => { _isHovered = false; ApplyVisualState(); };
    }

    private void BuildContent()
    {
        _prefixText = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 14,
            FontWeight = Microsoft.UI.Text.FontWeights.Black
        };

        _prefixBorder = new Border
        {
            Width = 28, Height = 28,
            CornerRadius = new CornerRadius(14),
            Child = _prefixText
        };

        _answerLabel = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 16,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            TextWrapping = TextWrapping.Wrap,
            Foreground = new SolidColorBrush(Colors.White)
        };

        _resultIcon = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 18,
            Visibility = Visibility.Collapsed
        };

        var grid = new Grid { Margin = new Thickness(20, 14, 20, 14) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var answerWrapper = new StackPanel { Margin = new Thickness(16, 0, 0, 0) };
        answerWrapper.Children.Add(_answerLabel);

        Grid.SetColumn(_prefixBorder, 0);
        Grid.SetColumn(answerWrapper, 1);
        Grid.SetColumn(_resultIcon, 2);
        grid.Children.Add(_prefixBorder);
        grid.Children.Add(answerWrapper);
        grid.Children.Add(_resultIcon);

        _outerBorder = new Border { CornerRadius = new CornerRadius(12), Child = grid };
        Content = _outerBorder;
    }

    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((AnswerButton)d).ApplyVisualState();

    private void ApplyVisualState()
    {
        if (_outerBorder is null) return;

        var idx = Math.Min(Index, Prefixes.Length - 1);

        _prefixText!.Text = Prefixes[idx];
        _prefixText.Foreground = new SolidColorBrush(ShowResult && IsCorrectAnswer
            ? Color.FromArgb(255, 52, 211, 153)
            : Color.FromArgb(255, 0, 191, 255));

        _prefixBorder!.Background = new SolidColorBrush(ShowResult && IsCorrectAnswer
            ? Color.FromArgb(50, 52, 211, 153)
            : Color.FromArgb(40, 0, 191, 255));

        if (_answerLabel != null) _answerLabel.Text = AnswerText;

        Color bg, border;

        if (ShowResult)
        {
            if (IsCorrectAnswer)
            {
                bg = Color.FromArgb(76, 52, 211, 153);
                border = Color.FromArgb(255, 52, 211, 153);
            }
            else if (IsAnswerSelected)
            {
                bg = Color.FromArgb(76, 239, 68, 68);
                border = Color.FromArgb(255, 239, 68, 68);
            }
            else
            {
                bg = Color.FromArgb(13, 255, 255, 255);
                border = Color.FromArgb(26, 255, 255, 255);
            }

            if (_resultIcon != null)
            {
                if (IsCorrectAnswer)
                {
                    _resultIcon.Text = "✓";
                    _resultIcon.Foreground = new SolidColorBrush(Color.FromArgb(255, 52, 211, 153));
                    _resultIcon.Visibility = Visibility.Visible;
                }
                else if (IsAnswerSelected)
                {
                    _resultIcon.Text = "✗";
                    _resultIcon.Foreground = new SolidColorBrush(Color.FromArgb(255, 239, 68, 68));
                    _resultIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    _resultIcon.Visibility = Visibility.Collapsed;
                }
            }
        }
        else
        {
            bg = _isHovered ? Color.FromArgb(38, 255, 255, 255) : Color.FromArgb(20, 255, 255, 255);
            border = _isHovered ? Color.FromArgb(128, 0, 191, 255) : Color.FromArgb(38, 255, 255, 255);
            if (_resultIcon != null) _resultIcon.Visibility = Visibility.Collapsed;
        }

        _outerBorder.Background = new SolidColorBrush(bg);
        _outerBorder.BorderBrush = new SolidColorBrush(border);
        _outerBorder.BorderThickness = new Thickness(ShowResult && (IsCorrectAnswer || IsAnswerSelected) ? 2 : 1);

        var scale = _isHovered && !ShowResult ? 1.02 : 1.0;
        RenderTransform = new Microsoft.UI.Xaml.Media.ScaleTransform { ScaleX = scale, ScaleY = scale };
        RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
    }
}
