namespace CampusSignal.Models;

/// <summary>
/// 판정의 기준값. 비어 있는 항목(null)은 "자격 미달"이 아니라 "확인 필요"를 만든다.
/// </summary>
public sealed class StudentProfile
{
    public string Department { get; set; } = "";
    public int GradeYear { get; set; }
    public int Term { get; set; }
    public double? LastTermGpa { get; set; }
    public double GpaScale { get; set; } = 4.5;
    public int? IncomeDecile { get; set; }
    public int? LastTermCredits { get; set; }
    public int? TotalCredits { get; set; }
    public int? ExtracurricularHours { get; set; }
    public int ExtracurricularRequired { get; set; } = 30;
    public int? LanguageScore { get; set; }
    public bool IsEnrolled { get; set; } = true;

    public double? Numeric(ProfileField field) => field switch
    {
        ProfileField.GradeYear => GradeYear,
        ProfileField.Term => Term,
        ProfileField.LastTermGpa => LastTermGpa,
        ProfileField.IncomeDecile => IncomeDecile,
        ProfileField.LastTermCredits => LastTermCredits,
        ProfileField.TotalCredits => TotalCredits,
        ProfileField.ExtracurricularHours => ExtracurricularHours,
        ProfileField.LanguageScore => LanguageScore,
        ProfileField.IsEnrolled => IsEnrolled ? 1 : 0,
        _ => null,
    };

    public string Display(ProfileField field) => field switch
    {
        ProfileField.Department => Department,
        ProfileField.GradeYear => $"{GradeYear}학년",
        ProfileField.Term => $"{Term}학기",
        ProfileField.LastTermGpa => LastTermGpa is null ? "" : $"{LastTermGpa:0.00} / {GpaScale:0.0}",
        ProfileField.IncomeDecile => IncomeDecile is null ? "" : $"{IncomeDecile}분위",
        ProfileField.LastTermCredits => LastTermCredits is null ? "" : $"{LastTermCredits}학점",
        ProfileField.TotalCredits => TotalCredits is null ? "" : $"{TotalCredits}학점",
        ProfileField.ExtracurricularHours =>
            ExtracurricularHours is null ? "" : $"{ExtracurricularHours} / {ExtracurricularRequired}시간",
        ProfileField.LanguageScore => LanguageScore is null ? "" : $"{LanguageScore}점",
        ProfileField.IsEnrolled => IsEnrolled ? "재학" : "휴학",
        _ => "",
    };

    public static string Label(ProfileField field) => field switch
    {
        ProfileField.Department => "학과",
        ProfileField.GradeYear => "학년",
        ProfileField.Term => "학기",
        ProfileField.LastTermGpa => "직전 학기 평점",
        ProfileField.IncomeDecile => "소득분위",
        ProfileField.LastTermCredits => "직전 학기 이수 학점",
        ProfileField.TotalCredits => "총 취득 학점",
        ProfileField.ExtracurricularHours => "비교과 이수 시간",
        ProfileField.LanguageScore => "어학 성적",
        ProfileField.IsEnrolled => "재학 여부",
        _ => "",
    };
}
