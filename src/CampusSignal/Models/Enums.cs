namespace CampusSignal.Models;

public enum ScreenKind
{
    Dashboard,
    NoticeList,
    NoticeDetail,
    Benefits,
    Calendar,
    Timetable,
    Profile,
    Notifications,
}

/// <summary>공고 하나에 대한 최종 판정.</summary>
public enum Judgment
{
    Eligible,   // 지원 가능
    NeedsCheck, // 확인 필요
    Ineligible, // 자격 미달
}

/// <summary>요건 하나에 대한 대조 결과.</summary>
public enum RequirementResult
{
    Met,      // 충족   ✓
    Unknown,  // 대조할 값 없음 ?
    Failed,   // 미충족 ✕
    Flagged,  // 판정을 막지는 않지만 알려야 하는 사항 !
}

/// <summary>프로필에서 판정에 쓰이는 값의 종류.</summary>
public enum ProfileField
{
    Department,
    GradeYear,
    Term,
    LastTermGpa,
    IncomeDecile,
    LastTermCredits,
    TotalCredits,
    ExtracurricularHours,
    LanguageScore,
    IsEnrolled,
}

public enum Comparison
{
    AtLeast,
    AtMost,
    Equal,
    IsTrue,
}

public enum DocumentState
{
    Held,           // 보유
    ActionNeeded,   // 오늘 신청 / 미작성 / 미업로드
    NotApplicable,  // 해당 없음
}

public enum SideNoteKind
{
    DuplicateBenefit,
    MissingInput,
    NotEligible,
    ExamConflict,
    Overlap,
}

public enum CalendarEventKind
{
    Deadline,
    Exam,
    Academic,
    Personal,
}

public enum AlertKind
{
    Start,     // 착수
    Deadline,  // 마감
    New,       // 신규
    Changed,   // 변경
    Warning,   // 경고
}

public enum BenefitState
{
    TopPick,      // 우선 추천
    Open,         // 신청 가능
    NeedsCheck,   // 확인 필요
    RuleFound,    // 규정 발굴
}

public enum NoticeSource
{
    SchoolRss,
    ScholarshipFoundationApi,
    PublicData,
    SelfRegistered,
    CampusOffice,
}
