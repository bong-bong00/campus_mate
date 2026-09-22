using System.Windows;
using System.Windows.Media;
using CampusSignal.Models;

namespace CampusSignal.Infrastructure;

/// <summary>Themes/Palette.xaml 의 브러시를 코드에서 꺼내 쓰기 위한 얇은 래퍼.</summary>
public static class Palette
{
    public static Brush Get(string key)
        => Application.Current?.TryFindResource(key) as Brush ?? Brushes.Transparent;

    // ── 판정별 색 묶음 ────────────────────────────────────────────────────
    /// <summary>목록·배지에 쓰는 연한 배경.</summary>
    public static Brush SoftBg(Judgment j) => Get(j switch
    {
        Judgment.Eligible => "EligibleBg",
        Judgment.NeedsCheck => "CheckBg",
        _ => "FailBg",
    });

    public static Brush SoftFg(Judgment j) => Get(j switch
    {
        Judgment.Eligible => "EligibleFg",
        Judgment.NeedsCheck => "CheckFg",
        _ => "FailFg",
    });

    /// <summary>히어로·상세 배너의 채움형 배지.</summary>
    public static Brush SolidBg(Judgment j) => Get(j switch
    {
        Judgment.Eligible => "EligibleSolidBg",
        Judgment.NeedsCheck => "CheckSolidBg",
        _ => "FailSolidBg",
    });

    public static Brush SolidFg(Judgment j) => Get(j switch
    {
        Judgment.Eligible => "EligibleSolidFg",
        Judgment.NeedsCheck => "CheckSolidFg",
        _ => "FailSolidFg",
    });

    public static Brush HeroBg(Judgment j) => Get(j switch
    {
        Judgment.Eligible => "EligibleHeroBg",
        Judgment.NeedsCheck => "CheckHeroBg",
        _ => "FailHeroBg",
    });

    public static Brush HeroBorder(Judgment j) => Get(j switch
    {
        Judgment.Eligible => "EligibleHeroBd",
        Judgment.NeedsCheck => "CheckHeroBd",
        _ => "FailHeroBd",
    });

    public static Brush Dot(Judgment j) => Get(j switch
    {
        Judgment.Eligible => "EligibleDot",
        Judgment.NeedsCheck => "CheckDot",
        _ => "FailDot",
    });

    public static string Text(Judgment j) => j switch
    {
        Judgment.Eligible => "지원 가능",
        Judgment.NeedsCheck => "확인 필요",
        _ => "자격 미달",
    };

    // ── 요건 마크 색 ──────────────────────────────────────────────────────
    public static Brush RequirementMark(RequirementResult r) => Get(r switch
    {
        RequirementResult.Met => "EligibleFg",
        RequirementResult.Unknown => "CheckFg",
        _ => "FailFg",
    });

    // ── 서류 상태 배지 ────────────────────────────────────────────────────
    public static Brush DocumentBg(DocumentState s) => Get(s switch
    {
        DocumentState.Held => "EligibleBg",
        DocumentState.ActionNeeded => "CheckBg",
        _ => "NeutralBg",
    });

    public static Brush DocumentFg(DocumentState s) => Get(s switch
    {
        DocumentState.Held => "EligibleFg",
        DocumentState.ActionNeeded => "CheckFg",
        _ => "NeutralFg",
    });

    // ── 알림 ──────────────────────────────────────────────────────────────
    public static Brush AlertDot(AlertKind k) => Get(k switch
    {
        AlertKind.New => "EligibleDot",
        AlertKind.Changed => "CheckDot",
        _ => "FailFg",
    });

    public static Brush AlertBg(AlertKind k) => Get(k switch
    {
        AlertKind.New => "EligibleBg",
        AlertKind.Start or AlertKind.Changed => "CheckBg",
        _ => "FailBg",
    });

    public static Brush AlertFg(AlertKind k) => Get(k switch
    {
        AlertKind.New => "EligibleFg",
        AlertKind.Start or AlertKind.Changed => "CheckFg",
        _ => "FailFg",
    });

    // ── 캘린더 이벤트 칩 ──────────────────────────────────────────────────
    public static Brush EventBg(CalendarEventKind k) => Get(k switch
    {
        CalendarEventKind.Deadline => "EvtDeadlineBg",
        CalendarEventKind.Exam => "EvtExamBg",
        CalendarEventKind.Academic => "EvtAcademicBg",
        _ => "EvtPersonalBg",
    });

    public static Brush EventFg(CalendarEventKind k) => Get(k switch
    {
        CalendarEventKind.Deadline => "EvtDeadlineFg",
        CalendarEventKind.Exam => "EvtExamFg",
        CalendarEventKind.Academic => "EvtAcademicFg",
        _ => "EvtPersonalFg",
    });

    // ── 상세 우측 경고 카드 ───────────────────────────────────────────────
    public static Brush SideBg(SideNoteKind k) => Get(k switch
    {
        SideNoteKind.MissingInput => "CheckHeroBg",
        SideNoteKind.NotEligible => "FailHeroBg",
        _ => "WarnSurface",
    });

    public static Brush SideBorder(SideNoteKind k) => Get(k switch
    {
        SideNoteKind.MissingInput => "BorderCaution",
        _ => "BorderWarn",
    });

    public static Brush SideTone(SideNoteKind k) => Get(k switch
    {
        SideNoteKind.MissingInput => "CheckFg",
        _ => "FailFg",
    });

    // ── 혜택 카드 상태 배지 ───────────────────────────────────────────────
    public static Brush BenefitBg(BenefitState s) => Get(s switch
    {
        BenefitState.TopPick => "EligibleSolidBg",
        BenefitState.Open => "EligibleBg",
        BenefitState.NeedsCheck => "CheckBg",
        _ => "NeutralBg",
    });

    public static Brush BenefitFg(BenefitState s) => Get(s switch
    {
        BenefitState.TopPick => "EligibleSolidFg",
        BenefitState.Open => "EligibleFg",
        BenefitState.NeedsCheck => "CheckFg",
        _ => "NeutralFg",
    });

    // ── 시간표 수업 블록 6색 순환 ─────────────────────────────────────────
    private static readonly string[] ClassFills = ["#EAF2E2", "#E9EFF3", "#F4EFE3", "#EFEAF2", "#E7F1EF", "#F2ECE8"];
    private static readonly string[] ClassBars = ["#7FA860", "#7C9DB5", "#C0A662", "#A18FB5", "#6FA79B", "#BE9782"];

    public static Brush ClassFill(int i) => Frozen(ClassFills[((i % 6) + 6) % 6]);
    public static Brush ClassBar(int i) => Frozen(ClassBars[((i % 6) + 6) % 6]);

    private static readonly Dictionary<string, Brush> Cache = [];

    private static Brush Frozen(string hex)
    {
        if (Cache.TryGetValue(hex, out var b)) return b;
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex)!);
        brush.Freeze();
        Cache[hex] = brush;
        return brush;
    }
}
