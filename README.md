# Campus Signal

공모전 정보를 한곳에 모아, 사용자의 프로필과 대조해 **지원 가능 / 확인 필요 / 자격 미달**을
판정하고 준비 방향을 코칭해주는 웹 서비스.

## 세 가지 원칙

1. **목록이 아니라 판정.** 공모전을 나열하지 않는다. 판정 결과를 먼저 말한다.
2. **마감이 아니라 착수일.** 서류 준비 기간을 역산해 "오늘 해야 하는 일"을 만든다.
3. **답이 아니라 근거.** AI 코칭은 요강의 어느 문장에서 나온 판단인지 항상 함께 보여준다.

## 실행

Node.js LTS 가 필요합니다.

```bash
npm install     # 처음 한 번만
npm run dev     # 개발 서버
```

브라우저에서 http://localhost:3000 을 엽니다.

```bash
npm run build   # 배포용 빌드 (에러 확인용으로도 씀)
npm run lint    # 코드 검사
```

## 기술 스택

| | |
|---|---|
| 프레임워크 | Next.js 16 (App Router) · React 19 · TypeScript |
| 스타일 | Tailwind CSS v4 |
| DB | 미정 — PostgreSQL + Prisma 예정 |
| AI 코칭 | 미연결 — Claude API 예정 |
| 배포 | 미배포 — Vercel 예정 |

## 구조

```
src/
  app/              화면 (Next.js App Router)
    page.tsx          대시보드
    contests/         공모전 목록
    chat/             AI 코칭
    profile/          프로필
  components/       화면 간 공유 컴포넌트
  features/         도메인별 타입과 로직
    contests/         공모전 · 자격 판정
    profile/          사용자 프로필
    chat/             AI 코칭
docs/design/        디자인 핸드오프 원본
```

## 현재 상태

화면 뼈대와 디자인 토큰만 있습니다. 네 화면 모두 실행되지만 내용은 자리표시자입니다.

다음 순서로 채웁니다.

1. **프로필 데이터 구조 확정** — 초안이 `src/features/profile/types.ts` 에 있습니다
2. **공모전 수집** — API·크롤링으로 데이터 가져오기
3. **AI 코칭** — Claude API 연결
4. (여유가 되면) 팀원 모집 게시판

## 이력

원래 WPF 데스크톱 앱(장학·공고 관리)으로 시작했다가, 2026-09-22 회의에서 웹앱 기반
공모전 플랫폼으로 방향을 바꿨습니다. WPF 코드는 커밋 히스토리에 남아 있습니다.
디자인 시스템(색·타이포·간격)은 그때 만든 것을 그대로 씁니다.
