namespace CampusSignal.Models;

/// <summary>수집된 공고 한 건. 판정 결과는 여기 저장하지 않고 판정 엔진이 계산한다.</summary>
public sealed class Notice
{
    public required string Id { get; init; }
    public required string Title { get; init; }

    /// <summary>목록 행의 부제 (수집 상황·주의점).</summary>
    public required string ListNote { get; init; }

    /// <summary>상세 화면에 쓰는 출처 표기.</summary>
    public required string SourceText { get; init; }

    /// <summary>필터용 출처 분류.</summary>
    public NoticeSource Source { get; init; }

    /// <summary>목록의 출처 열에 쓰는 짧은 표기.</summary>
    public required string SourceShort { get; init; }

    public DateTime Deadline { get; init; }

    /// <summary>마감 표기 (마감 10.16 18:00).</summary>
    public string DeadlineText => $"마감 {Deadline:MM.dd HH:mm}";

    public required string AmountText { get; init; }

    public IReadOnlyList<Requirement> Requirements { get; init; } = [];
    public IReadOnlyList<RequiredDocument> Documents { get; init; } = [];

    public SideNote? SideNote { get; init; }

    public required string PdfFile { get; init; }
    public required string PdfNote { get; init; }
    public required string ChangeHistory { get; init; }
    public required string CtaText { get; init; }

    /// <summary>주 액션 버튼을 잉크색이 아닌 판정색으로 칠할지 (확인 필요·자격 미달 공고).</summary>
    public bool CtaFollowsJudgment { get; init; }
}
