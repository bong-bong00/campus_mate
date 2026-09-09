namespace CampusSignal.Models;

/// <summary>
/// 공고문에서 추출한 자격 요건 하나.
/// <see cref="Evidence"/> 는 "어느 문장에서 뽑았는지"이며, 상세 화면의 핵심 신뢰 장치다.
/// </summary>
public sealed class Requirement
{
    public required string Label { get; init; }

    /// <summary>공고문 근거 (예: '공고문 3p · "직전 학기 12학점 이상"').</summary>
    public required string Evidence { get; init; }

    /// <summary>대조할 프로필 항목. null 이면 프로필에 대응 항목이 없는 요건이다.</summary>
    public ProfileField? Field { get; init; }

    public Comparison Comparison { get; init; } = Comparison.AtLeast;

    public double Threshold { get; init; }

    /// <summary>
    /// 출처(API·학사 시스템)가 값을 직접 제공하는 경우. 프로필보다 우선한다.
    /// 예: 한국장학재단 API 는 직전 학기 이수 학점을 함께 내려준다.
    /// </summary>
    public double? ProvidedValue { get; init; }

    /// <summary>표시용 값을 직접 지정할 때 (제공값·자기신고 항목).</summary>
    public string? ProvidedDisplay { get; init; }

    /// <summary>판정을 막지는 않지만 반드시 알려야 하는 사항 (중복 수혜·시간표 충돌 등).</summary>
    public bool IsFlag { get; init; }

    /// <summary>플래그가 실제 충돌인지(false = 확인 결과 무관).</summary>
    public bool FlagConflicts { get; init; } = true;

    /// <summary>프로필에 없는 값을 요구할 때 사용자에게 보여줄 항목 이름.</summary>
    public string? MissingFieldLabel { get; init; }
}

/// <summary>요건 하나를 프로필과 대조한 결과.</summary>
public sealed record RequirementCheck(
    Requirement Requirement,
    RequirementResult Result,
    string MyValueText)
{
    public string Label => Requirement.Label;
    public string Evidence => Requirement.Evidence;

    public string Mark => Result switch
    {
        RequirementResult.Met => "✓",
        RequirementResult.Unknown => "?",
        RequirementResult.Failed => "✕",
        _ => "!",
    };
}
