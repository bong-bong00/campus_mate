# Handoff: Campus Signal — 대학생 통합 공고·학사 관리 시스템 (WPF)

## Overview

흩어진 장학·공고·비교과 정보를 한곳에 모으고, 사용자의 학사 프로필과 대조해 **지원 가능 / 확인 필요 / 자격 미달**을 판정한 뒤 "지금 해야 할 일"로 제시하는 Windows 데스크톱 앱. 목표 환경은 **WPF (.NET 8)** 클라이언트다.

핵심 원칙 세 가지 — 화면 전체가 이 원칙 위에 서 있으니 구현 시에도 유지할 것:

1. **목록이 아니라 판정.** 공고를 나열하지 않고 판정 결과를 먼저 말한다.
2. **마감이 아니라 착수일.** 서류 발급 소요일을 역산해 "오늘 해야 하는 일"을 만든다.
3. **가장 급한 한 건을 크게.** 대시보드 상단 히어로는 항상 단 하나의 최우선 항목이다.

## About the Design Files

이 번들의 `.dc.html` 파일들은 **HTML로 만든 디자인 레퍼런스**다. 의도한 레이아웃·색·타이포·인터랙션을 보여주는 프로토타입이지 그대로 옮겨 쓸 프로덕션 코드가 아니다.

작업은 이 HTML 디자인을 **대상 코드베이스의 환경(WPF / XAML)에서 재현**하는 것이다. HTML/CSS 구조를 그대로 번역하지 말고, WPF의 관용 패턴(`Grid`, `StackPanel`, `ItemsControl` + `DataTemplate`, `Style`/`ResourceDictionary`, MVVM 바인딩)으로 다시 짜라. 아래 문서의 수치·색·문구는 정확하니 그것을 기준으로 삼는다.

HTML 프로토타입에서 하드코딩된 데이터(공고 9건, 알림 6건 등)는 **레이아웃 검증용 샘플**이다. 실제로는 수집기(RSS/API/직접 등록)와 판정 엔진의 출력으로 대체된다.

## Fidelity

**High-fidelity (hifi).** 색·타이포·간격·상태색이 모두 확정값이다. 픽셀 단위로 재현할 것.

단, WPF는 DIP(1/96 inch) 단위를 쓰고 HTML의 CSS px와 1:1 대응하므로 **아래 px 값은 그대로 XAML 수치로 옮기면 된다**. 창 기준 크기는 **1440 × 900**이며 최소 크기 1280 × 800을 권장한다. 좌우 사이드 컬럼은 고정 폭, 가운데 컬럼만 신축(`ColumnDefinition Width="*"`)으로 처리한다.

---

## Screens / Views

앱은 상단 고정 헤더(다크) + 그 아래 스크롤 콘텐츠 영역으로 구성된 **단일 창 / 뷰 스위칭** 구조다. 헤더는 모든 화면에서 동일하며 제목·부제만 바뀐다.

### 공통 셸 (Shell)

**타이틀바** — 높이 32px, 배경 `#12150F`, 좌우 패딩 26px.
- 좌: 텍스트 `CAMPUS SIGNAL`, IBM Plex Mono 11.5px, letter-spacing 0.18em, 색 `#8C9184`
- 우: 최소화/최대화/닫기. 프로토타입에서는 글리프(`—` `▫` `✕`) 11px `#6E7367`. **WPF에서는 커스텀 크롬(`WindowChrome`) + Segoe MDL2 Assets 글리프(`\uE921` `\uE922` `\uE8BB`)로 구현할 것.** 호버 시 배경 `#252A20`, 닫기만 `#C0392B`.

**헤더 본문** — 배경 동일 `#12150F`, 패딩 `14px 26px 18px`.
- 좌: 화면 제목 30px / weight 700 / letter-spacing −0.03em / 색 `#F2F1EA`, 그 아래 8px 띄우고 부제 13px `#9AA093`
- 우: 내비게이션 버튼 7개, 가로 flex, gap 8px, 세로 정렬은 baseline 기준 하단
  - 버튼: 패딩 `7px 13px`, 폰트 12.5px, radius 2px, `white-space: nowrap`
  - 비활성: 배경 투명, 테두리 1px `#3A3F34`, 글자 `#CFD4C6`
  - 활성: 배경 `#B9F27C`, 테두리 `#B9F27C`, 글자 `#12150F`
  - 호버: 테두리 `#B9F27C`
  - 라벨(순서 고정): `대시보드` / `공고 42` / `혜택` / `캘린더` / `시간표` / `프로필` / `알림 3` — 숫자는 실제 카운트 바인딩
  - **공고 상세 화면에서는 `공고 42` 탭이 활성 상태를 유지한다** (상세는 목록의 하위 뷰)

**콘텐츠 영역** — 배경 `#FFFFFF`, 세로 스크롤. 대부분 화면의 패딩은 `20px 26px 34px`(대시보드만 상단 0 — 히어로가 헤더에 맞닿음).

화면별 제목/부제:

