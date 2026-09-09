# CLAUDE.md — Campus Signal 작업 지침

이 파일은 이 저장소에서 일하는 Claude 를 위한 것이다. 사람이 읽는 소개는 `README.md` 에 있다.

---

## 0. 지금 가장 먼저 할 일

**이 코드는 한 번도 컴파일된 적이 없다.**

초기 구현은 Linux 컨테이너에서 이뤄졌다. WPF 는 Windows 전용이라 빌드가 불가능했고,
그 환경은 .NET SDK 다운로드도 네트워크 정책에 막혀 있었다. 대신 정적 검사만 돌렸다 —
XAML 전체 XML 파싱, `StaticResource` 키 전량 해석 대조, XAML 바인딩 경로와 뷰모델
멤버 대조, C# 괄호 균형. 그 과정에서 몇 건을 잡아 고쳤지만 **컴파일러가 잡을 오류는
남아 있을 수 있다.**

너는 Windows 에서 돌고 있으니 첫 작업은 이것이다.

```
dotnet build CampusSignal.sln
```

오류가 나오면 고친다. 특히 의심스러운 곳:

- `Infrastructure/Tracking.cs` — `InlineUIContainer` 로 자간을 만드는 부분. 베이스라인이
  틀어지거나 줄 높이가 늘어날 수 있다. 빌드가 되면 **눈으로 확인할 것.**
- `MainWindow.xaml` 의 `WindowChrome` — 최대화 시 작업 표시줄을 덮는 문제를
  `Window.BorderThickness` 로 우회해 뒀다. 실제로 동작하는지 확인이 필요하다.
- `Views/TimetableView.xaml` — `ItemsControl` 의 `ItemContainerStyle` 로 `Grid.Row`/
  `RowSpan`/`Column` 을 바인딩해 수업 블록을 얹는다. 컨테이너가 `ContentPresenter` 라는
  전제다.
- `Themes/Controls.xaml` 의 암시적 `ScrollBar` 스타일 — 가로 스크롤바 방향 반전 처리.

빌드가 통과하면 `dotnet run --project src/CampusSignal` 으로 8개 화면을 모두 눌러보고
`docs/design/CampusSignal.dc.html` 을 브라우저로 나란히 띄워 대조한다.

---

## 1. 이 앱이 하는 일

흩어진 장학·공고·비교과 정보를 모아, 사용자의 학사 프로필과 대조해
**지원 가능 / 확인 필요 / 자격 미달**을 판정하고 "지금 해야 할 일"로 제시한다.

화면 전체가 세 원칙 위에 서 있다. **기능을 더하거나 고칠 때 이걸 먼저 확인한다.**

1. **목록이 아니라 판정.** 공고를 나열하지 않는다. 판정 결과를 먼저 말한다.
2. **마감이 아니라 착수일.** 서류 발급 소요일을 역산해 "오늘 해야 하는 일"을 만든다.
3. **가장 급한 한 건을 크게.** 대시보드 히어로는 언제나 단 하나다. 둘로 늘리지 않는다.

---

## 2. 디자인 원본

`docs/design/` 이 디자인 핸드오프 번들 원본이다.

| 파일 | 무엇 |
|---|---|
| `README.md` | **스펙 원본.** 색·타이포·간격·문구가 전부 확정값으로 적혀 있다 |
| `CampusSignal.dc.html` | 채택된 최종 디자인. 8개 화면 전부가 이 한 파일에 있다 |
| `Dashboard C - Signal.dc.html` | 채택안의 대시보드 원본 |
| `Dashboard A`, `Dashboard B` | **채택되지 않은 탐색안. 구현 대상이 아니다** |

수치가 헷갈리면 `docs/design/README.md` 를 본다. 코드보다 그게 기준이다.
WPF 의 DIP 는 CSS px 와 1:1 이므로 **문서의 px 값을 그대로 XAML 수치로 옮기면 된다.**

`.dc.html` 은 레퍼런스지 옮겨 쓸 코드가 아니다. HTML/CSS 구조를 번역하지 말고
WPF 관용 패턴(`Grid`, `ItemsControl` + `DataTemplate`, `Style`, MVVM 바인딩)으로 다시 짠다.

---

## 3. 구조

