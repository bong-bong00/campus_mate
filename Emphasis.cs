using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CampusSignal.Infrastructure;

/// <summary>
/// 다크 카드의 강조 문구(라임색)나 본문 속 굵은 단어를 데이터에서 표현하기 위한 마크업.
/// 문자열 안의 <c>&lt;em&gt;…&lt;/em&gt;</c> 구간만 <see cref="BrushProperty"/> 색과
/// <see cref="WeightProperty"/> 굵기로 그린다.
/// </summary>
public static partial class Emphasis
{
    public static readonly DependencyProperty MarkupProperty = DependencyProperty.RegisterAttached(
        "Markup", typeof(string), typeof(Emphasis), new PropertyMetadata(null, OnChanged));

    public static readonly DependencyProperty BrushProperty = DependencyProperty.RegisterAttached(
        "Brush", typeof(Brush), typeof(Emphasis), new PropertyMetadata(null, OnChanged));

    public static readonly DependencyProperty WeightProperty = DependencyProperty.RegisterAttached(
        "Weight", typeof(FontWeight), typeof(Emphasis),
        new PropertyMetadata(FontWeights.Normal, OnChanged));

    public static void SetMarkup(DependencyObject o, string? v) => o.SetValue(MarkupProperty, v);
    public static string? GetMarkup(DependencyObject o) => (string?)o.GetValue(MarkupProperty);

    public static void SetBrush(DependencyObject o, Brush? v) => o.SetValue(BrushProperty, v);
    public static Brush? GetBrush(DependencyObject o) => (Brush?)o.GetValue(BrushProperty);

    public static void SetWeight(DependencyObject o, FontWeight v) => o.SetValue(WeightProperty, v);
    public static FontWeight GetWeight(DependencyObject o) => (FontWeight)o.GetValue(WeightProperty);

    [GeneratedRegex(@"<em>(.*?)</em>", RegexOptions.Singleline)]
    private static partial Regex EmTag();

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBlock tb) return;

        var markup = GetMarkup(tb);
        tb.Inlines.Clear();
        if (string.IsNullOrEmpty(markup)) return;

        var brush = GetBrush(tb);
        var weight = GetWeight(tb);
        var cursor = 0;

        foreach (Match m in EmTag().Matches(markup))
        {
            if (m.Index > cursor)
                tb.Inlines.Add(new Run(markup[cursor..m.Index]));

            var run = new Run(m.Groups[1].Value) { FontWeight = weight };
            if (brush is not null) run.Foreground = brush;
            tb.Inlines.Add(run);

            cursor = m.Index + m.Length;
        }

        if (cursor < markup.Length)
            tb.Inlines.Add(new Run(markup[cursor..]));
    }
}