| 화면 | 제목 | 부제 |
|---|---|---|
| 대시보드 | 지금 챙기지 않으면 놓칩니다 | 10월 13일 · 지원 가능 7 · 확인 필요 5 · 이번 주 마감 3 |
| 공고 목록 | 공고 42건 | 내 프로필과 대조해 판정한 결과입니다 · 마지막 수집 오늘 08:10 |
| 공고 상세 | 공고 상세 | 요건·서류·중복 수혜를 한 화면에서 확인합니다 |
| 혜택·프로그램 | 혜택 · 프로그램 11건 | 시간표와 겹치는 4건은 자동으로 제외했습니다 |
| 캘린더 | 2025년 10월 | 공고 마감 · 학사일정 · 시험 기간을 한 캘린더에 |
| 시간표 | 시간표 | 프로그램 충돌 판정과 마감 일정 계산에만 사용합니다 |
| 프로필 | 프로필 | 여기 채운 값이 모든 판정의 기준이 됩니다 |
| 알림 | 알림 6건 | 신규 공고 · 마감 임박 · 서류 착수 시점 |

---

### 1. 대시보드 (Dashboard)

**Purpose** — 앱을 켠 직후 "오늘 뭘 해야 하는지" 한 화면에서 파악한다.

**Layout**
- 히어로 배너 (전폭, 헤더에 맞닿음)
- 그 아래 26px 띄우고 2열 그리드: `1fr / 330px`, gap 26px, `align-items: start`

**히어로 배너**
- 배경 `#EEF7E2`, 테두리 1px `#C9E3A6` (상단 테두리는 없음 — 헤더와 붙음), 패딩 `22px 24px`, 커서 pointer
- 3열 그리드 `150px / 1fr / 260px`, gap 26px, 세로 가운데 정렬
- 1열: D-day `IBM Plex Mono 40px/600`, 색 `#1F5E44`, line-height 1 · 아래 5px, 마감시각 11.5px Mono `#5E6B55`
- 2열: 배지 행 → 제목 → 설명
  - 배지: 폰트 11px/600, 패딩 `3px 9px`, radius 2px, 배경 `#1F5E44`, 글자 `#EFFAE6`
  - 배지 옆 출처 텍스트 11.5px Mono `#5E6B55`, gap 10px
  - 제목: 22px/700, letter-spacing −0.02em, 위 11px
  - 설명: 13.5px `#4A5544`, line-height 1.6, 위 8px
- 3열: 세로 버튼 2개, gap 8px
  - 주 버튼: 패딩 11px, 배경 `#12150F`, 글자 `#F2F1EA` 13.5px/600, radius 2px, 가운데 정렬 — "신청서 작성하기"
  - 보조 버튼: 테두리 1px `#B4CE94`, 글자 `#33422C` 13px, 투명 배경 — "공고 원문 보기"
- 클릭 → 공고 상세로 이동

**좌측 컬럼 — "그다음 할 일"**
- 섹션 헤더: 제목 16px/700 + 우측 부제 12px `#5F6659`, gap 12px, baseline 정렬, 아래 12px
- 항목 리스트: 세로 flex, gap 8px
- 항목 행: 3열 그리드 `74px / 1fr / 112px`, gap 18px, 세로 가운데, 테두리 1px `#E2E2DB`, 배경 `#FCFCFA`, 패딩 `14px 16px`, radius 2px, 커서 pointer, **호버 시 테두리 `#12150F`**
  - 1열 D-day: Mono 16px/600, 상태별 색(아래 토큰 참조)
  - 2열: 제목 14.5px/600 + 판정 배지(10.5px, 패딩 `2px 7px`, radius 2px, nowrap), gap 9px / 아래 5px에 설명 12.5px `#6E7466` line-height 1.55
  - 3열: 행동 라벨 12px/500 `#33422C`, 우측 정렬, 끝에 ` →`
- 이어서 26px 아래 "시간표와 겹치지 않는 프로그램" 섹션 (부제 "충돌 4건 자동 제외됨")
  - 3열 그리드 gap 8px, 카드: 테두리 1px `#E2E2DB`, 배경 `#FCFCFA`, 패딩 `13px 14px`, radius 2px, 호버 테두리 `#12150F`
  - 카드 내부: 태그 10.5px Mono `#6B7163` → 제목 13.5px/600 (위 8px, line-height 1.4) → 메타 11.5px `#6E7466` (위 7px)

**우측 컬럼 (330px)** — 세로 flex, gap 16px
1. **시험 충돌 카드** — 배경 `#12150F`, 글자 `#EFEFE7`, 패딩 18px, radius 2px
   - 라벨 `EXAM CONFLICT` Mono 10.5px letter-spacing 0.14em 색 `#B9F27C`
   - 제목 15.5px/700 (위 10px, line-height 1.5), 본문 12.5px `#A3A99A` (위 9px, line-height 1.65), 본문 내 강조 텍스트는 `#EFEFE7`
2. **이번 주 캘린더** — 테두리 1px `#E2E2DB`, radius 2px, overflow hidden
   - 헤더: 패딩 `12px 15px`, 배경 `#F7F7F3`, 하단 테두리, 좌측 13px/700 "이번 주 캘린더" / 우측 "전체 보기 →" 11.5px/500 `#1F5E44` (클릭 → 캘린더 화면)
   - 행: 패딩 `10px 15px`, 하단 테두리 `#EFEFE9`, gap 12px — 요일 34px Mono 11px `#6B7163` / 제목 12.5px `#2A2F26` / 우측 6px 원형 점(일정 종류색)
3. **확인 필요 안내 카드** — 테두리 1px `#E2E2DB`, 패딩 `14px 15px`, radius 2px, 커서 pointer, 호버 테두리 `#12150F`
   - 제목 13px/700 "확인 필요 5건" / 본문 12.5px `#6E7466` line-height 1.6 (내부 `직전 학기 이수 학점`은 weight 600 `#12140F`) / 링크 12.5px/600 `#1F5E44` "성적표 업로드로 자동 입력 →"
   - 클릭 → 프로필 화면

