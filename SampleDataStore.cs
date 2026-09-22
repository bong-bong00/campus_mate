using CampusSignal.Models;

namespace CampusSignal.Services;

/// <summary>
/// 수집기(RSS · 공공 API · 직접 등록)가 붙기 전까지 앱을 굴리는 표본 데이터.
/// 판정 결과는 여기 적지 않는다 — 요건과 서류만 넣고 <see cref="JudgmentEngine"/> 가 계산한다.
/// </summary>
public sealed class SampleDataStore
{
    public StudentProfile Profile { get; } = new()
    {
        Department = "컴퓨터공학과",
        GradeYear = 3,
        Term = 2,
        LastTermGpa = 4.02,
        GpaScale = 4.5,
        IncomeDecile = 8,
        LastTermCredits = null,      // 비어 있어 여러 공고의 판정을 보류시킨다
        TotalCredits = 96,
        ExtracurricularHours = 14,
        ExtracurricularRequired = 30,
        LanguageScore = null,        // 비어 있음
        IsEnrolled = true,
    };

    // 자주 쓰는 요건들 -------------------------------------------------------
    private static Requirement Enrolled(string evidence) => new()
    {
        Label = "재학 중 (휴학자 제외)",
        Evidence = evidence,
        Field = ProfileField.IsEnrolled,
        Comparison = Comparison.IsTrue,
    };

    private static Requirement Gpa(string label, double threshold, string evidence) => new()
    {
        Label = label,
        Evidence = evidence,
        Field = ProfileField.LastTermGpa,
        Comparison = Comparison.AtLeast,
        Threshold = threshold,
    };

    public IReadOnlyList<Notice> Notices { get; }