```
CampusSignal.sln
docs/design/                     디자인 핸드오프 원본
src/CampusSignal/
  App.xaml(.cs)                  리소스 병합 + 뷰모델→뷰 DataTemplate 매핑, 진입점
  MainWindow.xaml(.cs)           셸 — 타이틀바 · 헤더 · 내비 · 콘텐츠 · 토스트
  Themes/
    Palette.xaml                 색 토큰 전부
    Typography.xaml              폰트·텍스트 스타일
    Controls.xaml                버튼·카드·행·배지·입력·토글 스타일
  Models/                        도메인. 로직 없음
  Services/
    JudgmentEngine.cs            ★ 자격 판정 + 착수일 역산
    NoticeCatalog.cs             전체 공고의 판정 결과 집합. 프로필 바뀌면 Refresh()
    SampleDataStore.cs           표본 데이터 (수집기 대체물)
    BusinessDays.cs              영업일 가감
    IClock.cs                    SystemClock / FixedClock
    AppServices.cs               조립 지점 (DI 컨테이너 대신)
  ViewModels/
    ShellViewModel.cs            화면 전환 · 헤더 문구 · 토스트 소유. IAppNavigator 구현
    ScreenViewModel.cs           8개 화면 VM 의 베이스 (Kind/Title/Subtitle/Activate)
    ItemViewModels.cs            행·카드 단위 VM 전부 (한 파일에 모아둠)
    <Screen>ViewModel.cs         화면별
  Views/
    <Screen>View.xaml(.cs)       화면별
    ToastView.xaml               인앱 토스트
  Infrastructure/
    ObservableObject.cs          INotifyPropertyChanged 기반
    RelayCommand.cs              ICommand
    Palette.cs                   판정→브러시 매핑. 리소스 키 조회
    Tracking.cs                  letter-spacing 첨부 속성
    Emphasis.cs                  <em> 마크업 → 인라인 첨부 속성
    Converters.cs                Bool/Null/Zero→Visibility, 리소스키→Brush
  Fonts/README.md                폰트 넣는 법 (바이너리는 미포함)
```

**단일 창 / 뷰 스위칭** 구조다. `ShellViewModel.Current` 에 화면 VM 이 들어가고
`App.xaml` 의 `DataTemplate` 이 그에 맞는 View 를 고른다. 화면 VM 은 `IAppNavigator`
하나만 알고, 셸이 그걸 구현한다.

---

## 4. 반드시 지킬 것

**색은 `Themes/Palette.xaml` 에만 있다.**
XAML 이나 C# 에 hex 리터럴을 쓰지 않는다. 새 색이 필요하면 화면이 아니라 팔레트에 먼저
추가하고 키로 참조한다. (예외: `Palette.cs` 의 시간표 6색 순환 배열. 순환값이라 배열이 맞다.)

**판정 결과를 저장하지 않는다.**
`Notice` 에 "지원 가능" 같은 필드를 두지 않는다. 요건과 서류만 담고 `JudgmentEngine` 이
계산한다. 화면은 `NoticeCatalog` 이 들고 있는 `NoticeJudgment` 를 읽는다. 프로필이
바뀌면 `Catalog.Refresh()` 한 번으로 앱 전체가 다시 계산된다 — 이 경로를 깨지 않는다.

**요건에는 항상 근거를 붙인다.**
`Requirement.Evidence` 는 "공고문 어느 문장에서 뽑았는지"다. 상세 화면이 이걸 항상 함께
보여주는 것이 이 앱의 신뢰 장치다. 근거 없는 요건을 만들지 않는다.

**호버에 트랜지션을 넣지 않는다.**
데스크톱 앱 반응성 우선. 디자인이 명시적으로 요구한 것이다.
애니메이션은 토스트 등장(220ms ease-out) 하나뿐이다.

**주석과 UI 문구는 한국어로 쓴다.** 기존 코드가 그렇다.

**주석은 "무엇"이 아니라 "왜"를 적는다.** 기존 주석의 밀도와 어조를 따른다.

---

## 5. 판정 엔진 (`Services/JudgmentEngine.cs`)

앱의 핵심. 두 가지를 한다.

### 자격 판정

공고의 요건 목록을 프로필과 대조해 요건마다 결과를 낸다.

| 결과 | 뜻 | 마크 |
|---|---|---|
| `Met` | 충족 | ✓ |
| `Unknown` | 대조할 값이 없음 | ? |
| `Failed` | 미충족 | ✕ |
| `Flagged` | 판정을 막지 않지만 알려야 함 (중복 수혜·시간표 충돌) | ! |

- 하나라도 `Failed` → **자격 미달**
- `Failed` 없고 `Unknown` 있음 → **확인 필요** (`BlockingFields` 에 어떤 항목이 비어서인지 담아 반환)
- 전부 `Met` → **지원 가능**

값의 우선순위는 `Requirement.ProvidedValue`(출처가 직접 내려준 값) → 프로필 → 없으면 `Unknown`.
한국장학재단 API 처럼 학적 정보를 함께 주는 출처를 표현하려고 둔 것이다.

### 착수일 역산

```
착수일 = 마감일 − max(미보유 서류 발급 영업일) − 작성 여유(기본 1영업일)
```

`WritingBufferDays` 로 조절한다. 착수일이 오늘 이하면 할 일 목록에서 D-day 대신
**"착수"** 배지(`#3F6B8A`)로 표시한다. 착수 구간이 시험 기간(`ExamPeriod`, 기본 10.20–24)과
겹치면 충돌 경고를 만든다.

역산 결론 문장(`LeadNote`)은 엔진이 생성한다. `<em>` 로 감싼 부분이 다크 카드에서
라임색으로 그려진다 (`Infrastructure/Emphasis.cs`).

---

## 6. WPF 에 없어서 직접 채운 것