---

### 2. 공고 목록 (Notice List)

**Purpose** — 판정 결과·출처로 걸러가며 전체 공고를 훑는다.

**Layout** — 2열 그리드 `220px / 1fr`, 콘텐츠 영역 전체 높이(패딩 없음, 사이드바가 바닥까지)

**좌측 필터 사이드바 (220px)** — 배경 `#F8F8F5`, 우측 테두리 1px `#E2E2DB`, 상하 패딩 18px
- 섹션 라벨: `판정` / `출처` — Mono 10.5px, letter-spacing 0.13em, 색 `#6B7163`, 패딩 `0 16px 10px`
- 필터 행: 패딩 `8px 16px`, 폰트 13px, 커서 pointer, 호버 배경 `#EFEFE9`
  - 좌측 8×8px 상태색 사각형(radius 1px) + 라벨(flex 1) + 우측 카운트 Mono 11px `#6B7163`, gap 9px
  - 선택됨: 배경 `#EEF7E2`, 글자 `#12140F`, weight 600 / 미선택: 투명, `#4A5044`, weight 400
  - 항목: `전체 42` `지원 가능 7` `확인 필요 5` `자격 미달 12` `마감 임박 3`
  - 점 색: 전체 `#6B7163` / 가능 `#2F8F5E` / 확인 `#C9A227` / 미달 `#B7BFC8` / 임박 `#9C482B`
- 출처 행(점 없음, 폰트 12.5px `#4A5044`, 패딩 `7px 16px`): `학교 공지 RSS 14` `한국장학재단 API 11` `공공데이터 6` `직접 등록 4` `교내 부서 7`
- 하단 등록 버튼: 마진 `18px 16px 0`, 패딩 12px, 점선 테두리 1px `#C6C7BE`, radius 2px, 가운데 정렬, 호버 테두리 `#12150F`
  - "＋ 공고 직접 등록" 12.5px `#4A5044` + 아래 4px "URL · PDF · 캡처" 11px `#6B7163`

**우측 테이블**
- 컬럼 그리드(헤더/행 동일): `78px / 1fr / 96px / 130px / 92px`
- 헤더 행: 패딩 `12px 22px`, 배경 `#FAFAF7`, 하단 테두리 1px `#E2E2DB`, Mono 10.5px letter-spacing 0.08em 색 `#6B7163` — `D-DAY` `공고` `판정` `출처` `요건`
- 데이터 행: 패딩 `14px 22px`, 하단 테두리 1px `#EFEFE9`, 세로 가운데, 커서 pointer, **호버 배경 `#F6FAF1`**
  - D-day: Mono 13.5px/600, 상태색
  - 공고: 제목 14px/600 + 아래 4px 부제 12px `#5F6659` (우측 패딩 18px)
  - 판정: 배지 10.5px, 패딩 `2px 7px`, radius 2px, nowrap
  - 출처: 11.5px Mono `#5F6659`
  - 요건: 11.5px Mono `#4A5044` — `4/4` 형식(충족/전체)
- 행 클릭 → 공고 상세

---

### 3. 공고 상세 + 판정 결과 (Notice Detail)

**Purpose** — 왜 이 판정이 나왔는지 근거를 보이고, 무엇부터 하면 되는지 알려준다. **앱에서 가장 중요한 화면.**

**Layout** — 상단에 뒤로가기 링크(12.5px `#6E7466`, 아래 14px, "← 공고 목록"), 그 아래 2열 그리드 `1fr / 420px`, gap 24px, `align-items: start`

**좌측**

1. **요약 배너** — 배경 `#EEF7E2`, 테두리 1px `#C9E3A6`, 패딩 `20px 22px`, radius 2px
   - 배지 + 출처 행(gap 10px) → 제목 24px/700 letter-spacing −0.02em (위 12px) → 메타 행(위 14px, gap 26px, Mono 12.5px `#4A5544`): 마감 / D-day / 지원금
2. **자격 요건 대조** (위 22px)
   - 섹션 제목 15px/700 + 아래 4px 설명 12px `#5F6659`
   - 표: 테두리 1px `#E2E2DB`, radius 2px, overflow hidden, 위 12px
   - 행: 3열 그리드 `28px / 1fr / 200px`, gap 14px, 패딩 `13px 16px`, 하단 테두리 `#EFEFE9`, 세로 가운데
     - 1열 마크: 14px/700 가운데 — 충족 `✓` `#1F5E44`, 주의 `!` `#9C482B`, 미충족 `✕` `#9C482B`
     - 2열: 요건 문장 13.5px/500 + 아래 3px 근거 11.5px `#6B7163` (공고문 페이지·인용구)
     - 3열: 내 값 12.5px Mono `#3B4235`
   - **근거 표기가 이 화면의 핵심 신뢰 장치다.** 어떤 문장에서 요건을 추출했는지 항상 함께 보인다.
3. **필요 서류 · 착수 역산** (위 22px)
   - 카드 리스트 gap 8px, 각 카드: 4열 그리드 `1fr / 130px / 118px / 92px`, gap 12px, 테두리 1px `#E2E2DB`, 배경 `#FCFCFA`, 패딩 `12px 15px`, radius 2px
     - 서류명 13.5px/500 / 발급처 12px `#5F6659` / 소요기간 12px Mono `#4A5044` / 상태 배지
   - 아래 10px에 **역산 결론 카드**: 배경 `#12150F`, 글자 `#EFEFE7`, 패딩 `14px 16px`, radius 2px, 13px line-height 1.6 — 강조 단어는 `#B9F27C`
     - 예: "가장 오래 걸리는 서류가 **영문 성적증명서(3영업일)** 입니다. 마감 10.16 기준 **10.13(오늘)** 신청하면 됩니다."