    public SampleDataStore()
    {
        Notices =
        [
            new Notice
            {
                Id = "nsf2",
                Title = "국가장학금 II유형 2학기 신청",
                ListNote = "출처 2곳에서 수집 · [수정] 재공고 반영",
                SourceText = "한국장학재단 API · 학생지원팀 RSS (출처 2)",
                Source = NoticeSource.ScholarshipFoundationApi,
                SourceShort = "장학재단 API",
                Deadline = new DateTime(2025, 10, 16, 18, 0, 0),
                AmountText = "지원금 최대 1,350,000원",
                Requirements =
                [
                    new Requirement
                    {
                        Label = "직전 학기 12학점 이상 이수",
                        Evidence = "공고문 3p · \"직전 학기 12학점 이상\"",
                        Field = ProfileField.LastTermCredits,
                        Comparison = Comparison.AtLeast,
                        Threshold = 12,
                        // 장학재단 API 가 학적 정보를 함께 내려주므로 프로필이 비어도 대조된다.
                        ProvidedValue = 18,
                        ProvidedDisplay = "18학점 (API 제공)",
                    },
                    Gpa("직전 학기 성적 80/100 이상", 3.6, "공고문 3p"),
                    new Requirement
                    {
                        Label = "소득분위 8분위 이하",
                        Evidence = "공고문 2p · 별표1",
                        Field = ProfileField.IncomeDecile,
                        Comparison = Comparison.AtMost,
                        Threshold = 8,
                    },
                    Enrolled("공고문 2p"),
                    new Requirement
                    {
                        Label = "타 장학금 중복 수혜 제한",
                        Evidence = "공고문 4p · 붙임2",
                        IsFlag = true,
                    },
                ],
                Documents =
                [
                    new RequiredDocument { Name = "가족관계증명서", Issuer = "정부24", LeadBusinessDays = 0, LeadText = "즉시 발급", State = DocumentState.Held, StateText = "보유" },
                    new RequiredDocument { Name = "성적증명서 (국문)", Issuer = "학교 포털", LeadBusinessDays = 0, LeadText = "즉시 발급", State = DocumentState.Held, StateText = "보유" },
                    new RequiredDocument { Name = "영문 성적증명서", Issuer = "학사지원팀", LeadBusinessDays = 3, LeadText = "3영업일", State = DocumentState.ActionNeeded, StateText = "오늘 신청" },
                ],
                SideNote = new SideNote
                {
                    Kind = SideNoteKind.DuplicateBenefit,
                    Title = "교내 성적우수 장학과 동시 수령 불가",
                    Body = "수혜액 기준 II유형(135만) > 성적우수(80만). II유형을 권장합니다.",
                },
                PdfFile = "2025_2학기_국가장학금.pdf · 4p",
                PdfNote = "3페이지 \"직전 학기 12학점 이상 이수\" 문장에서 요건을 추출했습니다.",
                ChangeHistory = "10.09 [수정] 재공고 — 마감이 10.14 → 10.16으로 변경되었습니다.",
                CtaText = "신청서 작성하기",
            },

            new Notice
            {
                Id = "merit",
                Title = "교내 성적우수 장학",
                ListNote = "직전 학기 이수 학점 미확인",
                SourceText = "학생지원팀 RSS",
                Source = NoticeSource.SchoolRss,
                SourceShort = "학생지원팀",
                Deadline = new DateTime(2025, 10, 18, 23, 59, 0),
                AmountText = "지원금 800,000원",
                Requirements =
                [
                    new Requirement
                    {
                        Label = "직전 학기 성적 상위 10%",
                        Evidence = "공고문 1p",
                        Comparison = Comparison.AtMost,
                        Threshold = 10,
                        ProvidedValue = 7,               // 석차는 학사 시스템이 계산해 내려준다
                        ProvidedDisplay = "상위 7%",
                    },
                    new Requirement
                    {
                        Label = "직전 학기 12학점 이상 이수",
                        Evidence = "공고문 1p · 단서 조항",
                        Field = ProfileField.LastTermCredits,
                        Comparison = Comparison.AtLeast,
                        Threshold = 12,
                    },
                    Enrolled("공고문 1p"),
                    new Requirement
                    {
                        Label = "국가장학금 중복 수혜 제한",
                        Evidence = "공고문 2p",
                        IsFlag = true,
                    },
                ],
                Documents =
                [
                    new RequiredDocument { Name = "성적증명서 (국문)", Issuer = "학교 포털", LeadBusinessDays = 0, LeadText = "즉시 발급", State = DocumentState.Held, StateText = "보유" },
                    new RequiredDocument { Name = "재학증명서", Issuer = "학교 포털", LeadBusinessDays = 0, LeadText = "즉시 발급", State = DocumentState.Held, StateText = "보유" },
                ],
                SideNote = new SideNote
                {
                    Kind = SideNoteKind.MissingInput,
                    Title = "프로필 한 항목이 판정을 막고 있습니다",
                    Body = "직전 학기 이수 학점을 채우면 이 공고를 포함해 보류된 공고가 즉시 판정됩니다. 성적표 업로드로 자동 입력할 수 있습니다.",
                },
                PdfFile = "2025-2_성적우수장학_공고.pdf · 2p",
                PdfNote = "1페이지 단서 조항에서 이수 학점 요건을 발견했으나, 대조할 값이 없어 판정을 보류했습니다.",
                ChangeHistory = "변경 이력 없음 · 10.06 최초 등록",
                CtaText = "성적표 올리고 판정 다시 받기",
                CtaFollowsJudgment = true,
            },

            new Notice
            {
                Id = "toeic",
                Title = "토익 응시료 지원 (2차)",
                ListNote = "응시확인서 필요 · 시험 주간 겹침",
                SourceText = "교양교육원 · 직접 등록",
                Source = NoticeSource.SelfRegistered,
                SourceShort = "교양교육원",
                Deadline = new DateTime(2025, 10, 22, 17, 0, 0),
                AmountText = "응시료 50,000원 환급",
                Requirements =
                [
                    new Requirement
                    {
                        Label = "학기당 2회 이내",
                        Evidence = "공고문 1p",
                        Comparison = Comparison.AtMost,
                        Threshold = 2,
                        ProvidedValue = 1,
                        ProvidedDisplay = "올해 1회",
                    },
                    Enrolled("공고문 1p"),
                ],
                Documents =
                [
                    new RequiredDocument { Name = "응시확인서", Issuer = "토익 사이트", LeadBusinessDays = 0, LeadText = "즉시 출력", State = DocumentState.ActionNeeded, StateText = "미업로드" },
                    new RequiredDocument { Name = "응시료 영수증", Issuer = "토익 사이트", LeadBusinessDays = 0, LeadText = "즉시 출력", State = DocumentState.Held, StateText = "보유" },
                ],
                SideNote = new SideNote
                {
                    Kind = SideNoteKind.ExamConflict,
                    Title = "마감 10.22가 시험 기간과 겹칩니다",
                    Body = "10.17(금)까지 끝내면 시험 주간에 신경 쓸 일이 없습니다.",
                },
                PdfFile = "토익응시료지원_2차.pdf · 1p",
                PdfNote = "캡처 이미지로 등록된 공고입니다. OCR로 텍스트를 추출했습니다.",
                ChangeHistory = "사용자가 10.07 직접 등록 · 출처 1곳",
                CtaText = "응시확인서 업로드",
            },

            new Notice
            {
                Id = "abroad",
                Title = "해외교류 프로그램 참가자 모집",
                ListNote = "영문 성적증명서 3영업일 소요",
                SourceText = "국제교류원 RSS",
                Source = NoticeSource.CampusOffice,
                SourceShort = "국제교류원",
                Deadline = new DateTime(2025, 10, 23, 17, 0, 0),
                AmountText = "항공료 · 체재비 일부 지원",
                Requirements =
                [
                    new Requirement
                    {
                        Label = "총 취득 학점 60학점 이상",
                        Evidence = "공고문 2p",
                        Field = ProfileField.TotalCredits,
                        Comparison = Comparison.AtLeast,
                        Threshold = 60,
                    },
                    Gpa("평점 3.0 이상", 3.0, "공고문 2p"),
                    new Requirement
                    {
                        Label = "어학 성적 제출 가능",
                        Evidence = "공고문 2p · 붙임1",
                        Comparison = Comparison.IsTrue,
                        ProvidedValue = 1,               // 사용자가 제출 예정으로 표시한 항목
                        ProvidedDisplay = "제출 예정",
                    },
                    Enrolled("공고문 2p"),
                    new Requirement
                    {
                        Label = "파견 학기 시간표 충돌",
                        Evidence = "공고문 3p",
                        IsFlag = true,
                        FlagConflicts = false,
                        ProvidedDisplay = "다음 학기 — 무관",
                    },
                ],
                Documents =
                [
                    new RequiredDocument { Name = "영문 성적증명서", Issuer = "학사지원팀", LeadBusinessDays = 3, LeadText = "3영업일", State = DocumentState.ActionNeeded, StateText = "오늘 신청" },
                    new RequiredDocument { Name = "수학계획서", Issuer = "직접 작성", LeadBusinessDays = 2, LeadText = "2일 예상", State = DocumentState.ActionNeeded, StateText = "미작성" },
                    new RequiredDocument { Name = "어학 성적표", Issuer = "토익 사이트", LeadBusinessDays = 0, LeadText = "즉시 출력", State = DocumentState.Held, StateText = "보유" },
                ],
                SideNote = new SideNote
                {
                    Kind = SideNoteKind.ExamConflict,
                    Title = "마감이 중간고사 주간(10.20–10.24) 안에 있습니다",
                    Body = "시험 전 주에 서류를 끝내두는 편이 안전합니다.",
                },
                PdfFile = "2025_동계_해외교류_모집.pdf · 6p",
                PdfNote = "붙임1에서 어학 성적 기준과 제출 서류 목록을 추가로 추출했습니다.",
                ChangeHistory = "변경 이력 없음 · 10.03 최초 등록",
                CtaText = "증명서 신청하기",
            },

            new Notice
            {
                Id = "lowinc",
                Title = "저소득층 근로장학",
                ListNote = "소득 6분위 이하 요건 — 내 8분위",
                SourceText = "한국장학재단 API",
                Source = NoticeSource.ScholarshipFoundationApi,
                SourceShort = "장학재단 API",
                Deadline = new DateTime(2025, 10, 24, 18, 0, 0),
                AmountText = "시급 11,000원 · 주 15시간",
                Requirements =
                [
                    new Requirement
                    {
                        Label = "소득분위 6분위 이하",
                        Evidence = "공고문 1p · 필수 요건",
                        Field = ProfileField.IncomeDecile,
                        Comparison = Comparison.AtMost,
                        Threshold = 6,
                    },
                    Gpa("직전 학기 성적 70/100 이상", 3.15, "공고문 1p"),
                    Enrolled("공고문 1p"),
                ],
                Documents =
                [
                    new RequiredDocument { Name = "소득분위 확인서", Issuer = "한국장학재단", LeadBusinessDays = 1, LeadText = "1영업일", State = DocumentState.NotApplicable, StateText = "해당 없음" },
                ],
                SideNote = new SideNote
                {
                    Kind = SideNoteKind.NotEligible,
                    Title = "소득분위 6분위 이하 요건에서 미달",
                    Body = "현재 8분위입니다. 예외 조항은 발견되지 않았습니다. 재산정 결과가 반영되면 자동으로 다시 판정합니다.",
                },
                PdfFile = "2025_저소득층근로장학_공고.pdf · 3p",
                PdfNote = "1페이지 \"소득 6분위 이하\" 문장을 필수 요건으로 추출했습니다. 예외 조항 없음.",
                ChangeHistory = "변경 이력 없음 · 10.02 최초 등록",
                CtaText = "유사 공고 찾아보기",
                CtaFollowsJudgment = true,
            },

            new Notice
            {
                Id = "work2",
                Title = "2학기 근로장학생 추가 모집",
                ListNote = "주 15시간 이내 근로 가능 여부 확인",
                SourceText = "학생지원팀 RSS",
                Source = NoticeSource.SchoolRss,
                SourceShort = "학생지원팀",
                Deadline = new DateTime(2025, 10, 27, 18, 0, 0),
                AmountText = "시급 11,000원 · 주 12시간",
                Requirements =
                [
                    Enrolled("공고문 1p"),
                    Gpa("직전 학기 성적 70/100 이상", 3.15, "공고문 1p"),
                    new Requirement
                    {
                        Label = "주 15시간 이내 근로 가능",
                        Evidence = "공고문 2p",
                        Comparison = Comparison.IsTrue,
                        ProvidedValue = 1,
                        ProvidedDisplay = "시간표 기준 가능",
                    },
                ],
                Documents =
                [
                    new RequiredDocument { Name = "재학증명서", Issuer = "학교 포털", LeadBusinessDays = 0, LeadText = "즉시 발급", State = DocumentState.Held, StateText = "보유" },
                ],
                PdfFile = "2025-2_근로장학_추가모집.pdf · 2p",
                PdfNote = "2페이지 근로 시간 조항을 시간표와 대조했습니다.",
                ChangeHistory = "변경 이력 없음 · 10.10 최초 등록",
                CtaText = "신청서 작성하기",
            },

            new Notice
            {
                Id = "startup",
                Title = "창업동아리 활동비 지원",
                ListNote = "팀 구성 3인 이상 요건 불확실",
                SourceText = "사용자 직접 등록 (URL)",
                Source = NoticeSource.SelfRegistered,
                SourceShort = "직접 등록",
                Deadline = new DateTime(2025, 10, 31, 18, 0, 0),
                AmountText = "팀당 2,000,000원",
                Requirements =
                [
                    Enrolled("공고문 1p"),
                    new Requirement
                    {
                        Label = "총 취득 학점 30학점 이상",
                        Evidence = "공고문 1p",
                        Field = ProfileField.TotalCredits,
                        Comparison = Comparison.AtLeast,
                        Threshold = 30,
                    },
                    new Requirement
                    {
                        Label = "팀 구성 3인 이상",
                        Evidence = "공고문 2p · 붙임1 서식",
                        Comparison = Comparison.AtLeast,
                        Threshold = 3,
                        MissingFieldLabel = "창업팀 구성원 수",
                    },
                ],
                Documents =
                [
                    new RequiredDocument { Name = "사업계획서", Issuer = "직접 작성", LeadBusinessDays = 3, LeadText = "3일 예상", State = DocumentState.ActionNeeded, StateText = "미작성" },
                    new RequiredDocument { Name = "팀원 재학증명서", Issuer = "학교 포털", LeadBusinessDays = 0, LeadText = "즉시 발급", State = DocumentState.ActionNeeded, StateText = "미제출" },
                ],
                SideNote = new SideNote
                {
                    Kind = SideNoteKind.MissingInput,
                    Title = "팀 구성원 수를 입력해야 판정할 수 있습니다",
                    Body = "직접 등록한 공고라 학사 시스템에서 가져올 값이 없습니다. 팀원 수를 입력하면 즉시 판정합니다.",
                },
                PdfFile = "창업동아리_활동비지원.pdf · 3p",
                PdfNote = "사용자가 등록한 URL 을 크롤링해 본문을 추출했습니다.",
                ChangeHistory = "사용자가 10.05 직접 등록 · 출처 1곳",
                CtaText = "팀 구성원 수 입력",
                CtaFollowsJudgment = true,
            },

            new Notice
            {
                Id = "winterwork",
                Title = "국가근로 동계 집중근로",
                ListNote = "방학 중 근로 · 시간표 무관",
                SourceText = "한국장학재단 API",
                Source = NoticeSource.ScholarshipFoundationApi,
                SourceShort = "장학재단 API",
                Deadline = new DateTime(2025, 11, 5, 18, 0, 0),
                AmountText = "시급 11,000원 · 총 160시간",
                Requirements =
                [
                    Enrolled("공고문 1p"),
                    new Requirement
                    {
                        Label = "소득분위 8분위 이하",
                        Evidence = "공고문 1p",
                        Field = ProfileField.IncomeDecile,
                        Comparison = Comparison.AtMost,
                        Threshold = 8,
                    },
                    Gpa("직전 학기 성적 70/100 이상", 3.15, "공고문 1p"),
                    new Requirement
                    {
                        Label = "방학 중 근로 가능",
                        Evidence = "공고문 2p",
                        Comparison = Comparison.IsTrue,
                        ProvidedValue = 1,
                        ProvidedDisplay = "가능",
                    },
                ],
                Documents =
                [
                    new RequiredDocument { Name = "근로 신청서", Issuer = "직접 작성", LeadBusinessDays = 1, LeadText = "1일 예상", State = DocumentState.ActionNeeded, StateText = "미작성" },
                ],
                PdfFile = "2025_동계_국가근로.pdf · 4p",
                PdfNote = "1페이지 지원 자격 표에서 요건 4건을 추출했습니다.",
                ChangeHistory = "변경 이력 없음 · 10.11 최초 등록",
                CtaText = "신청서 작성하기",
            },

            new Notice
            {
                Id = "grad",
                Title = "대학원 진학 예정자 연구장학",
                ListNote = "졸업 예정자 한정 — 3학년",
                SourceText = "대학원 행정실 공지",
                Source = NoticeSource.CampusOffice,
                SourceShort = "대학원 행정실",
                Deadline = new DateTime(2025, 11, 7, 18, 0, 0),
                AmountText = "연구비 1,000,000원",
                Requirements =
                [
                    new Requirement
                    {
                        Label = "졸업 예정자 (4학년)",
                        Evidence = "공고문 1p · 필수 요건",
                        Field = ProfileField.GradeYear,
                        Comparison = Comparison.AtLeast,
                        Threshold = 4,
                        ProvidedDisplay = "3학년",
                    },
                    Enrolled("공고문 1p"),
                    Gpa("평점 3.5 이상", 3.5, "공고문 2p"),
                ],
                Documents =
                [
                    new RequiredDocument { Name = "지도교수 추천서", Issuer = "지도교수", LeadBusinessDays = 5, LeadText = "5영업일", State = DocumentState.NotApplicable, StateText = "해당 없음" },
                ],
                SideNote = new SideNote
                {
                    Kind = SideNoteKind.NotEligible,
                    Title = "졸업 예정자 요건에서 미달",
                    Body = "현재 3학년 2학기입니다. 4학년이 되면 자동으로 다시 판정합니다.",
                },
                PdfFile = "연구장학_모집공고.pdf · 2p",
                PdfNote = "1페이지 필수 요건에서 학년 조건을 추출했습니다.",
                ChangeHistory = "변경 이력 없음 · 10.04 최초 등록",
                CtaText = "유사 공고 찾아보기",
                CtaFollowsJudgment = true,
            },
        ];
    }

