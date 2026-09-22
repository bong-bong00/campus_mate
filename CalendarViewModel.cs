using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>마감·시험·학사일정을 한 격자에서 겹쳐 보는 화면.</summary>
public sealed class CalendarViewModel : ScreenViewModel
{
    public CalendarViewModel(AppServices services, IAppNavigator nav) : base(services, nav)
    {
        Month = new DateTime(services.Clock.Today.Year, services.Clock.Today.Month, 1);
        Services.Catalog.Changed += Build;
        Build();
    }

    public override ScreenKind Kind => ScreenKind.Calendar;
    public override string Title => $"{Month:yyyy년 M월}";
    public override string Subtitle => "공고 마감 · 학사일정 · 시험 기간을 한 캘린더에";

    public DateTime Month { get; }

    public IReadOnlyList<string> DayHeaders { get; } =
        ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];

    public List<CalendarCellViewModel> Cells { get; } = [];

    public IReadOnlyList<LegendItemViewModel> Legend { get; } =
    [
        new("공고 마감", Palette.EventBg(CalendarEventKind.Deadline)),
        new("시험 기간", Palette.EventBg(CalendarEventKind.Exam)),
        new("학사 일정", Palette.EventBg(CalendarEventKind.Academic)),
        new("개인 일정", Palette.EventBg(CalendarEventKind.Personal)),
    ];

    public string OverlapTitle { get; private set; } = "";
    public string OverlapBody { get; private set; } = "";
    public bool HasOverlap => !string.IsNullOrEmpty(OverlapTitle);

    private void Build()
    {
        var today = Services.Clock.Today;
        var (examStart, examEnd) = Store.ExamPeriod;

        // 이벤트를 날짜별로 모은다. 시험 기간은 매일 칩이 하나씩 붙는다.
        var events = Catalog.DeadlineEvents()
            .Concat(Catalog.StartEvents())
            .Concat(Store.AcademicEvents)
            .Concat(ExamChips(examStart, examEnd))
            .Where(e => e.Date.Year == Month.Year && e.Date.Month == Month.Month)
            .GroupBy(e => e.Date.Day)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 첫 칸 오프셋은 그 달 1일의 요일로 계산한다.
        var offset = (int)Month.DayOfWeek;
        var daysInMonth = DateTime.DaysInMonth(Month.Year, Month.Month);
        var cellCount = (int)Math.Ceiling((offset + daysInMonth) / 7.0) * 7;

        Cells.Clear();
        for (var i = 0; i < cellCount; i++)
        {
            var day = i - offset + 1;
            var inMonth = day >= 1 && day <= daysInMonth;
            var date = inMonth ? new DateTime(Month.Year, Month.Month, day) : default;
            var isToday = inMonth && date == today;
            var inExams = inMonth && date >= examStart && date <= examEnd;

            Cells.Add(new CalendarCellViewModel
            {
                Number = inMonth ? day.ToString() : "",
                NumberBrush = Palette.Get(
                    isToday ? "EligibleFg" : inMonth ? "TextBody" : "TextDisabledNum"),
                Background = Palette.Get(
                    isToday ? "AccentBg" : inExams ? "ExamWeekCell" : "Surface"),
                Events = inMonth && events.TryGetValue(day, out var list)
                    ? list.Select(e => new EventChipViewModel(
                        e.Label, Palette.EventBg(e.Kind), Palette.EventFg(e.Kind))).ToList()
                    : [],
            });
        }

        BuildOverlapCard(examStart, examEnd);
        Raise(nameof(Cells));
        Raise(nameof(OverlapTitle));
        Raise(nameof(OverlapBody));
        Raise(nameof(HasOverlap));
    }

    private static IEnumerable<CalendarEvent> ExamChips(DateTime start, DateTime end)
    {
        for (var d = start; d <= end; d = d.AddDays(1))
            yield return new CalendarEvent("중간고사", CalendarEventKind.Exam, d);
    }

    private void BuildOverlapCard(DateTime examStart, DateTime examEnd)
    {
        var clashing = Catalog.All
            .Where(r => r.Judgment != Judgment.Ineligible &&
                        r.Notice.Deadline.Date >= examStart && r.Notice.Deadline.Date <= examEnd)
            .OrderBy(r => r.Notice.Deadline)
            .ToList();

        if (clashing.Count == 0)
        {
            OverlapTitle = "";
            OverlapBody = "";
            return;
        }

        OverlapTitle = $"시험 주간에 마감 {clashing.Count}건";
        OverlapBody = string.Join(", ",
            clashing.Select(r => $"{r.Notice.Deadline:MM.dd} {r.Notice.Title}")) +
            ". 시험 전 주로 당겨 처리하세요.";
    }
}
