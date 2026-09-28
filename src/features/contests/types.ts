import type { Region } from "@/features/profile/types";

/**
 * 공모전과 자격 판정.
 *
 * 핵심은 두 가지다.
 *  1) 요건마다 "요강 어느 문장에서 뽑았는지"(evidence)를 함께 들고 다닌다.
 *  2) 대조할 값이 없으면 "미달"이 아니라 "확인 필요"다.
 * LLM 에게 자유롭게 답하게 두면 그럴듯한 거짓말을 한다. 요건을 구조화해 뽑고
 * 근거를 함께 보여주는 것이 신뢰성의 전부다.
 */

/** 공고 하나에 대한 최종 판정. */
export type Judgment = "지원 가능" | "확인 필요" | "자격 미달";

/**
 * 요건 하나에 대한 대조 결과.
 * "참고"는 시스템이 구조적으로 판단할 수 없는 조항(미발표작 여부 등)이다.
 * 판정을 막지는 않지만 사용자에게는 반드시 보여준다.
 */
export type RequirementResult = "충족" | "확인 필요" | "미충족" | "참고";

/**
 * 요건을 기계가 대조할 수 있는 형태로 표현한 것.
 * 나중에 LLM 이 요강에서 요건을 뽑을 때도 이 모양으로 내놓게 한다.
 */
export type Rule =
  /** 만 나이 제한. "만 19~34세" */
  | { kind: "age"; min?: number; max?: number }
  /** 거주 지역 제한. "인천광역시 거주자" */
  | { kind: "region"; allowed: Region[] }
  /** 재학생 한정 */
  | { kind: "enrolled" }
  /** 학년 제한. "3학년 이상" */
  | { kind: "gradeYear"; min?: number; max?: number }
  /** 나열한 스킬 중 하나라도 있으면 충족 */
  | { kind: "skill"; anyOf: string[] }
  /** 사람이 직접 확인해야 하는 조항. 판정을 막지 않고 "참고"로 표시한다 */
  | { kind: "manual" };

/** 요강에서 추출한 참가 자격 한 줄. */
export interface Requirement {
  label: string;
  /** 요강 근거 — 예: '모집요강 2p · "인천광역시에 거주하는 청년"' */
  evidence: string;
  rule: Rule;
}

/** 요건 하나를 프로필과 대조한 결과. */
export interface RequirementCheck {
  requirement: Requirement;
  result: RequirementResult;
  /** 내 프로필의 대응 값. 없으면 안내 문구 */
  myValue: string;
}

export interface Contest {
  id: string;
  title: string;
  host: string;
  /** 수집 출처 — 링커리어 / 공공데이터 / 직접 등록 등 */
  source: string;
  deadline: string; // "2026-10-31"
  prize?: string;
  url?: string;
  /** 분야 태그 — 관심 분야와 맞춰 추천 정렬에 쓴다 */
  fields: string[];
  requirements: Requirement[];
}

/** 마감까지 남은 일수. 음수면 이미 지났다. */
export function daysLeft(deadline: string, today = new Date()): number {
  const end = new Date(deadline);
  end.setHours(0, 0, 0, 0);
  const from = new Date(today);
  from.setHours(0, 0, 0, 0);
  return Math.round((end.getTime() - from.getTime()) / 86_400_000);
}

/** 목록 1열에 찍히는 문구. */
export function ddayText(deadline: string, today = new Date()): string {
  const d = daysLeft(deadline, today);
  if (d < 0) return "마감";
  if (d === 0) return "D-DAY";
  return `D-${d}`;
}