**우측 (420px)** — 세로 flex, gap 14px
1. **중복 수혜 경고** — 테두리 1px `#E3C9BE`, 배경 `#FCF4F0`, 패딩 `15px 16px`, radius 2px
   - 라벨 `DUPLICATE BENEFIT` Mono 10.5px letter-spacing 0.12em `#9C482B` / 제목 14px/700 (위 8px) / 본문 12.5px `#6B584F` line-height 1.6
2. **공고 원문 뷰어** — 테두리 1px `#E2E2DB`, radius 2px, overflow hidden
   - 헤더: 패딩 `11px 14px`, 배경 `#F7F7F3`, 하단 테두리 — 좌 12.5px/700 "공고 원문" / 우 11px Mono `#6B7163` 파일명·페이지수
   - 본문: 높이 250px. **프로토타입은 사선 해치 플레이스홀더다. 실제로는 PDF 렌더러(PdfiumViewer 등)를 얹고, 추출 근거 문장에 하이라이트를 그린다.**
   - 하단 캡션: 패딩 `11px 14px`, 상단 테두리, 12px `#6E7466` line-height 1.6
3. **변경 이력** — 테두리 1px `#E2E2DB`, 패딩 `14px 15px`, radius 2px
   - 제목 12.5px/700 / 본문 12.5px `#6E7466` line-height 1.65, `[수정]` 태그는 `#9C482B`
4. **액션 버튼 행** — gap 8px
   - 주: flex 1, 패딩 12px, 배경 `#12150F`, 글자 `#F2F1EA` 13.5px/600, radius 2px
   - 보조: 패딩 `12px 16px`, 테두리 1px `#C6C7BE`, 13px — "저장"

---

### 4. 혜택 · 프로그램 (Benefits)

**Purpose** — 공고로 뜨지 않는 교내 혜택·비교과까지 포함해, 시간표와 겹치지 않는 것만 보여준다.

**Layout** — 상단 탭 행 → 카드 그리드 → 제외 목록

- **탭 행**: gap 8px, 아래 18px. 버튼 패딩 `7px 14px`, 폰트 12.5px, radius 2px
  - 활성: 배경/테두리 `#12150F`, 글자 `#F2F1EA` / 비활성: 투명, 테두리 `#DDDDD5`, 글자 `#3B4235`
  - 라벨: `전체 11` `졸업요건 인정 4` `어학·자격 3` `상담·복지 4`
- **카드 그리드**: 3열 `1fr 1fr 1fr`, gap 10px
  - 카드: 테두리 1px `#E2E2DB`, 배경 `#FCFCFA`, 패딩 `16px 17px`, radius 2px
  - **우선 추천 카드만** 테두리 `#C9E3A6`, 배경 `#F7FCF1`, 배지가 채움형(배경 `#1F5E44` / 글자 `#EFFAE6`)
  - 내부: 상단 행(부서명 10.5px Mono `#6B7163` ↔ 상태 배지) → 제목 15px/700 (위 10px, line-height 1.4) → 설명 12.5px `#6E7466` (위 8px, line-height 1.6) → 구분선(위 12px, 패딩탑 11px, 상단 테두리 `#E7E7E0`) 아래 메타 행 Mono 11.5px `#5F6659`, gap 14px — 시간 / 인정 시간
- **제외 목록** (위 22px): 제목 15px/700 "시간표 충돌로 제외된 항목 4건"
  - 행: 테두리 1px `#EFEFE9`, 배경 `#FAFAF7`, 패딩 `11px 15px`, radius 2px, gap 6px(행간)
  - 좌 제목 13px `#5F6659` (flex 1) / 우 충돌 사유 11.5px Mono `#9C482B` — 예 "화 13:00 자료구조"

---

### 5. 캘린더 (Calendar)

**Purpose** — 마감·시험·학사일정을 한 격자에서 겹쳐 본다.

**Layout** — 2열 그리드 `1fr / 300px`, gap 22px, `align-items: start`

**월간 격자** — 테두리 1px `#E2E2DB`, radius 2px, overflow hidden
- 요일 헤더: 7열 균등, 배경 `#F7F7F3`, 하단 테두리, 셀 패딩 `9px 0`, 가운데, Mono 11px `#6B7163` — `SUN`…`SAT`
- 날짜 격자: 7열 균등, **행 높이 고정 112px**, 총 35칸(5주)
  - 셀: 우/하단 테두리 1px `#EFEFE9`, 패딩 `7px 8px`, overflow hidden
  - 날짜 숫자: Mono 11.5px — 오늘 `#1F5E44`, 이번 달 `#3B4235`, 이전/다음 달 `#C6C7BE`
  - 셀 배경: 오늘 `#EEF7E2`, 시험 기간(10.20–24) `#F8F8F3`, 그 외 `#FFFFFF`
  - 이벤트 칩(세로 flex, gap 3px, 위 5px): 폰트 10.5px, 패딩 `2px 5px`, radius 2px, `text-overflow: ellipsis`, nowrap
    - 마감 → 배경 `#F0E3DE` 글자 `#9C482B` / 시험 → `#12150F` · `#EFEFE7` / 학사 → `#E4F1DA` · `#1F5E44` / 개인 → `#EDEDE6` · `#4A5044`
  - **첫 칸 오프셋은 해당 월 1일의 요일로 계산한다** (샘플: 2025-10-01 = 수요일 → 앞 2칸 공백)

