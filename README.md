# Campus Signal

흩어진 장학·공고·비교과 정보를 한곳에 모으고, 사용자의 학사 프로필과 대조해
**지원 가능 / 확인 필요 / 자격 미달**을 판정한 뒤 "지금 해야 할 일"로 제시하는
Windows 데스크톱 앱. **WPF (.NET 8)**.

디자인 핸드오프의 8개 화면을 XAML/MVVM 으로 구현한 것이다.
핸드오프 원본은 `docs/design/` 에 있고, 그중 `docs/design/README.md` 가 스펙 원본이다.
작업 지침은 `CLAUDE.md` 를 참고한다.

## 세 가지 원칙

화면 전체가 이 위에 서 있다.

1. **목록이 아니라 판정.** 공고를 나열하지 않고 판정 결과를 먼저 말한다.
2. **마감이 아니라 착수일.** 서류 발급 소요일을 역산해 "오늘 해야 하는 일"을 만든다.
3. **가장 급한 한 건을 크게.** 대시보드 히어로는 항상 단 하나의 최우선 항목이다.

## 빌드 · 실행

**Windows + .NET 8 SDK 가 필요하다** (WPF 는 Windows 전용이다).

```
dotnet build CampusSignal.sln
dotnet run --project src/CampusSignal
```

Visual Studio 2022 / Rider 에서는 `CampusSignal.sln` 을 열면 된다.

## 화면

| 화면 | 뷰 | 뷰모델 |
|---|---|---|
| 대시보드 | `Views/DashboardView.xaml` | `DashboardViewModel` |
| 공고 목록 | `Views/NoticeListView.xaml` | `NoticeListViewModel` |
| 공고 상세 · 판정 결과 | `Views/NoticeDetailView.xaml` | `NoticeDetailViewModel` |
| 혜택 · 프로그램 | `Views/BenefitsView.xaml` | `BenefitsViewModel` |
| 캘린더 | `Views/CalendarView.xaml` | `CalendarViewModel` |
| 시간표 | `Views/TimetableView.xaml` | `TimetableViewModel` |
| 프로필 | `Views/ProfileView.xaml` | `ProfileViewModel` |
| 알림 | `Views/NotificationsView.xaml` | `NotificationsViewModel` |
| 인앱 토스트 | `Views/ToastView.xaml` | `ShellViewModel.Toast` |

상단 고정 헤더(다크) + 그 아래 스크롤 콘텐츠의 **단일 창 / 뷰 스위칭** 구조다.
`ShellViewModel` 이 화면 전환·헤더 문구·토스트를 소유하고, 뷰모델 → 뷰 매핑은
`App.xaml` 의 `DataTemplate` 이 한다. 창 크롬은 `WindowChrome` + Segoe MDL2 글리프다.

## 판정 엔진

앱의 핵심은 `Services/JudgmentEngine.cs` 두 가지 파생 로직이다.

**1. 자격 판정** — 공고의 요건 목록을 프로필과 대조해 요건마다 `Met / Unknown / Failed` 를 낸다.

- 하나라도 `Failed` → **자격 미달**
- `Failed` 없고 `Unknown` 있음 → **확인 필요** (어떤 프로필 항목이 비어서인지 함께 반환)
- 전부 `Met` → **지원 가능**

중복 수혜·시간표 충돌처럼 판정을 막지는 않지만 반드시 알려야 하는 사항은
`Flagged` 로 따로 다룬다.

요건마다 **공고문 어느 문장에서 뽑았는지(`Requirement.Evidence`)** 를 함께 들고 다니고,
상세 화면이 그것을 항상 같이 보여준다. 이게 이 앱의 신뢰 장치다.

**2. 착수일 역산** — `착수일 = 마감일 − max(미보유 서류 발급 영업일) − 작성 여유(기본 1영업일)`.
착수일이 오늘 이하면 할 일 목록에서 D-day 대신 **"착수"** 배지로 표시한다.
착수 구간이 시험 기간과 겹치면 충돌 경고를 만든다.

판정 결과는 어디에도 저장하지 않는다. `NoticeCatalog` 이 전체 공고를 프로필과 대조해 들고
있다가 프로필이 바뀌면 `Refresh()` 한 번으로 앱 전체가 다시 계산된다. 프로필 화면의
"성적표 업로드"를 눌러 보면 보류돼 있던 공고들의 판정이 즉시 바뀌는 걸 볼 수 있다.

## 프로젝트 구조

