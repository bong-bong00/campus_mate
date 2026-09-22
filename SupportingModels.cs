namespace CampusSignal.Models;

/// <summary>혜택 · 비교과 프로그램.</summary>
public sealed class BenefitProgram
{
    public required string Department { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string TimeText { get; init; }
    public required string CreditText { get; init; }
    public BenefitState State { get; init; }
    public required string Category { get; init; }

    public string StateText => State switch
    {
        BenefitState.TopPick => "우선 추천",
        BenefitState.Open => "신청 가능",
        BenefitState.NeedsCheck => "확인 필요",
        _ => "규정 발굴",
    };
}

/// <summary>시간표와 겹쳐 목록에서 뺀 프로그램.</summary>
public sealed record ExcludedProgram(string Title, string ClashText);

/// <summary>주간 시간표의 수업 한 칸.</summary>
public sealed class ClassBlock
{
    public required string Name { get; init; }
    public required string Room { get; init; }

    /// <summary>0 = 월 … 4 = 금.</summary>
    public int DayIndex { get; init; }

    /// <summary>24시간제 시작 시각 (9 = 09:00).</summary>
    public int StartHour { get; init; }

    /// <summary>24시간제 종료 시각 (12 = 12:00).</summary>
    public int EndHour { get; init; }

    public int ToneIndex { get; init; }

    public string DayLabel => DayIndex switch
    {
        0 => "월", 1 => "화", 2 => "수", 3 => "목", _ => "금",
    };

    public bool Overlaps(int dayIndex, int hour) =>
        DayIndex == dayIndex && hour >= StartHour && hour < EndHour;
}

/// <summary>캘린더 셀 안의 이벤트 칩.</summary>
public sealed record CalendarEvent(string Label, CalendarEventKind Kind, DateTime Date);

/// <summary>알림 한 건.</summary>
public sealed class AlertItem
{
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required string TimeText { get; init; }
    public AlertKind Kind { get; init; }
    public bool IsUnread { get; init; }

    /// <summary>클릭했을 때 열 공고 (없으면 화면 이동만 한다).</summary>
    public string? NoticeId { get; init; }
    public ScreenKind? TargetScreen { get; init; }

    public string KindText => Kind switch
    {
        AlertKind.Start => "착수",
        AlertKind.Deadline => "마감",
        AlertKind.New => "신규",
        AlertKind.Changed => "변경",
        _ => "경고",
    };
}

/// <summary>알림 설정 토글.</summary>
public sealed record NotificationSetting(string Label, bool Enabled);

/// <summary>프로필 우측 카드의 키-값 행.</summary>
public sealed record KeyValueRow(string Key, string Value, string ToneKey = "TextMuted2");

/// <summary>인앱 토스트 내용.</summary>
public sealed record ToastMessage(
    string Title,
    string Body,
    string PrimaryText,
    string SecondaryText,
    string? NoticeId);