**우측 (300px)** — gap 14px
1. **범례 카드** — 테두리 1px `#E2E2DB`, 패딩 `14px 15px`, radius 2px. 제목 12.5px/700, 항목 행 패딩 `5px 0`, gap 9px, 10×10px radius 2px 색상칩 + 12.5px `#3B4235`
2. **겹침 경고 카드** — 배경 `#12150F` 계열, 대시보드 시험 충돌 카드와 동일 스펙(라벨 `OVERLAP`)

---

### 6. 시간표 (Timetable)

**Purpose** — 프로그램 충돌 판정의 입력값을 넣는다. **수강신청 기능이 아님을 화면에서 명시한다.**

**Layout** — 2열 그리드 `1fr / 300px`, gap 22px

**주간 격자** — 테두리 1px `#E2E2DB`, radius 2px, 배경 `#FCFCFA`, overflow hidden
- 헤더: 그리드 `56px repeat(5, 1fr)`, 배경 `#F7F7F3`, 하단 테두리. 첫 칸 비움, 요일 셀 패딩 `9px 0`, 가운데, 12.5px/600 — 월~금
- 본문: 그리드 `56px repeat(5, 1fr)` × **행 10개, 각 56px** (09:00–19:00, 1시간 단위)
  - 시간 눈금 열(1열): 우측 테두리 `#E2E2DB`, 하단 테두리 `#F0F0EA`, 패딩 `4px 6px`, 우측 정렬, Mono 10.5px `#6B7163`
  - 빈 셀: 우/하단 테두리 `#F0F0EA`
  - 수업 블록: 해당 grid-area에 절대 배치, 마진 2px, radius 2px, **좌측 3px 컬러 바** + 연한 배경, 패딩 `6px 8px`, overflow hidden
    - 과목명 12px/600 `#20261C` / 강의실 10.5px Mono `#5E6B55` (위 3px)
    - 색 페어(배경/바) 6종 순환: `#EAF2E2`/`#7FA860`, `#E9EFF3`/`#7C9DB5`, `#F4EFE3`/`#C0A662`, `#EFEAF2`/`#A18FB5`, `#E7F1EF`/`#6FA79B`, `#F2ECE8`/`#BE9782`
  - WPF 구현: `Grid`에 `RowDefinition Height="56"` 10개 + 수업은 `Grid.Row`/`Grid.RowSpan`/`Grid.Column` 바인딩

**우측 (300px)** — gap 14px
1. **업로드 드롭존** — 점선 테두리 1px `#C6C7BE`, 패딩 `20px 16px`, radius 2px, 가운데, 호버 테두리 `#12150F`
   - 13.5px/600 "시간표 파일 업로드" + 11.5px `#6B7163` line-height 1.6 "포털에서 내려받은 xlsx · 캡처 이미지 / 끌어다 놓으세요"
   - **WPF: `AllowDrop="True"` + `Drop` 핸들러. 드래그 오버 시 테두리 `#1F5E44` + 배경 `#F7FCF1`로 피드백.**
2. **수동 추가 폼** — 테두리 1px `#E2E2DB`, 패딩 `14px 15px`, radius 2px. 제목 12.5px/700 아래 10px
   - 입력 필드: 테두리 1px `#DDDDD5`, 패딩 `9px 11px`, 12.5px, radius 2px, 플레이스홀더 `#6B7163`. 세로 gap 8px, 요일/교시는 2열 그리드 gap 8px
   - 추가 버튼: 패딩 10px, 배경 `#12150F`, 글자 `#F2F1EA` 12.5px/600, radius 2px
3. **안내 문단** — 12px `#5F6659`, line-height 1.7, 좌우 패딩 2px

---

### 7. 프로필 (Profile)

**Purpose** — 판정의 기준값을 채운다. 비어 있는 항목이 어떤 판정을 막고 있는지 되돌려 보여준다.

**Layout** — 2열 그리드 `1fr / 330px`, gap 24px, `align-items: start`

**좌측**
1. **성적표 업로드 배너** — 배경 `#EEF7E2`, 테두리 1px `#C9E3A6`, 패딩 `16px 18px`, radius 2px, 가로 flex 세로 가운데, gap 18px
   - 좌(flex 1): 14.5px/700 "성적표를 올리면 아래 항목이 자동으로 채워집니다" + 12.5px `#4A5544` (위 6px) "학점·이수 학점·수강 이력 · 파일은 기기에만 저장됩니다"
   - 우: 버튼 패딩 `10px 16px`, 배경 `#12150F`, 글자 `#F2F1EA` 12.5px/600, radius 2px
2. **필드 그룹** ×2 (그룹 간 위 22px)
   - 그룹 헤더: 제목 14.5px/700 + 아래 4px 설명 12px `#5F6659`
   - 필드 그리드: 2열, gap 10px, 위 12px
   - 필드 카드: 패딩 `11px 13px`, radius 2px
     - 정상: 테두리 `#DDDDD5`, 배경 `#FCFCFA`, 값 색 `#12140F`
     - **미입력(판정 차단): 테두리 `#E3C9BE`, 배경 `#FCF4F0`, 값 색 `#9C482B`, 값 텍스트 "입력 필요"**
     - 라벨 11px Mono `#6B7163` / 값 14px/500 (위 6px)
   - 그룹 1 "필수 항목" — 학과 / 학년·학기 / 직전 학기 평점 / 소득분위
   - 그룹 2 "판정 정확도를 높이는 항목" — 직전 학기 이수 학점(미입력) / 총 취득 학점 / 비교과 이수 시간 / 어학 성적(미입력)
   - **실제 구현에서는 이 카드가 곧 편집 가능한 입력 컨트롤이다** (클릭 시 인라인 편집 또는 `TextBox`/`ComboBox` 전환). 프로토타입은 읽기 상태만 보여준다.

