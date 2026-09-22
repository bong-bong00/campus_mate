using CampusSignal.Models;

namespace CampusSignal.Services;

/// <summary>
/// 핸드오프 문서의 "핵심 파생 로직 두 가지".
///
/// 1) 자격 판정 — 요건 × 프로필 → 요건별 Met / Unknown / Failed
///    · 하나라도 Failed  → 자격 미달
///    · Failed 없고 Unknown 있음 → 확인 필요 (어떤 항목 때문인지 함께 돌려준다)
///    · 전부 Met → 지원 가능
///
/// 2) 착수일 역산 — 착수일 = 마감일 − max(서류별 발급 소요 영업일) − 작성 여유
/// </summary>
public sealed class JudgmentEngine(IClock clock)
{
    /// <summary>신청서 작성에 남겨두는 여유 (영업일).</summary>
    public int WritingBufferDays { get; init; } = 1;

    /// <summary>중간고사 등 시험 기간. 착수 구간이 여기 걸리면 충돌 경고를 만든다.</summary>
    public (DateTime Start, DateTime End) ExamPeriod { get; init; } =
        (new DateTime(2025, 10, 20), new DateTime(2025, 10, 24));

    public NoticeJudgment Evaluate(Notice notice, StudentProfile profile)
    {
        var checks = notice.Requirements.Select(r => Check(r, profile)).ToList();

        var judgment =
            checks.Any(c => c.Result == RequirementResult.Failed) ? Judgment.Ineligible
            : checks.Any(c => c.Result == RequirementResult.Unknown) ? Judgment.NeedsCheck
            : Judgment.Eligible;

        var blocking = checks
            .Where(c => c.Result == RequirementResult.Unknown)
            .Select(c => c.Requirement.MissingFieldLabel
                         ?? (c.Requirement.Field is { } f ? StudentProfile.Label(f) : c.Label))
            .Distinct()
            .ToList();

        // 요건 비율: 판정을 막지 않는 "충돌 플래그"는 분모에서 뺀다.
        // 확인 결과 무관으로 정리된 플래그는 충족으로 센다.
        var counted = checks
            .Where(c => !(c.Requirement.IsFlag && c.Requirement.FlagConflicts))
            .ToList();

        var (start, longest, leadDays) = BackCalculateStart(notice);
        var today = clock.Today;

        return new NoticeJudgment
        {
            Notice = notice,
            Judgment = judgment,
            Checks = checks,
            MetCount = counted.Count(c => c.Result is RequirementResult.Met or RequirementResult.Flagged),
            TotalCount = counted.Count,
            BlockingFields = blocking,
            StartDate = start,
            IsStartDue = start <= today,
            LongestDocument = longest,
            DaysLeft = (notice.Deadline.Date - today).Days,
            LeadNote = BuildLeadNote(notice, judgment, start, longest, leadDays),
            ClashesWithExams = Overlaps(start, notice.Deadline.Date),
        };
    }

    private static RequirementCheck Check(Requirement r, StudentProfile profile)
    {
        if (r.IsFlag)
        {
            var flagText = r.ProvidedDisplay ?? (r.FlagConflicts ? "충돌 1건" : "무관");
            return new RequirementCheck(r, RequirementResult.Flagged, flagText);
        }

        // 출처가 값을 직접 제공하면 프로필보다 우선한다.
        var value = r.ProvidedValue ?? (r.Field is { } field ? profile.Numeric(field) : null);

        if (value is null)
        {
            return new RequirementCheck(r, RequirementResult.Unknown,
                r.ProvidedDisplay ?? "미입력 — 확인 필요");
        }

        var met = r.Comparison switch
        {
            Comparison.AtLeast => value >= r.Threshold,
            Comparison.AtMost => value <= r.Threshold,
            Comparison.Equal => Math.Abs(value.Value - r.Threshold) < 1e-9,
            Comparison.IsTrue => value.Value != 0,
            _ => false,
        };

        var display = r.ProvidedDisplay
                      ?? (r.Field is { } f2 ? profile.Display(f2) : value.Value.ToString("0.##"));

        return new RequirementCheck(
            r,
            met ? RequirementResult.Met : RequirementResult.Failed,
            met ? display : $"{display} — 미달");
    }

    /// <summary>착수일 = 마감일 − max(미보유 서류 소요 영업일) − 작성 여유.</summary>
    private (DateTime Start, RequiredDocument? Longest, int LeadDays) BackCalculateStart(Notice notice)
    {
        var pending = notice.Documents.Where(d => d.CountsTowardLeadTime).ToList();
        var longest = pending.MaxBy(d => d.LeadBusinessDays);
        var leadDays = longest?.LeadBusinessDays ?? 0;
        var start = BusinessDays.Subtract(notice.Deadline.Date, leadDays + WritingBufferDays);
        return (start, longest, leadDays);
    }

    private string BuildLeadNote(
        Notice notice, Judgment judgment, DateTime start, RequiredDocument? longest, int leadDays)
    {
        if (judgment == Judgment.Ineligible)
            return "필수 요건 하나가 미달이라 서류를 준비할 필요가 없습니다. 프로필 값이 바뀌면 자동으로 다시 판정합니다.";

        var startText = $"{start:MM.dd}({BusinessDays.KoreanDay(start)})";
        var when = start < clock.Today ? $"<em>{startText}</em> 이 이미 지났습니다. 오늘 바로 착수하세요."
                 : start == clock.Today ? $"<em>{startText} 오늘</em> 신청하면 됩니다."
                 : $"<em>{startText}</em> 까지 신청하면 됩니다.";

        if (longest is null)
            return $"필요 서류가 모두 즉시 발급 가능합니다. 마감 {notice.Deadline:MM.dd} 기준 작성 여유 " +
                   $"{WritingBufferDays}영업일을 두면 {when}";

        return $"가장 오래 걸리는 서류가 <em>{longest.Name}({longest.LeadText})</em> 입니다. " +
               $"마감 {notice.Deadline:MM.dd} 에서 {leadDays}영업일 + 작성 여유 {WritingBufferDays}영업일을 빼면 {when}";
    }

    private bool Overlaps(DateTime start, DateTime end) =>
        start <= ExamPeriod.End && end >= ExamPeriod.Start;
}
