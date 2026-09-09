using System.Collections.ObjectModel;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>공고로 뜨지 않는 교내 혜택까지 포함하되, 시간표와 겹치는 것은 뺀 목록.</summary>
public sealed class BenefitsViewModel : ScreenViewModel
{
    private const string AllCategory = "전체";

    public BenefitsViewModel(AppServices services, IAppNavigator nav) : base(services, nav)
    {
        var categories = new[] { AllCategory }
            .Concat(Store.Benefits.Select(b => b.Category).Distinct())
            .ToList();

        Tabs = categories
            .Select(c => new BenefitTabViewModel(c, $"{c} {CountOf(c)}"))
            .ToList();

        foreach (var t in Tabs)
            t.Command = new RelayCommand(() => Select(t));

        Tabs[0].IsActive = true;
        Rebuild();
    }

    public override ScreenKind Kind => ScreenKind.Benefits;
    public override string Title => $"혜택 · 프로그램 {Store.Benefits.Count}건";

    public override string Subtitle =>
        $"시간표와 겹치는 {Store.ExcludedPrograms.Count}건은 자동으로 제외했습니다";

    public IReadOnlyList<BenefitTabViewModel> Tabs { get; }
    public ObservableCollection<BenefitCardViewModel> Cards { get; } = [];

    public IReadOnlyList<ExcludedProgram> Excluded => Store.ExcludedPrograms;

    public string ExcludedTitle => $"시간표 충돌로 제외된 항목 {Store.ExcludedPrograms.Count}건";

    private int CountOf(string category) => category == AllCategory
        ? Store.Benefits.Count
        : Store.Benefits.Count(b => b.Category == category);

    private void Select(BenefitTabViewModel tab)
    {
        foreach (var t in Tabs) t.IsActive = t == tab;
        Rebuild();
    }

    private void Rebuild()
    {
        var active = Tabs.First(t => t.IsActive).Category;

        Cards.Clear();
        foreach (var b in Store.Benefits.Where(b => active == AllCategory || b.Category == active))
        {
            var top = b.State == BenefitState.TopPick;
            Cards.Add(new BenefitCardViewModel
            {
                Department = b.Department,
                Title = b.Title,
                Description = b.Description,
                TimeText = b.TimeText,
                CreditText = b.CreditText,
                StateText = b.StateText,
                StateBackground = Palette.BenefitBg(b.State),
                StateForeground = Palette.BenefitFg(b.State),
                // 우선 추천 카드만 연녹 배경 + 강조 테두리
                CardBackground = Palette.Get(top ? "AccentBgSoft" : "SurfaceRaised"),
                CardBorder = Palette.Get(top ? "BorderAccent" : "Border"),
            });
        }
    }
}
