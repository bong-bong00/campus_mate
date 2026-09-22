using System.Collections.ObjectModel;
using System.Windows.Input;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>앱을 켠 직후 "오늘 뭘 해야 하는지"를 한 화면에 담는다.</summary>
public sealed class DashboardViewModel : ScreenViewModel
{
    public DashboardViewModel(AppServices services, IAppNavigator nav) : base(services, nav)
    {
        GoCalendarCommand = new RelayCommand(() => Nav.Go(ScreenKind.Calendar));
        GoProfileCommand = new RelayCommand(() => Nav.Go(ScreenKind.Profile));
        HeroCommand = new RelayCommand(() => { if (Hero is not null) Nav.OpenNotice(Hero.Notice.Id); });
        HeroSecondaryCommand = new RelayCommand(() => { if (Hero is not null) Nav.OpenNotice(Hero.Notice.Id); });
        Services.Catalog.Changed += Rebuild;
        Rebuild();
    }

    public override ScreenKind Kind => ScreenKind.Dashboard;
    public override string Title => "지금 챙기지 않으면 놓칩니다";

    public override string Subtitle =>
        $"{Services.Clock.Today:M월 d일} · 지원 가능 {Catalog.CountOf(Judgment.Eligible)} · " +
        $"확인 필요 {Catalog.CountOf(Judgment.NeedsCheck)} · 이번 주 마감 {Catalog.DueWithin(7)}";

    public ICommand GoCalendarCommand { get; }
    public ICommand GoProfileCommand { get; }
    public ICommand HeroCommand { get; }
    public ICommand HeroSecondaryCommand { get; }

    // ── 히어로 ────────────────────────────────────────────────────────────
    public NoticeJudgment? Hero { get; private set; }

    public bool HasHero => Hero is not null;
    public string HeroDDay => Hero?.DDayText ?? "";
    public string HeroDeadline => Hero is null ? "" : $"{Hero.Notice.Deadline:MM.dd HH:mm}";
    public string HeroJudgmentText => Hero is null ? "" : Palette.Text(Hero.Judgment);
    public System.Windows.Media.Brush HeroBadgeBackground => Palette.SolidBg(Hero?.Judgment ?? Judgment.Eligible);
    public System.Windows.Media.Brush HeroBadgeForeground => Palette.SolidFg(Hero?.Judgment ?? Judgment.Eligible);
    public System.Windows.Media.Brush HeroBackground => Palette.HeroBg(Hero?.Judgment ?? Judgment.Eligible);
    public System.Windows.Media.Brush HeroBorder => Palette.HeroBorder(Hero?.Judgment ?? Judgment.Eligible);
    public string HeroSource => Hero?.Notice.SourceText ?? "";
    public string HeroTitle => Hero?.Notice.Title ?? "";
    public string HeroDescription { get; private set; } = "";
    public string HeroPrimaryText => Hero?.Notice.CtaText ?? "";
    public string HeroSecondaryText => "공고 원문 보기";

    // ── 목록 ──────────────────────────────────────────────────────────────
    public ObservableCollection<TodoRowViewModel> Todos { get; } = [];
    public ObservableCollection<ProgramCardViewModel> Programs { get; } = [];
    public ObservableCollection<WeekRowViewModel> Week { get; } = [];

    public string TodoSubtitle => "긴급도 순 · 서류 발급 시간 반영";
    public string ProgramSubtitle => $"충돌 {Store.ExcludedPrograms.Count}건 자동 제외됨";

    // ── 우측 카드 ─────────────────────────────────────────────────────────
    public string ConflictTitle { get; private set; } = "";
    public string ConflictBody { get; private set; } = "";
    public bool HasConflict => !string.IsNullOrEmpty(ConflictTitle);

    public string BlockedTitle { get; private set; } = "";
    public string BlockedBody { get; private set; } = "";
    public string BlockedLink => "성적표 업로드로 자동 입력 →";
    public bool HasBlocked => Catalog.BlockedByTopField > 0;

    private void Rebuild()
    {
        Hero = Catalog.Hero;
        HeroDescription = Hero is null ? "" : DescribeHero(Hero);

        Todos.Clear();
        foreach (var r in Catalog.Todos(4)) Todos.Add(ToTodo(r));

        Programs.Clear();
        foreach (var b in Store.Benefits.Where(b => b.State is BenefitState.TopPick or BenefitState.Open).Take(3))
        {
            Programs.Add(new ProgramCardViewModel(
                b.Department, b.Title,
                b.CreditText == "—" ? b.TimeText : $"{b.CreditText} · {b.TimeText}",
                new RelayCommand(() => Nav.Go(ScreenKind.Benefits))));
        }

        Week.Clear();
        foreach (var w in UpcomingWeekRows(5)) Week.Add(w);

        BuildConflictCard();
        BuildBlockedCard();

        foreach (var p in new[]
                 {
                     nameof(Hero), nameof(HasHero), nameof(HeroDDay), nameof(HeroDeadline),
                     nameof(HeroJudgmentText), nameof(HeroBadgeBackground), nameof(HeroBadgeForeground),
                     nameof(HeroBackground), nameof(HeroBorder), nameof(HeroSource), nameof(HeroTitle),
                     nameof(HeroDescription), nameof(HeroPrimaryText), nameof(Subtitle),
                     nameof(ConflictTitle), nameof(ConflictBody), nameof(HasConflict),
                     nameof(BlockedTitle), nameof(BlockedBody), nameof(HasBlocked),
                 })
        {
            Raise(p);
        }
    }

