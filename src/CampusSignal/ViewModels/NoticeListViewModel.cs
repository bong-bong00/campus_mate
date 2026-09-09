using System.Collections.ObjectModel;
using System.Windows.Input;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>판정 결과·출처로 걸러가며 전체 공고를 훑는 화면.</summary>
public sealed class NoticeListViewModel : ScreenViewModel
{
    public NoticeListViewModel(AppServices services, IAppNavigator nav) : base(services, nav)
    {
        Filters =
        [
            new JudgmentFilterViewModel("전체", null, false, Palette.Get("TextMonoLabel")) { IsSelected = true },
            new JudgmentFilterViewModel("지원 가능", Judgment.Eligible, false, Palette.Get("EligibleDot")),
            new JudgmentFilterViewModel("확인 필요", Judgment.NeedsCheck, false, Palette.Get("CheckDot")),
            new JudgmentFilterViewModel("자격 미달", Judgment.Ineligible, false, Palette.Get("FailDot")),
            new JudgmentFilterViewModel("마감 임박", null, true, Palette.Get("FailFg")),
        ];

        foreach (var f in Filters)
            f.Command = new RelayCommand(() => SelectFilter(f));

        Sources =
        [
            .. Enum.GetValues<NoticeSource>()
                   .Select(s => new SourceFilterViewModel(s, SampleDataStore.SourceLabel(s)))
        ];

        foreach (var s in Sources)
            s.Command = new RelayCommand(() => ToggleSource(s));

        RegisterNoticeCommand = new RelayCommand(() => Nav.ShowToast(new ToastMessage(
            "공고 직접 등록",
            "URL · PDF · 캡처 이미지를 끌어다 놓으면 OCR 로 본문을 추출합니다. 수집기 연동 단계에서 활성화됩니다.",
            "확인", "닫기", null)));

        Services.Catalog.Changed += Rebuild;
        Rebuild();
    }

    public override ScreenKind Kind => ScreenKind.NoticeList;
    public override string Title => $"공고 {Rows.Count}건";

    public override string Subtitle =>
        $"내 프로필과 대조해 판정한 결과입니다 · {Store.LastCollectedText}";

    public IReadOnlyList<JudgmentFilterViewModel> Filters { get; }
    public IReadOnlyList<SourceFilterViewModel> Sources { get; }
    public ObservableCollection<NoticeRowViewModel> Rows { get; } = [];
    public ICommand RegisterNoticeCommand { get; }

    private JudgmentFilterViewModel Selected => Filters.First(f => f.IsSelected);

    private void SelectFilter(JudgmentFilterViewModel filter)
    {
        // 판정 필터는 단일 선택(라디오 성격)
        foreach (var f in Filters) f.IsSelected = f == filter;
        Rebuild();
    }

    private void ToggleSource(SourceFilterViewModel source)
    {
        // 출처 필터는 다중 선택. 전부 끄면 아무것도 안 보이므로 마지막 하나는 남긴다.
        if (source.IsSelected && Sources.Count(s => s.IsSelected) == 1) return;
        source.IsSelected = !source.IsSelected;
        Rebuild();
    }

    private void Rebuild()
    {
        foreach (var f in Filters)
        {
            f.Count = f.IsImminentFilter
                ? Catalog.ImminentCount
                : f.Judgment is { } j ? Catalog.CountOf(j) : Catalog.All.Count;
        }

        foreach (var s in Sources)
            s.Count = Catalog.SourceCount(s.Source);

        var allowed = Sources.Where(s => s.IsSelected).Select(s => s.Source).ToHashSet();
        var filter = Selected;

        var query = Catalog.All.Where(r => allowed.Contains(r.Notice.Source));
        if (filter.IsImminentFilter)
            query = query.Where(r => r.DaysLeft >= 0 && r.DaysLeft <= NoticeCatalog.ImminentDays);
        else if (filter.Judgment is { } j)
            query = query.Where(r => r.Judgment == j);

        Rows.Clear();
        foreach (var r in query) Rows.Add(ToRow(r));

        Raise(nameof(Title));
    }

    private NoticeRowViewModel ToRow(NoticeJudgment r) => new()
    {
        Id = r.Notice.Id,
        DDay = r.DDayText,
        DDayBrush = Urgency.ToneFor(r, allowStartTone: false),
        Title = r.Notice.Title,
        Note = r.Notice.ListNote,
        JudgmentText = Palette.Text(r.Judgment),
        BadgeBackground = Palette.SoftBg(r.Judgment),
        BadgeForeground = Palette.SoftFg(r.Judgment),
        SourceShort = r.Notice.SourceShort,
        RequirementRatio = r.RequirementRatio,
        Command = new RelayCommand(() => Nav.OpenNotice(r.Notice.Id)),
    };
}