**우측 (330px)** — gap 14px
1. **차단 경고 카드** — 테두리 1px `#E3C9BE`, 배경 `#FCF4F0`, 패딩 `15px 16px`, radius 2px. 제목 13.5px/700 / 본문 12.5px `#6B584F` line-height 1.65, 건수는 weight 700
2. **개인정보 저장 위치 카드** — 테두리 1px `#E2E2DB`, 패딩 `15px 16px`, radius 2px
   - 제목 12.5px/700, 행: 좌우 배분, 패딩 `8px 0`, 하단 테두리 `#F0F0EA`, 12.5px — 키 `#4A5044` / 값 Mono, 값 색은 민감도별(`기기에만` `#1F5E44`, `판정 시 전송` `#7E651C`, `서버 저장` `#5F6659`)
   - 하단 캡션 11.5px `#6B7163` line-height 1.65
3. **수혜 이력 카드** — 동일 스타일, 값 색 `#5F6659` 고정

---

### 8. 알림 / 토스트 (Notifications)

**Purpose** — 놓치면 안 되는 순간(신규·마감·착수·변경·충돌)을 알린다.

**Layout** — 2열 그리드 `1fr / 330px`, gap 24px

**알림 리스트** — 테두리 1px `#E2E2DB`, radius 2px, overflow hidden
- 행: 3열 그리드 `8px / 1fr / 96px`, gap 14px, 패딩 `15px 18px`, 하단 테두리 `#EFEFE9`, 세로 가운데, 커서 pointer, 호버 배경 `#F6FAF1`
  - 1열: 8×8px 원형 점 — 긴급 `#9C482B` / 신규 `#2F8F5E` / 변경 `#C9A227`
  - 2열: 종류 배지(11px, 패딩 `2px 7px`, radius 2px) + 제목 14px/600, gap 9px / 아래 5px 본문 12.5px `#6E7466` line-height 1.55
  - 3열: 시각 11.5px Mono `#6B7163`, 우측 정렬
  - **미읽음 행 배경 `#FCFCF8`, 읽음 `#FFFFFF`**
- 종류: `착수`(노랑) `마감`(빨강) `신규`(초록) `변경`(노랑) `경고`(빨강) — 배지색은 상태 토큰 재사용

**우측 (330px)** — gap 14px
1. **토스트 미리보기 버튼** — 패딩 12px, 배경 `#12150F`, 글자 `#F2F1EA` 13px/600, radius 2px, 가운데 (개발/데모용, 프로덕션에서는 설정 화면으로 대체 가능)
2. **알림 설정 카드** — 테두리 1px `#E2E2DB`, 패딩 `15px 16px`, radius 2px
   - 제목 12.5px/700 아래 10px, 행: 좌우 배분, 패딩 `9px 0`, 하단 테두리 `#F0F0EA`
   - 라벨 12.5px `#3B4235` / 토글: 34×19px, radius 10px — 켬 트랙 `#1F5E44` 손잡이 x=17px, 끔 트랙 `#D5D5CD` 손잡이 x=2px. 손잡이 15×15px 원형 `#FFFFFF`, top 2px
   - 항목: 신규 공고(지원 가능만) ✓ / 마감 3일 전 ✓ / 서류 착수 시점 ✓ / 자격 미달 공고도 알림 ✗

**토스트 (Windows 알림)**
- 창 우하단 고정: `right: 20px; bottom: 20px`, 너비 376px
- 배경 `#1D2019`, 글자 `#F1F1E9`, 테두리 1px `#383D31`, **radius 6px**(창 내부 요소보다 큼 — OS 알림 관용), 그림자 `0 14px 34px rgba(0,0,0,.4)`, 패딩 `15px 16px`
- 상단 행: 좌 `CAMPUS SIGNAL` Mono 11px letter-spacing 0.1em `#8C9184` / 우 닫기 `✕` 12px `#8C9184`
- 제목 14.5px/700 (위 10px, line-height 1.45) / 본문 12.5px `#A3A99A` (위 7px, line-height 1.6)
- 버튼 행(위 13px, gap 8px): 주 버튼 flex 1, 패딩 8px, 배경 `#B9F27C`, 글자 `#12150F` 12.5px/600, radius 3px / 보조 패딩 `8px 14px`, 테두리 1px `#454B3C`, 글자 `#CFD4C6`
- 등장 애니메이션: `translateY(18px) → 0`, `opacity 0 → 1`, **220ms ease-out**
- **WPF 구현: 실제 배포에서는 이 디자인을 인앱 토스트로 쓰고, 앱이 백그라운드일 때는 `Microsoft.Toolkit.Uwp.Notifications`(Windows Toast)로 보낸다. 시스템 토스트는 OS 스타일을 따르므로 위 스펙은 인앱 토스트에만 적용한다.**

---

## Interactions & Behavior

