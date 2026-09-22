using System.Windows.Input;
using System.Windows.Media;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>
/// 왜 이 판정이 나왔는지 근거를 보이고, 무엇부터 하면 되는지 알려주는 화면.
/// 요건마다 "공고문 어느 문장에서 뽑았는지"를 함께 보이는 것이 이 화면의 핵심이다.
/// </summary>
public sealed class NoticeDetailViewModel : ScreenViewModel
{
    private NoticeJudgment _result;

    public NoticeDetailViewModel(AppServices services, IAppNavigator nav, string noticeId)
        : base(services, nav)
    {
        _result = Catalog.ById(noticeId);
        BackCommand = new RelayCommand(() => Nav.Go(ScreenKind.NoticeList));
        PrimaryCommand = new RelayCommand(OnPrimary);
        SaveCommand = new RelayCommand(() => Nav.ShowToast(new ToastMessage(
            "공고를 저장했습니다", $"{Notice.Title} · 마감 {Notice.Deadline:MM.dd}", "확인", "닫기", null)));
    }

    public override ScreenKind Kind => ScreenKind.NoticeDetail;
    public override string Title => "공고 상세";
    public override string Subtitle => "요건·서류·중복 수혜를 한 화면에서 확인합니다";

    public Notice Notice => _result.Notice;

    public ICommand BackCommand { get; }
    public ICommand PrimaryCommand { get; }
    public ICommand SaveCommand { get; }

    public string BackText => "← 공고 목록";

    // ── 요약 배너 ─────────────────────────────────────────────────────────
    public string JudgmentText => Palette.Text(_result.Judgment);
    public Brush BadgeBackground => Palette.SolidBg(_result.Judgment);
    public Brush BadgeForeground => Palette.SolidFg(_result.Judgment);
    public Brush HeroBackground => Palette.HeroBg(_result.Judgment);
    public Brush HeroBorder => Palette.HeroBorder(_result.Judgment);
    public string SourceText => Notice.SourceText;
    public string NoticeTitle => Notice.Title;
    public string DeadlineText => Notice.DeadlineText;
    public string DDayText => _result.DDayText;
    public string AmountText => Notice.AmountText;

    // ── 자격 요건 대조 ────────────────────────────────────────────────────
    public string RequirementCaption => "공고문에서 추출한 요건을 내 프로필과 겹쳐 판정했습니다";

    public IReadOnlyList<RequirementRowViewModel> Requirements => _result.Checks
        .Select(c => new RequirementRowViewModel
        {
            Mark = c.Mark,
            MarkBrush = Palette.RequirementMark(c.Result),
            Label = c.Label,
            Evidence = c.Evidence,
            MyValue = c.MyValueText,
        })
        .ToList();

    // ── 필요 서류 · 착수 역산 ─────────────────────────────────────────────
    public IReadOnlyList<DocumentRowViewModel> Documents => Notice.Documents
        .Select(d => new DocumentRowViewModel
        {
            Name = d.Name,
            Issuer = d.Issuer,
            LeadText = d.LeadText,
            StateText = d.StateText,
            StateBackground = Palette.DocumentBg(d.State),
            StateForeground = Palette.DocumentFg(d.State),
        })
        .ToList();

    public string LeadNote => _result.LeadNote;

    // ── 우측 컬럼 ─────────────────────────────────────────────────────────
    public bool HasSideNote => Notice.SideNote is not null;
    public string SideLabel => Notice.SideNote?.Label ?? "";
    public string SideTitle => Notice.SideNote?.Title ?? "";
    public string SideBody => Notice.SideNote?.Body ?? "";
    public Brush SideBackground => Palette.SideBg(Notice.SideNote?.Kind ?? SideNoteKind.ExamConflict);
    public Brush SideBorder => Palette.SideBorder(Notice.SideNote?.Kind ?? SideNoteKind.ExamConflict);
    public Brush SideTone => Palette.SideTone(Notice.SideNote?.Kind ?? SideNoteKind.ExamConflict);

    public string PdfFile => Notice.PdfFile;
    public string PdfNote => Notice.PdfNote;
    public string PdfPlaceholder => "PDF 뷰어 (요건 문장 하이라이트)";
    public string ChangeHistory => Notice.ChangeHistory;

    public string PrimaryText => Notice.CtaText;
    public string SecondaryText => "저장";

    public Brush PrimaryBackground => Notice.CtaFollowsJudgment
        ? Palette.SolidBg(_result.Judgment)
        : Palette.Get("Ink");

    public Brush PrimaryForeground => Notice.CtaFollowsJudgment
        ? Palette.SolidFg(_result.Judgment)
        : Palette.Get("TextOnDark");

    private void OnPrimary()
    {
        // 확인 필요 공고의 주 액션은 신청이 아니라 "판정을 막고 있는 값 채우기"다.
        if (_result.Judgment == Judgment.NeedsCheck)
        {
            Nav.Go(ScreenKind.Profile);
            return;
        }

        if (_result.Judgment == Judgment.Ineligible)
        {
            Nav.Go(ScreenKind.NoticeList);
            return;
        }

        Nav.ShowToast(new ToastMessage(
            Notice.CtaText,
            $"{Notice.Title} · 마감 {Notice.Deadline:MM.dd HH:mm}",
            "확인", "닫기", null));
    }

    /// <summary>프로필이 바뀐 뒤 같은 공고를 다시 볼 때 판정을 새로 읽어온다.</summary>
    public override void Activate()
    {
        _result = Catalog.ById(Notice.Id);
        foreach (var p in new[]
                 {
                     nameof(JudgmentText), nameof(BadgeBackground), nameof(BadgeForeground),
                     nameof(HeroBackground), nameof(HeroBorder), nameof(DDayText),
                     nameof(Requirements), nameof(Documents), nameof(LeadNote),
                     nameof(PrimaryBackground), nameof(PrimaryForeground),
                 })
        {
            Raise(p);
        }
    }
}
