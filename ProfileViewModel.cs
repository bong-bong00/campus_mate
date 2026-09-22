using System.Windows.Input;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>판정의 기준값을 채우는 화면. 비어 있는 항목이 무엇을 막고 있는지 되돌려 보여준다.</summary>
public sealed class ProfileViewModel : ScreenViewModel
{
    public ProfileViewModel(AppServices services, IAppNavigator nav) : base(services, nav)
    {
        var p = Store.Profile;

        Groups =
        [
            new ProfileGroupViewModel
            {
                Title = "필수 항목",
                Description = "이 네 가지만 있으면 대부분의 공고를 판정할 수 있습니다",
                Fields =
                [
                    new ProfileFieldViewModel { Label = "학과", Value = p.Department },
                    new ProfileFieldViewModel { Label = "학년 / 학기", Value = $"{p.GradeYear}학년 {p.Term}학기" },
                    new ProfileFieldViewModel
                    {
                        Label = "직전 학기 평점", Field = ProfileField.LastTermGpa,
                        Value = p.Display(ProfileField.LastTermGpa),
                    },
                    new ProfileFieldViewModel
                    {
                        Label = "소득분위", Field = ProfileField.IncomeDecile,
                        Value = p.Display(ProfileField.IncomeDecile),
                    },
                ],
            },
            new ProfileGroupViewModel
            {
                Title = "판정 정확도를 높이는 항목",
                Description = "비어 있으면 해당 요건은 \"확인 필요\"로 남습니다",
                Fields =
                [
                    new ProfileFieldViewModel
                    {
                        Label = "직전 학기 이수 학점", Field = ProfileField.LastTermCredits,
                        Value = p.Display(ProfileField.LastTermCredits),
                    },
                    new ProfileFieldViewModel
                    {
                        Label = "총 취득 학점", Field = ProfileField.TotalCredits,
                        Value = p.Display(ProfileField.TotalCredits),
                    },
                    new ProfileFieldViewModel
                    {
                        Label = "비교과 이수 시간", Field = ProfileField.ExtracurricularHours,
                        Value = p.Display(ProfileField.ExtracurricularHours),
                    },
                    new ProfileFieldViewModel
                    {
                        Label = "어학 성적", Field = ProfileField.LanguageScore,
                        Value = p.Display(ProfileField.LanguageScore),
                    },
                ],
            },
        ];

        UploadTranscriptCommand = new RelayCommand(UploadTranscript);
    }

    public override ScreenKind Kind => ScreenKind.Profile;
    public override string Title => "프로필";
    public override string Subtitle => "여기 채운 값이 모든 판정의 기준이 됩니다";

    public IReadOnlyList<ProfileGroupViewModel> Groups { get; }
    public ICommand UploadTranscriptCommand { get; }

    public string BannerTitle => "성적표를 올리면 아래 항목이 자동으로 채워집니다";
    public string BannerBody => "학점·이수 학점·수강 이력 · 파일은 기기에만 저장됩니다";
    public string BannerButton => "성적표 업로드";

    public string BlockedTitle => "비어 있는 항목이 판정을 막고 있습니다";

    public string BlockedBody => Catalog.TopBlockingField is { } field
        ? $"{field} 하나 때문에 <em>{Catalog.BlockedByTopField}건</em>이 \"확인 필요\"로 남아 있습니다."
        : "지금은 판정이 보류된 공고가 없습니다.";

    public bool HasBlocked => Catalog.BlockedByTopField > 0;

    public IReadOnlyList<KeyValueRow> PrivacyRows => Store.PrivacyRows;
    public string PrivacyCaption => "서버에는 판정에 필요한 최소 항목만 전송됩니다.";
    public IReadOnlyList<KeyValueRow> HistoryRows => Store.BenefitHistoryRows;

    /// <summary>
    /// 성적표 파싱은 후속 작업이라, 지금은 파싱 결과가 채워 넣을 값을 그대로 반영한다.
    /// 프로필이 바뀌면 카탈로그를 다시 계산해 앱 전체 판정이 갱신된다.
    /// </summary>
    private void UploadTranscript()
    {
        var p = Store.Profile;
        p.LastTermCredits = 18;

        foreach (var f in Groups.SelectMany(g => g.Fields).Where(f => f.Field is not null))
            f.Value = p.Display(f.Field!.Value);

        Catalog.Refresh();

        Raise(nameof(BlockedBody));
        Raise(nameof(HasBlocked));

        Nav.ShowToast(new ToastMessage(
            "성적표에서 이수 학점을 읽었습니다",
            "직전 학기 이수 학점 18학점 · 보류돼 있던 공고를 다시 판정했습니다",
            "공고 보기", "닫기", "merit"));
    }
}
