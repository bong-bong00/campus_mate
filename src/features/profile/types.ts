/**
 * 사용자 프로필 — AI 코칭이 공모전 요강과 대조하는 기준값.
 *
 * ⚠️ 초안입니다. 2026-09-22 회의에서 "다음 회의 전까지 정의"로 남은 항목이라,
 *    확정 전까지는 자유롭게 바꿔도 됩니다. 확정되면 이 주석을 지우세요.
 *
 * 설계 원칙 — 비어 있는 값은 "자격 미달"이 아니라 "확인 필요"를 만든다.
 * 그래서 대부분의 필드가 optional 이고, 판정 엔진이 빈 값을 따로 다룬다.
 */

/** 학적 정보 — 참가 자격(재학생 한정 등) 판정에 쓰인다. */
export interface Academic {
  university?: string;
  department?: string;
  /** 1~4, 졸업생은 undefined */
  gradeYear?: number;
  /** 졸업 예정자 한정 공모전 판정용 */
  expectedGraduation?: string; // "2027-02"
  isEnrolled: boolean;
}

/** 보유 스킬 한 건. 수준까지 받아야 "가능/불가" 판정이 갈린다. */
export interface Skill {
  name: string; // "React", "Figma", "Python"
  level: "입문" | "초급" | "중급" | "고급";
  /** 실제로 써본 개월 수 */
  months?: number;
}

/** 수상·참가 이력 — 코칭이 "이런 경험을 어필하라"고 말할 근거가 된다. */
export interface Award {
  contestName: string;
  prize?: string; // "대상", "장려상", "참가"
  year: number;
  role?: string; // "프론트엔드", "기획"
}

export interface StudentProfile {
  academic: Academic;
  skills: Skill[];
  awards: Award[];
  /** 관심 분야 — 공모전 추천 정렬에 쓴다 */
  interests: string[];
  /** 자기소개 자유 서술. 코칭 프롬프트에 그대로 들어간다 */
  bio?: string;
  /** 팀으로 나갈 수 있는지 — 팀원 모집 기능이 붙으면 쓴다 */
  prefersTeam?: boolean;
}

/** 아직 안 채워진 항목 목록. "확인 필요" 판정의 근거로 사용자에게 되돌려 보여준다. */
export function missingFields(p: StudentProfile): string[] {
  const missing: string[] = [];
  if (!p.academic.department) missing.push("학과");
  if (p.academic.gradeYear === undefined) missing.push("학년");
  if (p.skills.length === 0) missing.push("보유 스킬");
  if (p.interests.length === 0) missing.push("관심 분야");
  return missing;
}
