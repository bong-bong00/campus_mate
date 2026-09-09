namespace CampusSignal.Models;

/// <summary>필요 서류 한 건. 발급 소요 영업일이 착수일 역산의 입력이다.</summary>
public sealed class RequiredDocument
{
    public required string Name { get; init; }

    /// <summary>발급처 (정부24 · 학교 포털 · 직접 작성 …).</summary>
    public required string Issuer { get; init; }

    /// <summary>발급 소요 영업일. 0 이면 즉시 발급.</summary>
    public int LeadBusinessDays { get; init; }

    /// <summary>소요기간 표기 (즉시 발급 · 3영업일 · 2일 예상 …).</summary>
    public required string LeadText { get; init; }

    public DocumentState State { get; init; }

    /// <summary>상태 배지 문구 (보유 · 오늘 신청 · 미작성 · 미업로드 · 해당 없음).</summary>
    public required string StateText { get; init; }

    /// <summary>아직 손에 없어서 역산에 들어가는 서류인지.</summary>
    public bool CountsTowardLeadTime => State == DocumentState.ActionNeeded;
}
