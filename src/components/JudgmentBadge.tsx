import type { Judgment, RequirementResult } from "@/features/contests/types";

/** 판정 배지. 색은 globals.css 의 토큰에서 온다. */
export function JudgmentBadge({ judgment }: { judgment: Judgment }) {
  const tone =
    judgment === "지원 가능"
      ? "bg-ok-bg text-ok-fg"
      : judgment === "확인 필요"
        ? "bg-check-bg text-check-fg"
        : "bg-fail-bg text-fail-fg";

  return (
    <span className={`rounded-[2px] px-[7px] py-0.5 text-[10.5px] whitespace-nowrap ${tone}`}>
      {judgment}
    </span>
  );
}

/** 요건 대조표 1열의 마크. 충족 ✓ · 확인 필요 ? · 미충족 ✕ · 참고 ! */
export function ResultMark({ result }: { result: RequirementResult }) {
  const { mark, tone } = {
    "충족": { mark: "✓", tone: "text-ok-fg" },
    "확인 필요": { mark: "?", tone: "text-check-fg" },
    "미충족": { mark: "✕", tone: "text-fail-fg" },
    "참고": { mark: "!", tone: "text-check-fg" },
  }[result];

  return <span className={`text-[14px] font-bold ${tone}`}>{mark}</span>;
}

/** D-day 색 — 5일 이내면 경고색. */
export function DDay({ text, days }: { text: string; days: number }) {
  return (
    <span
      className={`font-mono text-[13.5px] font-semibold ${
        days <= 5 ? "text-fail-fg" : "text-body-2"
      }`}
    >
      {text}
    </span>
  );
}
