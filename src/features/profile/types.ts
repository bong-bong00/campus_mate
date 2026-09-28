/**
 * 사용자 프로필 — AI 코칭이 공모전 요강과 대조하는 기준값.
 *
 * 설계 원칙 — 비어 있는 값은 "자격 미달"이 아니라 "확인 필요"를 만든다.
 * 그래서 대부분의 필드가 optional 이고, 판정 엔진이 빈 값을 따로 다룬다.
 */

/** 17개 시·도. 지역 공모전("○○시 거주 대학생")이 자격 조건으로 거는 단위다. */
export type Region =
  | "서울" | "부산" | "대구" | "인천" | "광주" | "대전" | "울산" | "세종"
  | "경기" | "강원" | "충북" | "충남" | "전북" | "전남" | "경북" | "경남" | "제주";

/**
 * 자격 여부를 가르는 개인 정보.
 * 나이·지역 제한은 실제 공모전 요강에 흔해서, 없으면 판정이 보류된다.
 */
export interface Personal {
  /**
   * 생년월일 "1999-03-15".
   * 연도만 받으면 "만 29세 이하" 판정이 ±1년 틀리므로 날짜까지 받는다.
   */
  birthDate?: string;
  region?: Region;
}

/** 학적 정보 — 참가 자격(재학생 한정 등) 판정에 쓰인다. */
export interface Academic {
  university?: string;
  department?: string;
  /** 1~4, 졸업생은 undefined */
  gradeYear?: number;
  /** 졸업 예정자 한정 공모전 판정용. "2027-02" */
  expectedGraduation?: string;
  isEnrolled: boolean;
}

/**
 * 보유 스킬 한 건.
 * 수준까지 받아야 "React 초급이시니 UI 구현보다 기획 역할이 낫습니다" 같은
 * 구체적인 코칭이 가능하다. 이름만 있으면 "가능합니다"밖에 못 한다.
 */
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

/**
 * 결과물 링크. 자격 판정에는 쓰지 않는다.
 * 코칭이 "GitHub 의 그 프로젝트를 이 주제에 맞게 각색해보세요" 같은
 * 구체적인 제안을 할 수 있게 하는 재료다.
 */
export interface Links {
  portfolio?: string;
  github?: string;
}

export interface StudentProfile {
  personal: Personal;
  academic: Academic;
  skills: Skill[];
  awards: Award[];
  /** 관심 분야 — 공모전 추천 정렬에 쓴다 */
  interests: string[];
  /** 자기소개 자유 서술. 코칭 프롬프트에 그대로 들어간다 */
  bio?: string;
  links: Links;
  /** 팀으로 나갈 수 있는지 — 팀원 모집 기능이 붙으면 쓴다 */
  prefersTeam?: boolean;
}

/** 기준일 시점의 만 나이. 생일이 안 지났으면 한 살 빼야 해서 직접 계산한다. */
export function koreanAge(birthDate: string, on = new Date()): number {
  const birth = new Date(birthDate);
  let age = on.getFullYear() - birth.getFullYear();
  const beforeBirthday =
    on.getMonth() < birth.getMonth() ||
    (on.getMonth() === birth.getMonth() && on.getDate() < birth.getDate());
  if (beforeBirthday) age -= 1;
  return age;
}

/**
 * 아직 안 채워진 항목 목록.
 * "확인 필요" 판정의 근거로 사용자에게 되돌려 보여준다.
 * 자격 판정에 실제로 쓰이는 항목만 넣는다 — 자기소개나 링크는 없어도 판정된다.
 */
export function missingFields(p: StudentProfile): string[] {
  const missing: string[] = [];
  if (!p.personal.birthDate) missing.push("생년월일");
  if (!p.personal.region) missing.push("거주 지역");
  if (!p.academic.department) missing.push("학과");
  if (p.academic.gradeYear === undefined) missing.push("학년");
  if (p.skills.length === 0) missing.push("보유 스킬");
  if (p.interests.length === 0) missing.push("관심 분야");
  return missing;
}
