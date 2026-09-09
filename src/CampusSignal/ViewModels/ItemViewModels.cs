using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>헤더의 내비게이션 탭 하나.</summary>
public sealed class NavItemViewModel(string label, ScreenKind screen, ICommand command) : ObservableObject
{
    private bool _isActive;

    public string Label { get; private set; } = label;
    public ScreenKind Screen { get; } = screen;
    public ICommand Command { get; } = command;

    public bool IsActive
    {
        get => _isActive;
        set => Set(ref _isActive, value);
    }

    public void SetLabel(string value)
    {
        Label = value;
        Raise(nameof(Label));
    }
}

/// <summary>대시보드 "그다음 할 일" 한 줄.</summary>
public sealed class TodoRowViewModel
{
    public required string Urgency { get; init; }
    public required Brush UrgencyBrush { get; init; }
    public required string Title { get; init; }
    public required string JudgmentText { get; init; }
    public required Brush BadgeBackground { get; init; }
    public required Brush BadgeForeground { get; init; }
    public required string Note { get; init; }
    public required string ActionText { get; init; }
    public required ICommand Command { get; init; }
}

/// <summary>대시보드의 추천 프로그램 카드.</summary>
public sealed record ProgramCardViewModel(string Tag, string Title, string Meta, ICommand Command);

/// <summary>대시보드 "이번 주 캘린더" 한 줄.</summary>
public sealed record WeekRowViewModel(string DayLabel, string Title, Brush Dot);

/// <summary>공고 목록의 데이터 행.</summary>
public sealed class NoticeRowViewModel
{
    public required string Id { get; init; }
    public required string DDay { get; init; }
    public required Brush DDayBrush { get; init; }
    public required string Title { get; init; }
    public required string Note { get; init; }
    public required string JudgmentText { get; init; }
    public required Brush BadgeBackground { get; init; }
    public required Brush BadgeForeground { get; init; }
    public required string SourceShort { get; init; }
    public required string RequirementRatio { get; init; }
    public required ICommand Command { get; init; }
}

/// <summary>목록 사이드바의 판정 필터 (단일 선택).</summary>
public sealed class JudgmentFilterViewModel(string label, Judgment? judgment, bool imminent, Brush dot)
    : ObservableObject
{
    private bool _isSelected;
    private int _count;

    public string Label { get; } = label;
    public Judgment? Judgment { get; } = judgment;

    /// <summary>"마감 임박" 처럼 판정이 아니라 기한으로 거르는 필터인지.</summary>
    public bool IsImminentFilter { get; } = imminent;

    public Brush Dot { get; } = dot;
    public ICommand? Command { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set { if (Set(ref _isSelected, value)) { Raise(nameof(Weight)); } }
    }

    public int Count
    {
        get => _count;
        set => Set(ref _count, value);
    }

    public FontWeight Weight => IsSelected ? FontWeights.SemiBold : FontWeights.Normal;
}

/// <summary>목록 사이드바의 출처 필터 (다중 선택).</summary>
public sealed class SourceFilterViewModel(NoticeSource source, string label) : ObservableObject
{
    private bool _isSelected = true;
    private int _count;

    public NoticeSource Source { get; } = source;
    public string Label { get; } = label;
    public ICommand? Command { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => Set(ref _isSelected, value);
    }

    public int Count
    {
        get => _count;
        set => Set(ref _count, value);
    }
}

/// <summary>상세 화면의 요건 대조 행.</summary>
public sealed class RequirementRowViewModel
{
    public required string Mark { get; init; }
    public required Brush MarkBrush { get; init; }
    public required string Label { get; init; }
    public required string Evidence { get; init; }
    public required string MyValue { get; init; }
}

/// <summary>상세 화면의 필요 서류 행.</summary>
public sealed class DocumentRowViewModel
{
    public required string Name { get; init; }
    public required string Issuer { get; init; }
    public required string LeadText { get; init; }
    public required string StateText { get; init; }
    public required Brush StateBackground { get; init; }
    public required Brush StateForeground { get; init; }
}

