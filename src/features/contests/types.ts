/**
 * 공모전과 자격 판정.
 *
 * WPF 버전의 판정 엔진을 그대로 옮겨온 개념이다. 핵심은 두 가지 —
 *  1) 요건마다 "요강 어느 문장에서 뽑았는지"(evidence)를 함께 들고 다닌다.
 *  2) 대조할 값이 없으면 "미달"이 아니라 "확인 필요"다.
 * LLM 이 자유롭게 답하게 두면 그럴듯한 거짓말을 하므로, 요건을 구조화해 뽑고
 * 근거를 함께 보여주는 것이 신뢰성의 전부다.
 */

/** 공고 하나에 대한 최종 판정. */
export type Judgment = "지원 가능" | "확인 필요" | "자격 미달";

/** 요건 하나에 대한 대조 결과. */
export type RequirementResult = "충족" | "확인 필요" | "미충족";

/** 요강에서 추출한 참가 자격 한 줄. */
export interface Requirement {
  label: string;
  /** 요강 근거 — 예: '모집요강 2p · "재학생에 한함"' */
  evidence: string;
}

/** 요건 하나를 프로필과 대조한 결과. */
export interface RequirementCheck {
  requirement: Requirement;
  result: RequirementResult;
  /** 내 프로필의 대응 값. 없으면 "미입력" */
  myValue: string;
}

export interface Contest {
  id: string;
  title: string;
  host: string;
  /** 수집 출처 — 링커리어 / 공공데이터 / 직접 등록 등 */
  source: string;
  deadline: string; // ISO
  prize?: string;
  url?: string;
  requirements: Requirement[];
}

/** 요건별 결과를 모아 공모전 하나의 판정을 낸다. */
export function judge(checks: RequirementCheck[]): Judgment {
  if (checks.some((c) => c.result === "미충족")) return "자격 미달";
  if (checks.some((c) => c.result === "확인 필요")) return "확인 필요";
  return "지원 가능";
}

/** 마감까지 남은 일수. 음수면 이미 지났다. */
export function daysLeft(deadline: string, today = new Date()): number {
  const d = new Date(deadline);
  const ms = d.setHours(0, 0, 0, 0) - new Date(today).setHours(0, 0, 0, 0);
  return Math.round(ms / 86_400_000);
}
