using CampusSignal.Models;

namespace CampusSignal.Services;

/// <summary>판정 엔진이 공고 하나에 대해 내놓는 결과 전체.</summary>
public sealed class NoticeJudgment
{
    public required Notice Notice { get; init; }
    public Judgment Judgment { get; init; }
    public required IReadOnlyList<RequirementCheck> Checks { get; init; }

    /// <summary>목록의 "요건" 열 (충족/전체). 정보성 충돌 플래그는 분모에서 뺀다.</summary>
    public int MetCount { get; init; }
    public int TotalCount { get; init; }
    public string RequirementRatio => $"{MetCount}/{TotalCount}";

    /// <summary>판정을 보류시킨 프로필 항목 이름들.</summary>
    public required IReadOnlyList<string> BlockingFields { get; init; }

    /// <summary>서류 발급을 역산한 착수일. 서류가 없으면 마감 하루 전 영업일.</summary>
    public DateTime StartDate { get; init; }

    /// <summary>착수일이 오늘 이하 — 할 일 목록에서 D-day 대신 "착수"로 표시한다.</summary>
    public bool IsStartDue { get; init; }

    public RequiredDocument? LongestDocument { get; init; }

    /// <summary>마감까지 남은 일수. 음수면 이미 지났다.</summary>
    public int DaysLeft { get; init; }

    public string DDayText => DaysLeft switch
    {
        < 0 => "마감",
        0 => "D-DAY",
        _ => $"D-{DaysLeft}",
    };

    /// <summary>할 일 목록의 1열에 실제로 찍히는 문구.</summary>
    public string UrgencyText => IsStartDue && Judgment != Judgment.Ineligible ? "착수" : DDayText;

    /// <summary>역산 결론 카드 문장. 강조할 부분은 &lt;em&gt; 로 감싼다.</summary>
    public required string LeadNote { get; init; }

    /// <summary>착수 구간이 시험 기간과 겹치는지.</summary>
    public bool ClashesWithExams { get; init; }
}