/// <summary>혜택 카드.</summary>
public sealed class BenefitCardViewModel
{
    public required string Department { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string TimeText { get; init; }
    public required string CreditText { get; init; }
    public required string StateText { get; init; }
    public required Brush StateBackground { get; init; }
    public required Brush StateForeground { get; init; }
    public required Brush CardBackground { get; init; }
    public required Brush CardBorder { get; init; }
}

/// <summary>혜택 화면의 분류 탭.</summary>
public sealed class BenefitTabViewModel(string category, string label) : ObservableObject
{
    private bool _isActive;

    public string Category { get; } = category;
    public string Label { get; } = label;
    public ICommand? Command { get; set; }

    public bool IsActive
    {
        get => _isActive;
        set => Set(ref _isActive, value);
    }
}

/// <summary>캘린더 범례 항목.</summary>
public sealed record LegendItemViewModel(string Label, Brush Color);

/// <summary>캘린더 셀 안의 이벤트 칩.</summary>
public sealed record EventChipViewModel(string Label, Brush Background, Brush Foreground);

/// <summary>월간 격자의 칸 하나.</summary>
public sealed class CalendarCellViewModel
{
    public required string Number { get; init; }
    public required Brush NumberBrush { get; init; }
    public required Brush Background { get; init; }
    public required IReadOnlyList<EventChipViewModel> Events { get; init; }
}

/// <summary>주간 시간표의 수업 블록.</summary>
public sealed class ClassBlockViewModel
{
    public required string Name { get; init; }
    public required string Room { get; init; }
    public required int Row { get; init; }
    public required int RowSpan { get; init; }
    public required int Column { get; init; }
    public required Brush Fill { get; init; }
    public required Brush Bar { get; init; }
}

/// <summary>프로필 필드 카드. 비어 있으면 경고색으로 바뀐다.</summary>
public sealed class ProfileFieldViewModel : ObservableObject
{
    private string _value = "";

    public required string Label { get; init; }
    public ProfileField? Field { get; init; }

    public string Value
    {
        get => _value;
        set
        {
            if (!Set(ref _value, value)) return;
            Raise(nameof(IsMissing));
            Raise(nameof(Background));
            Raise(nameof(BorderBrush));
            Raise(nameof(Foreground));
        }
    }

    public bool IsMissing => string.IsNullOrWhiteSpace(Value);

    public string DisplayValue => IsMissing ? "입력 필요" : Value;

    public Brush Background => Palette.Get(IsMissing ? "WarnSurface" : "SurfaceRaised");
    public Brush BorderBrush => Palette.Get(IsMissing ? "BorderWarn" : "BorderInput");
    public Brush Foreground => Palette.Get(IsMissing ? "FailFg" : "TextPrimary");
}

public sealed class ProfileGroupViewModel
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<ProfileFieldViewModel> Fields { get; init; }
}

/// <summary>알림 목록의 행.</summary>
public sealed class AlertRowViewModel
{
    public required Brush Dot { get; init; }
    public required string KindText { get; init; }
    public required Brush KindBackground { get; init; }
    public required Brush KindForeground { get; init; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required string TimeText { get; init; }
    public required Brush RowBackground { get; init; }
    public required ICommand Command { get; init; }
}

/// <summary>알림 설정 토글 한 줄.</summary>
public sealed class SettingRowViewModel(string label, bool enabled) : ObservableObject
{
    private bool _isEnabled = enabled;

    public string Label { get; } = label;
    public ICommand? Command { get; set; }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => Set(ref _isEnabled, value);
    }
}

/// <summary>D-day 색 규칙 — 5일 이내는 경고색, 착수 시점은 파란 계열.</summary>
public static class Urgency
{
    public const int UrgentDays = 5;

    public static Brush ToneFor(NoticeJudgment r, bool allowStartTone)
    {
        if (allowStartTone && r.IsStartDue && r.Judgment != Judgment.Ineligible)
            return Palette.Get("StartTone");
        return Palette.Get(r.DaysLeft <= UrgentDays ? "UrgentTone" : "CalmTone");
    }
}