**`Infrastructure/Tracking.cs` — letter-spacing**
WPF `TextBlock` 에는 CSS 의 `letter-spacing` 이 없다. 디자인은 Mono 라벨에 +0.08~0.18em
트래킹을 요구한다. 글자 사이에 폭이 정확한 빈 `InlineUIContainer` 를 끼워 재현한다.

```xml
<TextBlock FontSize="10.5" inf:Tracking.Em="0.14" inf:Tracking.Text="EXAM CONFLICT"/>
```

`TextBlock.Text` 와 `Inlines` 는 함께 못 쓰므로 텍스트는 반드시 `Tracking.Text` 로 넣는다.
XAML 속성 순서는 `Em` → `Text` 로 쓴다.

**`Infrastructure/Emphasis.cs` — 부분 강조**
문장 일부만 다른 색·굵기로 그려야 하는 곳을 데이터에서 표현하려고 `<em>…</em>` 마크업을
인라인으로 푼다. 순서는 `Brush` → `Weight` → `Markup`.

```xml
<TextBlock inf:Emphasis.Brush="{StaticResource Lime}"
           inf:Emphasis.Weight="SemiBold"
           inf:Emphasis.Markup="{Binding LeadNote}"/>
```

---

## 7. 표본 데이터와 프로토타입의 차이

`Services/SampleDataStore.cs` 는 수집기가 붙기 전까지 앱을 굴리는 표본이다.
공고 9건. **판정 결과는 여기 없다** — 요건과 서류만 있고 엔진이 계산한다.

그래서 화면의 숫자가 `.dc.html` 프로토타입과 다르다. 프로토타입의 숫자(공고 42건,
요건 4/4 등)는 레이아웃 검증용이라 서로 아귀가 맞지 않는 곳이 있었다. **이건 버그가
아니다. 되돌리지 마라.** 판단이 필요했던 지점:

- **건수** — 필터·헤더의 모든 카운트를 실제 컬렉션에서 파생시켰다.
- **소득분위 8분위** — 프로토타입은 프로필에 "6분위", 저소득층 근로장학에는 "8분위 미달"로
  적혀 모순이었다. 공고 쪽 문구를 살리려고 프로필 값을 8분위로 뒀다.
- **국가장학금의 이수 학점** — "이수 학점이 비어 판정이 보류된다"와 "국가장학금은 요건
  4/4 충족"이 동시에 성립해야 했다. 장학재단 API 가 학적 정보를 함께 내려준다는 사실을
  `ProvidedValue` 로 모델링해 두 서술을 다 살렸다.
- **역산 문장** — 핸드오프의 공식대로 생성한다. 프로토타입의 손으로 쓴 날짜와 하루 이틀 차이 난다.

`App.xaml.cs` 에서 `FixedClock(2025-10-13)` 으로 시계를 고정해 표본이 핸드오프 문서와
같은 날짜로 읽힌다. **실제 수집기를 붙일 때 `SystemClock` 으로 바꾼다.**

---

## 8. 아직 자리표시자인 것

| 항목 | 현재 | 해야 할 것 |
|---|---|---|
| 공고 원문 뷰어 | 사선 해치 자리표시자 | `PdfiumViewer` 등을 얹고 추출 근거 문장에 하이라이트 |
| 시간표 파일 | 드롭존은 동작(`AllowDrop` + 피드백) | xlsx 파서 · 캡처 이미지 OCR |
| 성적표 업로드 | 파싱 결과를 시뮬레이션(18학점 채우고 재판정) | 실제 PDF/이미지 파싱 |
| 시스템 토스트 | 인앱 토스트만 | 백그라운드일 때 `Microsoft.Toolkit.Uwp.Notifications`. **시스템 토스트는 OS 스타일을 따르므로 디자인 스펙은 인앱 토스트에만 적용된다** |
| 폰트 | 폴백 체인 | Pretendard · IBM Plex Mono 임베드 (`src/CampusSignal/Fonts/README.md`) |
| 수집기 | 없음 | RSS · 공공 API · 직접 등록, 공고 병합, 마감·금액 변경 감지 |
| 프로필 편집 | 읽기 전용 카드 | 클릭 시 `TextBox`/`ComboBox` 인라인 편집 |

핸드오프가 "아직 정의되지 않았다"고 못 박은 상태들도 그대로 비어 있다 —
로딩(수집 중 스켈레톤), 빈 상태(공고 목록만 있음), 오류(수집 실패·판정 불가), 오프라인.

---

## 9. Git

- 개발 브랜치: **`claude/wpf-app-design-implementation-w1glgm`**
- 기본 브랜치: `main`
- 작업을 이 브랜치에 쌓고, 정리되면 `main` 으로 PR 을 올린다
- PR 은 사용자가 요청할 때만 만든다

---

## 10. 잘 모르겠으면

1. `docs/design/README.md` 의 해당 화면 절을 읽는다. 수치와 문구가 확정값으로 있다.
2. `docs/design/CampusSignal.dc.html` 을 브라우저로 열어 실제 동작을 본다.
3. 그래도 애매하면 사용자에게 묻는다. 디자인을 임의로 해석해 바꾸지 않는다.
