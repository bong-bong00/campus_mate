# CLAUDE.md — Campus Signal 작업 지침

이 저장소에서 일하는 Claude 를 위한 파일이다. 사람이 읽는 소개는 `README.md` 에 있다.

## 이 앱이 하는 일

공모전 정보를 모아, 사용자 프로필과 대조해 **지원 가능 / 확인 필요 / 자격 미달**을 판정하고
준비 방향을 코칭한다. 화면 전체가 세 원칙 위에 서 있다. **기능을 더하거나 고칠 때 먼저 확인한다.**

1. **목록이 아니라 판정.** 공모전을 나열하지 않는다. 판정 결과를 먼저 말한다.
2. **마감이 아니라 착수일.** 서류 준비 기간을 역산해 "오늘 해야 하는 일"을 만든다.
3. **답이 아니라 근거.** AI 코칭은 요강의 어느 문장에서 나온 판단인지 항상 함께 보여준다.

## 사람에 대해

**개발자는 한 명이고, 웹 개발이 처음이다.** WPF(C#)는 해봤지만 React·TypeScript·Next.js 는 처음이다.

- 새 개념이 나오면 한 줄이라도 설명을 붙인다. 익숙하다고 가정하지 않는다
- 라이브러리를 추가할 때는 **왜 필요한지** 먼저 말한다. 조용히 늘리지 않는다
- 터미널 명령을 안내할 때는 어느 폴더에서 실행하는지 함께 적는다
- 한 번에 파일 열 개를 바꾸지 않는다. 작게 나눠서, 매번 `npm run dev` 로 눈에 보이게 한다

## 기술 스택

Next.js 16 (App Router) · React 19 · TypeScript · Tailwind CSS v4.

DB(PostgreSQL + Prisma), AI 코칭(Claude API), 배포(Vercel)는 **아직 붙지 않았다.**
붙일 때가 되면 그 단계에서 결정한다.

## 구조

```
src/
  app/              화면 — Next.js App Router
    layout.tsx        루트 레이아웃 (헤더 + 본문)
    globals.css       ★ 디자인 토큰 전부
    page.tsx          대시보드
    contests/         공모전 목록
    chat/             AI 코칭
    profile/          프로필
  components/       화면 간 공유 컴포넌트
    AppHeader.tsx     상단 다크 헤더 · 네비게이션
  features/         도메인별 타입과 로직 — 화면이 아니라 여기에 로직을 둔다
    contests/types.ts   공모전 · 요건 · 판정
    profile/types.ts    사용자 프로필
    chat/               (비어 있음)
docs/design/        디자인 핸드오프 원본
```

**모놀리식이되 경계는 선명하게.** 도메인 로직은 `features/` 에, 화면은 `app/` 에 둔다.
`app/` 의 페이지 컴포넌트가 비즈니스 로직을 들고 있으면 잘못 짠 것이다.

MSA 로 쪼개지 않는다. 1인 프로젝트에서 서비스 분리 비용은 기능 개발 시간을 전부 잡아먹는다.
학습 주제로 조사하는 것과 이 코드를 그렇게 짜는 것은 다른 얘기다.

## 반드시 지킬 것

**색은 `src/app/globals.css` 의 `@theme` 에만 있다.**
컴포넌트에 hex 리터럴(`#12150F`)을 쓰지 않는다. 새 색이 필요하면 화면이 아니라 토큰에 먼저
추가하고 `bg-ink` `text-muted` 같은 클래스로 참조한다.

**판정 결과를 저장하지 않는다.**
`Contest` 에 "지원 가능" 같은 필드를 두지 않는다. 요건만 담고 판정 함수가 계산한다.
프로필이 바뀌면 전체 판정이 자동으로 따라와야 한다.

**요건에는 항상 근거를 붙인다.**
`Requirement.evidence` 는 "요강 어느 문장에서 뽑았는지"다. 이게 이 앱의 신뢰 장치다.
LLM 에게 자유롭게 답하게 두면 그럴듯한 거짓말을 한다. 요건을 구조화해 뽑고 근거를 함께
보여주는 설계를 깨지 않는다. 근거 없는 요건을 만들지 않는다.

**빈 값은 "미달"이 아니라 "확인 필요"다.**
프로필이 덜 채워졌다고 자격 미달로 판정하지 않는다. 어떤 항목이 비어서 보류됐는지
사용자에게 되돌려 보여준다.

**주석과 UI 문구는 한국어로 쓴다.** 주석은 "무엇"이 아니라 "왜"를 적는다.

**호버에 트랜지션을 넣지 않는다.** 디자인이 명시적으로 요구한 것이다.

## 디자인

`docs/design/` 이 핸드오프 원본이다. 수치가 헷갈리면 `docs/design/README.md` 를 본다.

⚠️ **핸드오프는 장학·공고 도메인(WPF 시절)으로 쓰였다.** 화면 구성과 기능 설명은 이제
맞지 않는다. 하지만 **색·타이포·간격·컴포넌트 스타일은 그대로 유효하고**, 실제로
`globals.css` 의 토큰이 거기서 왔다. 디자인 시스템만 참고하고 화면 명세는 무시한다.

`Dashboard A`, `Dashboard B` 는 채택되지 않은 탐색안이다. 구현 대상이 아니다.

CSS px 값을 그대로 쓰면 된다 — `text-[13.5px]` 처럼 임의 값 문법을 쓴다.

## 변경 후 확인

```bash
npm run dev      # 눈으로 확인 — 가장 중요하다
npm run build    # 타입 · 빌드 에러
npm run lint     # 코드 검사
```

**코드를 고쳤으면 `npm run build` 를 돌려보고 커밋한다.** 이 프로젝트의 이전 버전(WPF)은
한 번도 컴파일하지 못한 채 커밋됐다. 같은 일을 반복하지 않는다.

Next.js 16 은 학습 데이터와 다른 부분이 많다. API·규약·파일 구조를 추측하지 말고
`node_modules/next/dist/docs/` 의 해당 문서를 먼저 읽는다. 특히 데이터 패칭·캐싱·
Route Handler 는 버전마다 크게 바뀐 영역이다.

이 파일 끝의 `nextjs-agent-rules` 블록은 `next dev` 가 매번 다시 붙인다. 지우지 않는다.

## Git

- 개발 브랜치: `claude/wpf-app-design-implementation-w1glgm`
- 기본 브랜치: `main`
- 작업을 브랜치에 쌓고, 정리되면 `main` 으로 합친다
- PR 은 사용자가 요청할 때만 만든다

## 현재 상태

| 단계 | 상태 |
|---|---|
| 프로필 데이터 구조 | **완료** — `features/profile/types.ts` |
| 프로필 입력 폼 | **완료** — 브라우저 저장소에 저장 |
| 판정 엔진 | **완료** — `features/contests/judge.ts` |
| 공모전 목록 · 상세 | **완료** — 표본 데이터 기준 |
| 공모전 수집 (API · 크롤링) | 없음 — `features/contests/sample.ts` 가 대신하고 있다 |
| AI 코칭 (Claude API) | 없음 |
| 대시보드 내용 | 없음 — 자리표시자 |
| DB (PostgreSQL + Prisma) | 없음 |
| 로그인 | 없음 |
| 팀원 모집 게시판 | 보류 — 여유가 되면 |

로딩 · 오류 화면도 아직 없다.

**판정 엔진을 고칠 때는** `features/contests/judge.ts` 의 `Rule` 종류를 늘리는 식으로 한다.
요강마다 자격 조건의 모양이 다르므로, 새 조건이 나오면 `Rule` 에 kind 를 추가하고
`check()` 에 분기를 넣는다. 화면 코드는 건드릴 일이 없어야 한다.

<!-- BEGIN:nextjs-agent-rules -->

# This is NOT the Next.js you know

This version has breaking changes — APIs, conventions, and file structure may all differ from your training data. Read the relevant guide in `node_modules/next/dist/docs/` (resolved from this file's directory; in monorepos the `next` package may not be visible from the repo root) before writing any code. Heed deprecation notices.

This block is written and re-added by `next dev` — verify at `node_modules/next/dist/server/lib/generate-agent-files.js`. Removing it from a diff only re-creates the uncommitted change; committing it with your work keeps the tree clean.

<!-- END:nextjs-agent-rules -->