```
src/CampusSignal/
  Themes/           디자인 토큰 — Palette(색) · Typography(폰트) · Controls(스타일)
  Models/           도메인 — Notice, Requirement, RequiredDocument, StudentProfile …
  Services/         판정 엔진 · 영업일 계산 · 공고 카탈로그 · 표본 데이터
  ViewModels/       셸 + 8개 화면 + 행 단위 뷰모델
  Views/            화면 XAML
  Infrastructure/   MVVM 기반, 팔레트 조회, 컨버터, 트래킹/강조 첨부 속성
```

디자인 토큰은 전부 `Themes/Palette.xaml` 에 있다. **새 색이 필요하면 화면이 아니라 거기에 먼저 추가한다.**

### WPF 에 없는 두 가지를 채운 첨부 속성

- `Infrastructure/Tracking.cs` — WPF 에는 CSS 의 `letter-spacing` 이 없다.
  글자 사이에 폭이 정확한 빈 `InlineUIContainer` 를 끼워 Mono 라벨의 트래킹(+0.08~0.18em)을 재현한다.
  `<TextBlock ui:Tracking.Em="0.14" ui:Tracking.Text="EXAM CONFLICT"/>`
- `Infrastructure/Emphasis.cs` — 다크 카드의 라임색 강조처럼 문장 일부만 다르게 그려야 하는 곳을
  데이터에서 표현하려고 `<em>…</em>` 마크업을 인라인으로 푼다.

## 표본 데이터와 판정 결과에 대해

`Services/SampleDataStore.cs` 는 수집기(RSS · 공공 API · 직접 등록)가 붙기 전까지 앱을 굴리는
표본이다. **판정 결과는 여기 적지 않는다** — 요건과 서류만 넣고 엔진이 계산한다.

그래서 화면의 숫자는 HTML 프로토타입의 값과 다르다. 프로토타입의 숫자(공고 42건, 요건 4/4,
착수일 문구 등)는 레이아웃 검증용이라 서로 아귀가 맞지 않는 곳이 있었고, 여기서는 전부
실제 데이터에서 계산해 자체 정합성을 맞췄다. 눈에 띄는 차이는 다음과 같다.

- **건수** — 공고 9건, 필터·헤더의 모든 카운트는 실제 컬렉션에서 파생된다 (프로토타입은 42건 고정).
- **소득분위 8분위** — 프로토타입은 프로필에 "6분위", 저소득층 근로장학에는 "8분위 미달"이라고
  적혀 서로 모순이었다. 공고 쪽 문구를 살리려고 프로필 값을 8분위로 뒀다.
- **국가장학금의 이수 학점** — "직전 학기 이수 학점이 비어 있어 판정이 보류된다"는 이야기와
  "국가장학금은 요건 4/4 충족"이 동시에 성립해야 했다. 한국장학재단 API 가 학적 정보를 함께
  내려준다는 사실을 `Requirement.ProvidedValue`(출처 제공값이 프로필보다 우선)로 모델링해
  두 서술을 모두 살렸다.
- **역산 결론 문장** — 핸드오프에 적힌 공식(작성 여유 1영업일 포함)을 그대로 구현해 문장을
  생성한다. 프로토타입의 손으로 쓴 날짜와는 하루 이틀 차이가 난다.

`FixedClock(2025-10-13)` 으로 시계를 고정해 두어 표본이 핸드오프 문서와 같은 날짜로 읽힌다.
실제 수집기를 붙일 때 `App.xaml.cs` 에서 `SystemClock` 으로 바꾸면 된다.

## 아직 자리표시자인 것

- **공고 원문 뷰어** — 사선 해치 자리표시자다. `PdfiumViewer` 등을 얹고 추출 근거 문장에
  하이라이트를 그려야 한다.
- **시간표 파일 파싱** — 드롭존은 동작하지만(`AllowDrop` + 드래그 피드백) xlsx·이미지 OCR 파서는 없다.
- **성적표 파싱** — 업로드 버튼은 파싱 결과가 채울 값을 시뮬레이션해 판정 갱신 경로를 보여준다.
- **Windows 시스템 토스트** — 인앱 토스트만 구현했다. 앱이 백그라운드일 때는
  `Microsoft.Toolkit.Uwp.Notifications` 로 보내야 하며, 시스템 토스트는 OS 스타일을 따르므로
  디자인 스펙은 인앱 토스트에만 적용된다.
- **폰트** — Pretendard / IBM Plex Mono 바이너리는 포함하지 않았다.
  `src/CampusSignal/Fonts/README.md` 참고. 없으면 Segoe UI → Malgun Gothic / Consolas 로 폴백한다.
- **수집기** — RSS · 공공 API · 직접 등록과 공고 병합·변경 감지는 아직 없다.