    public static string SourceLabel(NoticeSource s) => s switch
    {
        NoticeSource.SchoolRss => "학교 공지 RSS",
        NoticeSource.ScholarshipFoundationApi => "한국장학재단 API",
        NoticeSource.PublicData => "공공데이터",
        NoticeSource.SelfRegistered => "직접 등록",
        _ => "교내 부서",
    };

    public string LastCollectedText => "마지막 수집 오늘 08:10";

    // ── 혜택 · 프로그램 ────────────────────────────────────────────────────
    public IReadOnlyList<BenefitProgram> Benefits { get; } =
    [
        new() { Department = "SW중심대학사업단", Title = "SW 역량강화 부트캠프", Category = "졸업요건 인정",
                Description = "졸업 요건 비교과 8시간이 한 번에 채워집니다. 미충족 항목 12시간 중 8시간 해당.",
                TimeText = "목 18:00–20:00", CreditText = "8시간 인정", State = BenefitState.TopPick },
        new() { Department = "국제교류원", Title = "외국인 학생 멘토링", Category = "상담·복지",
                Description = "학기 중 주 1회 활동. 활동비 30만원 지급.",
                TimeText = "금 16:00–18:00", CreditText = "4시간 인정", State = BenefitState.Open },
        new() { Department = "교양교육원", Title = "토익 응시료 지원 (2차)", Category = "어학·자격",
                Description = "응시확인서 제출 시 5만원 환급. 학기당 2회까지.",
                TimeText = "상시", CreditText = "—", State = BenefitState.Open },
        new() { Department = "학생상담센터", Title = "진로 심층 상담 (4회기)", Category = "상담·복지",
                Description = "회기당 50분. 학칙상 비교과 인정 대상이나 공고로 올라오지 않는 항목.",
                TimeText = "예약제", CreditText = "2시간 인정", State = BenefitState.RuleFound },
        new() { Department = "도서관", Title = "논문 검색·인용 워크숍", Category = "졸업요건 인정",
                Description = "RISS·DBpia 실습 포함.",
                TimeText = "금 15:00–17:00", CreditText = "2시간 인정", State = BenefitState.Open },
        new() { Department = "경력개발센터", Title = "모의 면접 클리닉", Category = "상담·복지",
                Description = "현직자 1:1 피드백. 정원 20명.",
                TimeText = "수 14:00–16:00", CreditText = "3시간 인정", State = BenefitState.NeedsCheck },
        new() { Department = "공학교육혁신센터", Title = "캡스톤 예비 워크숍", Category = "졸업요건 인정",
                Description = "졸업 작품 주제 선정과 팀 매칭을 함께 진행합니다.",
                TimeText = "화 18:00–20:00", CreditText = "4시간 인정", State = BenefitState.Open },
        new() { Department = "어학교육원", Title = "OPIc 집중 대비반", Category = "어학·자격",
                Description = "8주 과정. 수강료 50% 지원.",
                TimeText = "월 19:00–21:00", CreditText = "6시간 인정", State = BenefitState.Open },
        new() { Department = "교양교육원", Title = "한국사능력검정 응시료 지원", Category = "어학·자격",
                Description = "3급 이상 취득 시 응시료 전액 환급.",
                TimeText = "상시", CreditText = "—", State = BenefitState.Open },
        new() { Department = "학생상담센터", Title = "심리검사 무료 이용", Category = "상담·복지",
                Description = "MBTI · 홀랜드 검사 포함. 학기당 1회.",
                TimeText = "예약제", CreditText = "1시간 인정", State = BenefitState.Open },
        new() { Department = "SW중심대학사업단", Title = "오픈소스 기여 챌린지", Category = "졸업요건 인정",
                Description = "8주간 오픈소스 PR 3건 이상 제출.",
                TimeText = "비대면", CreditText = "6시간 인정", State = BenefitState.NeedsCheck },
    ];

