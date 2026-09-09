namespace CampusSignal.Models;

/// <summary>공고 상세 우측 최상단의 경고 카드.</summary>
public sealed class SideNote
{
    public SideNoteKind Kind { get; init; }
    public required string Title { get; init; }
    public required string Body { get; init; }

    public string Label => Kind switch
    {
        SideNoteKind.DuplicateBenefit => "DUPLICATE BENEFIT",
        SideNoteKind.MissingInput => "MISSING INPUT",
        SideNoteKind.NotEligible => "NOT ELIGIBLE",
        SideNoteKind.ExamConflict => "EXAM CONFLICT",
        _ => "OVERLAP",
    };
}
