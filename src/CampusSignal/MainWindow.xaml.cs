using System.Windows;

namespace CampusSignal;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        StateChanged += (_, _) => UpdateMaximizeGlyph();
    }

    private void OnMinimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void OnMaximizeRestore(object sender, RoutedEventArgs e) =>
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

    private void OnClose(object sender, RoutedEventArgs e) => Close();

    private void UpdateMaximizeGlyph()
    {
        var key = WindowState == WindowState.Maximized ? "GlyphRestore" : "GlyphMaximize";
        MaximizeButton.Content = TryFindResource(key);
        MaximizeButton.ToolTip = WindowState == WindowState.Maximized ? "이전 크기로" : "최대화";

        // WindowChrome 로 최대화하면 작업 표시줄을 덮으므로 여백을 준다.
        BorderThickness = WindowState == WindowState.Maximized ? new Thickness(7) : new Thickness(0);
    }
}
