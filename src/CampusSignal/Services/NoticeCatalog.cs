using CampusSignal.Models;

namespace CampusSignal.Services;

/// <summary>
/// 수집된 공고 전체를 프로필과 대조해 둔 결과 집합.
/// 프로필이 바뀌면 <see cref="Refresh"/> 한 번으로 앱 전체 판정이 다시 계산된다.
/// </summary>
public sealed class NoticeCatalog
{
    private readonly SampleDataStore _store;
    private readonly JudgmentEngine _engine;
    private readonly IClock _clock;
    private List<NoticeJudgment> _results = [];

    public NoticeCatalog(SampleDataStore store, JudgmentEngine engine, IClock clock)
    {
        _store = store;
        _engine = engine;
        _clock = clock;
        Refresh();
    }

    /// <summary>프로필이나 수집 결과가 바뀌었을 때.</summary>
    public event Action? Changed;

    public IReadOnlyList<NoticeJudgment> All => _results;

    public void Refresh()
    {
        _results = _store.Notices
            .Select(n => _engine.Evaluate(n, _store.Profile))
            .OrderBy(r => r.Notice.Deadline)
            .ToList();
        Changed?.Invoke();
    }

    public NoticeJudgment ById(string id) => _results.First(r => r.Notice.Id == id);

    public int CountOf(Judgment j) => _results.Count(r => r.Judgment == j);

    /// <summary>7일 안에 마감되는 건수.</summary>
    public int DueWithin(int days) =>
        _results.Count(r => r.DaysLeft >= 0 && r.DaysLeft <= days);

    /// <summary>목록 사이드바의 "마감 임박" 필터 기준.</summary>
    public const int ImminentDays = 7;

    public int ImminentCount => DueWithin(ImminentDays);

    public int SourceCount(NoticeSource s) => _results.Count(r => r.Notice.Source == s);

    /// <summary>판정을 가장 많이 막고 있는 프로필 항목.</summary>
    public string? TopBlockingField => _results
        .Where(r => r.Judgment == Judgment.NeedsCheck)
        .SelectMany(r => r.BlockingFields)
        .GroupBy(f => f)
        .OrderByDescending(g => g.Count())
        .Select(g => g.Key)
        .FirstOrDefault();

    /// <summary>그 항목 때문에 보류된 건수.</summary>
    public int BlockedByTopField
    {
        get
        {
            var field = TopBlockingField;
            return field is null
                ? 0
                : _results.Count(r => r.Judgment == Judgment.NeedsCheck && r.BlockingFields.Contains(field));
        }
    }

    /// <summary>대시보드 히어로 — 가장 급한 단 한 건. 자격 미달은 제외한다.</summary>
    public NoticeJudgment? Hero => Ranked().FirstOrDefault();

    /// <summary>히어로를 뺀 "그다음 할 일".</summary>
    public IEnumerable<NoticeJudgment> Todos(int take)
    {
        var hero = Hero;
        return _results
            .Where(r => r != hero && r.DaysLeft >= 0)
            .OrderBy(r => r.Judgment == Judgment.Ineligible ? 1 : 0)
            .ThenByDescending(r => r.IsStartDue)
            .ThenBy(r => r.DaysLeft)
            .Take(take);
    }

    private IEnumerable<NoticeJudgment> Ranked() => _results
        .Where(r => r.Judgment != Judgment.Ineligible && r.DaysLeft >= 0)
        .OrderByDescending(r => r.IsStartDue)
        .ThenBy(r => r.DaysLeft);

    /// <summary>공고 마감을 캘린더 이벤트로 변환한 것.</summary>
    public IEnumerable<CalendarEvent> DeadlineEvents() => _results
        .Select(r => new CalendarEvent(
            $"{Shorten(r.Notice.Title)} 마감", CalendarEventKind.Deadline, r.Notice.Deadline.Date));

    /// <summary>오늘 착수해야 하는 건들을 하나의 학사 이벤트로 묶은 것.</summary>
    public IEnumerable<CalendarEvent> StartEvents()
    {
        foreach (var group in _results
                     .Where(r => r.Judgment != Judgment.Ineligible && r.StartDate >= _clock.Today)
                     .GroupBy(r => r.StartDate))
        {
            yield return new CalendarEvent($"서류 착수 ×{group.Count()}", CalendarEventKind.Academic, group.Key);
        }
    }

    private static string Shorten(string title)
    {
        var cut = title.IndexOf(' ');
        return cut > 0 && title.Length > 12 ? title[..cut] : title;
    }
}