**내비게이션**
- 상단 탭 7개 클릭 → 해당 뷰로 즉시 전환(전환 애니메이션 없음)
- 대시보드 히어로 / 할 일 항목 / 공고 목록 행 / 알림 행 클릭 → 공고 상세
- 대시보드 "확인 필요 5건" 카드, 할 일 중 `확인 필요` 항목 → 프로필
- 대시보드 "전체 보기 →" → 캘린더
- 대시보드 프로그램 카드 → 혜택·프로그램
- 공고 상세 "← 공고 목록" → 목록. 상세에 있는 동안 `공고` 탭은 활성 유지

**호버 상태** (모두 즉시 전환, transition 없음 — 데스크톱 앱 반응성 우선)
- 카드/행 테두리형: 테두리 → `#12150F`
- 테이블/리스트 행: 배경 → `#F6FAF1`
- 사이드바 필터 행: 배경 → `#EFEFE9`
- 헤더 탭: 테두리 → `#B9F27C`

**필터링**
- 판정 필터는 단일 선택(라디오 성격), 출처 필터는 다중 선택 의도
- 선택 시 목록 즉시 갱신, 헤더 부제의 건수도 갱신

**토스트**
- 등장 220ms ease-out. 자동 소멸은 두지 않음(중요 알림이므로 사용자가 닫아야 함) — 필요 시 마감 알림만 20초 후 자동 소멸
- "공고 열기" → 토스트 닫고 공고 상세로 이동 / "나중에" → 닫기만

**아직 정의되지 않은 상태 (구현 시 필요)**
- 로딩: 수집기 실행 중 헤더 부제에 "수집 중…" 표기 + 목록 스켈레톤
- 빈 상태: 필터 결과 0건, 공고 0건, 알림 0건
- 오류: 수집 실패(출처별), 판정 불가(프로필 미완)
- 오프라인: 마지막 수집 시각만 표시하고 캐시 데이터 사용

## State Management

MVVM 기준. 뷰모델 단위 제안:

- `ShellViewModel` — `CurrentScreen`(enum: Dash/List/Detail/Benefit/Cal/Time/Profile/Alert), `NavigateCommand`, 탭별 배지 카운트, 토스트 표시 여부
- `DashboardViewModel` — `HeroItem`(최우선 1건), `TodoItems`, `SuggestedPrograms`, `WeekEvents`, `ConflictSummary`, `BlockedJudgmentCount`
- `NoticeListViewModel` — `AllNotices`, `SelectedJudgmentFilter`, `SelectedSources`, `FilteredNotices`(파생), `FilterCounts`
- `NoticeDetailViewModel` — `Notice`, `Requirements`(요건별 충족 여부 + 근거 인용 + 내 값), `Documents`(발급처·소요일·보유 여부), `EarliestStartDate`(파생), `DuplicateConflicts`, `ChangeHistory`, `SourcePdfPath`
- `ProfileViewModel` — 필드별 값 + `IsMissing`, `BlockedNoticeCount`(파생), `UploadTranscriptCommand`
- `TimetableViewModel` — `Classes`(요일/시작교시/종료교시/과목/강의실), `ImportCommand`, `AddClassCommand`
- `CalendarViewModel` — `Month`, `Days`(35칸, 각 셀의 이벤트 목록), `Legend`
- `NotificationViewModel` — `Alerts`, `Settings`(4개 토글), `MarkReadCommand`

**핵심 파생 로직 두 가지 (판정 엔진)**

1. **자격 판정** — 공고의 요건 목록 × 프로필 값 → 요건별 `Met / Unknown / Failed`
   - 하나라도 `Failed` → **자격 미달**
   - `Failed` 없고 `Unknown` 있음 → **확인 필요** (어떤 필드가 비어서인지 반드시 함께 반환)
   - 전부 `Met` → **지원 가능**
2. **착수일 역산** — `착수일 = 마감일 − max(서류별 발급 소요 영업일) − 작성 여유(기본 1영업일)`
   - 착수일이 오늘 이하 → 할 일 목록에서 D-day 대신 **"착수"** 배지로 표시(색 `#3F6B8A`)
   - 착수일 구간이 시험 기간과 겹치면 충돌 경고 생성

**데이터 수집** — RSS / 공공 API / 사용자 직접 등록(URL·PDF·캡처). 같은 공고가 여러 출처에서 오면 병합하고 "출처 2곳에서 수집"으로 표기. 마감·금액 변경 감지 시 `[수정]` 이력과 알림 생성.

## Design Tokens

**색 — 배경/면**
| 이름 | 값 | 용도 |
|---|---|---|
| ink | `#12150F` | 헤더·타이틀바·다크 카드·주 버튼 |
| ink-alt | `#1D2019` | 토스트 배경 |
| surface | `#FFFFFF` | 콘텐츠 바탕 |
| surface-raised | `#FCFCFA` | 카드 |
| surface-sunken | `#F7F7F3` | 카드 헤더 |
| surface-side | `#F8F8F5` | 필터 사이드바 |
| surface-table-head | `#FAFAF7` | 테이블 헤더 |
| accent-bg | `#EEF7E2` | 히어로·선택 상태 |
| accent-bg-soft | `#F7FCF1` | 추천 카드 |
| hover-row | `#F6FAF1` | 행 호버 |
| hover-side | `#EFEFE9` | 사이드바 호버 |

