using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AITrivia.Controls;

public sealed partial class AboutControl : UserControl
{
    public event EventHandler? CloseRequested;

    public AboutControl()
    {
        InitializeComponent();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
        => CloseRequested?.Invoke(this, EventArgs.Empty);
}