    public IReadOnlyList<ExcludedProgram> ExcludedPrograms { get; } =
    [
        new("창업 아이디어 워크숍", "월 10:00 자료구조"),
        new("글로벌 역량강화 특강", "화 12:00 데이터베이스"),
        new("통계 프로그래밍 기초", "월 13:00 컴퓨터네트워크"),
        new("리더십 캠프 (1박)", "목 14:00 소프트웨어공학"),
    ];

    // ── 시간표 ────────────────────────────────────────────────────────────
    public IReadOnlyList<ClassBlock> Classes { get; } =
    [
        new() { Name = "자료구조",       Room = "공학관 401", DayIndex = 0, StartHour = 10, EndHour = 12, ToneIndex = 0 },
        new() { Name = "운영체제",       Room = "공학관 305", DayIndex = 2, StartHour = 10, EndHour = 12, ToneIndex = 1 },
        new() { Name = "컴퓨터네트워크", Room = "IT관 208",   DayIndex = 0, StartHour = 13, EndHour = 15, ToneIndex = 2 },
        new() { Name = "데이터베이스",   Room = "IT관 311",   DayIndex = 1, StartHour = 12, EndHour = 14, ToneIndex = 3 },
        new() { Name = "소프트웨어공학", Room = "공학관 402", DayIndex = 3, StartHour = 14, EndHour = 17, ToneIndex = 4 },
        new() { Name = "교양: 현대윤리", Room = "인문관 112", DayIndex = 1, StartHour = 15, EndHour = 17, ToneIndex = 5 },
    ];

