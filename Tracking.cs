using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CampusSignal.Infrastructure;

/// <summary>
/// WPF 의 TextBlock 에는 CSS 의 letter-spacing 에 해당하는 속성이 없다.
/// 디자인은 Mono 라벨에 +0.08 ~ +0.18em 트래킹을 요구하므로, 글자 사이에
/// 폭이 정확히 지정된 빈 <see cref="InlineUIContainer"/> 를 끼워 넣어 재현한다.
///
///   &lt;TextBlock FontSize="10.5" ui:Tracking.Em="0.14" ui:Tracking.Text="EXAM CONFLICT"/&gt;
///
/// 트래킹을 적용할 텍스트는 <see cref="TextProperty"/> 로 넣는다.
/// (TextBlock.Text 와 Inlines 는 동시에 못 쓰기 때문이다.)
/// </summary>
public static class Tracking
{
    public static readonly DependencyProperty EmProperty = DependencyProperty.RegisterAttached(
        "Em", typeof(double), typeof(Tracking),
        new PropertyMetadata(0d, OnChanged));

    public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
        "Text", typeof(string), typeof(Tracking),
        new PropertyMetadata(null, OnChanged));

    public static void SetEm(DependencyObject o, double v) => o.SetValue(EmProperty, v);
    public static double GetEm(DependencyObject o) => (double)o.GetValue(EmProperty);

    public static void SetText(DependencyObject o, string? v) => o.SetValue(TextProperty, v);
    public static string? GetText(DependencyObject o) => (string?)o.GetValue(TextProperty);

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock tb) return;

        // FontSize 가 스타일에서 뒤늦게 흘러들어오는 경우가 있어 Loaded 에서 한 번 더 그린다.
        tb.Loaded -= OnLoaded;
        tb.Loaded += OnLoaded;
        Render(tb);
    }

    private static void OnLoaded(object sender, RoutedEventArgs e) => Render((TextBlock)sender);

    private static void Render(TextBlock tb)
    {
        var text = GetText(tb);
        if (text is null) return;

        var em = GetEm(tb);
        tb.Inlines.Clear();

        if (em <= 0)
        {
            tb.Inlines.Add(new Run(text));
            return;
        }

        var gap = tb.FontSize * em;
        for (var i = 0; i < text.Length; i++)
        {
            tb.Inlines.Add(new Run(text[i].ToString()));
            if (i == text.Length - 1) continue;
            tb.Inlines.Add(new InlineUIContainer(new Border { Width = gap, Height = 0 })
            {
                BaselineAlignment = BaselineAlignment.Baseline
            });
        }
    }
}