    private static string DescribeHero(NoticeJudgment r)
    {
        var parts = new List<string> { $"요건 {r.RequirementRatio} 충족" };

        var docs = r.Notice.Documents.Where(d => d.State != DocumentState.NotApplicable).ToList();
        if (docs.Count > 0)
        {
            var pending = docs.Count(d => d.State == DocumentState.ActionNeeded);
            parts.Add(pending == 0
                ? $"필요 서류 {docs.Count}종 모두 보유"
                : $"필요 서류 {docs.Count}종 중 {pending}종 준비 필요");
        }

        var conflicts = r.Checks.Count(c =>
            c.Result == RequirementResult.Flagged && c.Requirement.FlagConflicts);
        if (conflicts > 0) parts.Add($"중복 수혜 충돌 {conflicts}건");

        return string.Join(" · ", parts);
    }

    private TodoRowViewModel ToTodo(NoticeJudgment r) => new()
    {
        Urgency = r.UrgencyText,
        UrgencyBrush = Urgency.ToneFor(r, allowStartTone: true),
        Title = r.Notice.Title,
        JudgmentText = Palette.Text(r.Judgment),
        BadgeBackground = Palette.SoftBg(r.Judgment),
        BadgeForeground = Palette.SoftFg(r.Judgment),
        Note = TodoNote(r),
        ActionText = TodoAction(r),
        Command = new RelayCommand(() =>
        {
            // 확인 필요는 공고가 아니라 비어 있는 프로필 항목으로 데려간다.
            if (r.Judgment == Judgment.NeedsCheck) Nav.Go(ScreenKind.Profile);
            else Nav.OpenNotice(r.Notice.Id);
        }),
    };

    private static string TodoNote(NoticeJudgment r) => r.Judgment switch
    {
        Judgment.NeedsCheck when r.BlockingFields.Count > 0 =>
            $"{string.Join(" · ", r.BlockingFields)} 값이 확인되지 않았습니다",
        Judgment.Ineligible =>
            r.Checks.FirstOrDefault(c => c.Result == RequirementResult.Failed) is { } f
                ? $"{f.Label} — {f.MyValueText}"
                : r.Notice.ListNote,
        _ => r.LongestDocument is { } d
            ? $"{d.Name} 발급 {d.LeadText} · 마감 {r.Notice.Deadline:MM.dd}"
            : r.Notice.ListNote,
    };

    private static string TodoAction(NoticeJudgment r) => r.Judgment switch
    {
        Judgment.NeedsCheck => "성적표 올리기",
        Judgment.Ineligible => "사유 보기",
        _ => r.LongestDocument is null ? "신청서 작성" : "서류 준비",
    };

    private IEnumerable<WeekRowViewModel> UpcomingWeekRows(int take)
    {
        var today = Services.Clock.Today;

        var events = Catalog.DeadlineEvents()
            .Concat(Catalog.StartEvents())
            .Concat(Store.AcademicEvents)
            .Append(new CalendarEvent("중간고사 시작", CalendarEventKind.Exam, Store.ExamPeriod.Start))
            .Where(e => e.Date >= today)
            .OrderBy(e => e.Date)
            .Take(take);

        foreach (var e in events)
        {
            var tone = e.Kind switch
            {
                CalendarEventKind.Deadline => Palette.Get("FailFg"),
                CalendarEventKind.Exam => Palette.Get("CheckDot"),
                CalendarEventKind.Academic when e.Label.StartsWith("서류 착수") => Palette.Get("StartTone"),
                _ => Palette.Get("EligibleDot"),
            };
            yield return new WeekRowViewModel(EnglishDay(e.Date), e.Label, tone);
        }
    }

    private static string EnglishDay(DateTime d) => d.DayOfWeek switch
    {
        DayOfWeek.Monday => "MON",
        DayOfWeek.Tuesday => "TUE",
        DayOfWeek.Wednesday => "WED",
        DayOfWeek.Thursday => "THU",
        DayOfWeek.Friday => "FRI",
        DayOfWeek.Saturday => "SAT",
        _ => "SUN",
    };

    private void BuildConflictCard()
    {
        var (examStart, examEnd) = Store.ExamPeriod;

        var clashing = Catalog.All
            .Where(r => r.Judgment != Judgment.Ineligible &&
                        r.Notice.Deadline.Date >= examStart && r.Notice.Deadline.Date <= examEnd)
            .ToList();

        if (clashing.Count == 0)
        {
            ConflictTitle = "";
            ConflictBody = "";
            return;
        }

        var earliest = clashing.Min(r => r.StartDate);
        ConflictTitle = $"중간고사 {examStart:MM.dd}–{examEnd:MM.dd}와 마감 {clashing.Count}건이 겹칩니다";
        ConflictBody = "서류 발급 소요를 더하면 실질 착수 기한은 " +
                       $"<em>{earliest:MM.dd}({BusinessDays.KoreanDay(earliest)})</em>입니다.";
    }

    private void BuildBlockedCard()
    {
        var count = Catalog.BlockedByTopField;
        var field = Catalog.TopBlockingField;

        BlockedTitle = $"확인 필요 {Catalog.CountOf(Judgment.NeedsCheck)}건";
        BlockedBody = field is null
            ? "보류된 항목이 없습니다."
            : $"프로필의 <em>{field}</em>이(가) 비어 있어 {count}건의 판정이 보류된 상태입니다.";
    }
}