    // ── 학사 일정 (공고 마감은 판정 엔진이 따로 만들어 얹는다) ──────────────
    public IReadOnlyList<CalendarEvent> AcademicEvents { get; } =
    [
        new("근로장학 공고", CalendarEventKind.Academic, new DateTime(2025, 10, 8)),
        new("수강 정정", CalendarEventKind.Academic, new DateTime(2025, 10, 31)),
    ];

    public (DateTime Start, DateTime End) ExamPeriod { get; } =
        (new DateTime(2025, 10, 20), new DateTime(2025, 10, 24));

    // ── 알림 ──────────────────────────────────────────────────────────────
    public IReadOnlyList<AlertItem> Alerts { get; } =
    [
        new() { Kind = AlertKind.Start, IsUnread = true, TimeText = "08:10",
                Title = "해외교류 서류, 오늘 신청해야 합니다",
                Body = "영문 성적증명서 발급 3영업일 · 마감 10.23", NoticeId = "abroad" },
        new() { Kind = AlertKind.Deadline, IsUnread = true, TimeText = "08:10",
                Title = "국가장학금 II유형 D-3",
                Body = "요건 4/4 충족 · 신청서만 작성하면 됩니다", NoticeId = "nsf2" },
        new() { Kind = AlertKind.New, IsUnread = true, TimeText = "어제",
                Title = "2학기 근로장학생 추가 모집",
                Body = "학생지원팀 RSS · 판정 결과 지원 가능", NoticeId = "work2" },
        new() { Kind = AlertKind.Changed, IsUnread = false, TimeText = "10.09",
                Title = "[수정] 국가장학금 마감이 변경되었습니다",
                Body = "10.14 → 10.16 18:00 · 변경 부분 보기", NoticeId = "nsf2" },
        new() { Kind = AlertKind.Warning, IsUnread = false, TimeText = "10.09",
                Title = "중복 수혜 충돌 감지",
                Body = "국가장학금 II유형 ↔ 교내 성적우수 장학", NoticeId = "nsf2" },
        new() { Kind = AlertKind.New, IsUnread = false, TimeText = "10.08",
                Title = "SW 역량강화 부트캠프 (졸업요건 8시간)",
                Body = "시간표 충돌 없음 · 미충족 요건에 해당", TargetScreen = ScreenKind.Benefits },
    ];

    public IReadOnlyList<NotificationSetting> NotificationSettings { get; } =
    [
        new("신규 공고 (지원 가능만)", true),
        new("마감 3일 전", true),
        new("서류 착수 시점", true),
        new("자격 미달 공고도 알림", false),
    ];

    // ── 프로필 우측 카드 ──────────────────────────────────────────────────
    public IReadOnlyList<KeyValueRow> PrivacyRows { get; } =
    [
        new("학번 · 성적표 원본", "기기에만", "EligibleFg"),
        new("소득분위", "판정 시 전송", "CheckFg"),
        new("학년 · 학과 · 평점", "서버 저장", "TextMuted2"),
    ];

    public IReadOnlyList<KeyValueRow> BenefitHistoryRows { get; } =
    [
        new("2025-1 국가장학금 I유형", "수혜 260만"),
        new("2024-2 교내 성적우수", "수혜 80만"),
        new("2024-1 근로장학", "미선정"),
    ];

    public ToastMessage SampleToast { get; } = new(
        "오늘 서류를 신청해야 마감을 맞출 수 있습니다",
        "해외교류 프로그램 · 영문 성적증명서 발급 3영업일 · 마감 10.23",
        "공고 열기", "나중에", "abroad");
}