**색 — 선**
`#E2E2DB` 기본 테두리 · `#EFEFE9` 행 구분선 · `#F0F0EA` 격자 내부선 · `#DDDDD5` 입력 필드 · `#C6C7BE` 점선/보조 버튼 · `#C9E3A6` 강조 테두리 · `#E3C9BE` 경고 테두리 · `#3A3F34` 다크 위 테두리

**색 — 텍스트** (모두 4.5:1 이상 검증 완료)
| 이름 | 값 | 대비 | 용도 |
|---|---|---|---|
| text-primary | `#12140F` | — | 본문 |
| text-strong-alt | `#2A2F26` / `#20261C` | — | 리스트 제목 |
| text-body | `#3B4235` / `#4A5044` | — | 보조 본문 |
| text-muted | `#6E7466` | 4.8:1 | 카드 설명 |
| text-muted-2 | `#5F6659` | 5.6:1 | 부제·메타 |
| text-mono-label | `#6B7163` | 4.6:1 | Mono 라벨·타임스탬프 |
| text-on-dark | `#F2F1EA` / `#EFEFE7` | — | 다크 위 본문 |
| text-on-dark-muted | `#9AA093` / `#A3A99A` | — | 다크 위 보조 |
| text-on-accent | `#4A5544` / `#5E6B55` / `#33422C` | — | 연녹 배경 위 |

⚠️ 이전 버전의 `#8A9081` / `#7C8275`는 대비 미달로 폐기됨. 사용 금지.

**색 — 판정/상태** (배지: 배경 + 글자 쌍)
| 상태 | 배경 | 글자 | 점/강조 |
|---|---|---|---|
| 지원 가능 | `#E4F1DA` | `#1F5E44` | `#2F8F5E` |
| 확인 필요 | `#F7EED4` | `#7E651C` | `#C9A227` |
| 자격 미달 | `#F0E3DE` | `#9C482B` | `#B7BFC8` |
| 마감 임박 | `#F0E3DE` | `#9C482B` | `#9C482B` |
| 착수 시점 | — | — | `#3F6B8A` |
| 채움형 강조 배지 | `#1F5E44` | `#EFFAE6` | — |

**색 — 액센트**
`#B9F27C` (라임) — 활성 탭 배경, 다크 카드 위 라벨/강조, 토스트 주 버튼. **밝은 배경 위 텍스트로는 절대 쓰지 말 것** (대비 미달).

**타이포**
- 본문: **Pretendard** (fallback: Apple SD Gothic Neo, Segoe UI). WPF에서는 앱에 Pretendard를 임베드하거나 미설치 시 `Segoe UI` → `맑은 고딕` 폴백
- 수치·라벨: **IBM Plex Mono** (fallback: Consolas)
- 스케일: 30 / 24 / 22 / 16 / 15.5 / 15 / 14.5 / 14 / 13.5 / 13 / 12.5 / 12 / 11.5 / 11 / 10.5 px
- weight: 400 / 500 / 600 / 700
- letter-spacing: 큰 제목 −0.03em · 중간 제목 −0.02em · Mono 라벨 +0.08~0.18em
- line-height: 제목 1.1~1.5 / 본문 1.55~1.7

**간격**
- 창 좌우 패딩 26px, 화면 상하 패딩 `20px / 34px`
- 컬럼 간격 22~26px, 카드 간격 8~16px, 카드 내부 패딩 `11~22px`
- 우측 컬럼 폭: 300px(캘린더·시간표) / 330px(대시보드·프로필·알림) / 420px(공고 상세)
- 좌측 사이드바 220px

**radius** — 기본 2px, 색상칩 1px, 토스트 6px, 토스트 버튼 3px, 원형 점 50%

**그림자** — 창 자체 `0 24px 60px rgba(18,22,16,.24)` (프로토타입 표현용, 실제 앱에서는 OS가 처리) / 토스트 `0 14px 34px rgba(0,0,0,.4)`

**애니메이션** — 토스트 등장 220ms ease-out. 그 외 트랜지션 없음.

## Assets

외부 이미지·아이콘 없음. 모든 그래픽은 색·타이포·테두리로만 구성했다.

- **폰트**: Pretendard(오픈 폰트, jsDelivr CDN 로드 중 — WPF에서는 프로젝트에 임베드), IBM Plex Mono(Google Fonts)
- **아이콘**: 프로토타입은 유니코드 글리프(`✓` `!` `✕` `→` `＋` `—` `▫`)만 사용. **WPF에서는 Segoe MDL2 Assets 또는 Segoe Fluent Icons로 교체 권장** (창 컨트롤은 필수)
- **PDF 뷰어 영역**: 사선 해치 플레이스홀더 → 실제 PDF 렌더러로 교체 필요

## Files

- `CampusSignal.dc.html` — **최종 디자인. 8개 화면 전부가 이 한 파일에 있다.** 상단 탭으로 화면 전환, 알림 화면의 버튼으로 토스트 재생
- `Dashboard A - Paper Briefing.dc.html` — 탐색안 A(종이 브리핑, 세리프). 채택되지 않음
- `Dashboard B - Workbench.dc.html` — 탐색안 B(3분할 워크벤치, 고밀도). 채택되지 않음
- `Dashboard C - Signal.dc.html` — 탐색안 C(채택안의 대시보드 원본)

브라우저로 `CampusSignal.dc.html`을 열면 그대로 동작한다. 화면별 초기 상태는 파일 상단 로직의 `startScreen` 값으로 바꿀 수 있다.

탐색안 A·B는 **채택되지 않았으니 구현 대상이 아니다.** 대안 레이아웃이 필요할 때만 참고할 것.
