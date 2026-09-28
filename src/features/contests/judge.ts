import { koreanAge, type StudentProfile } from "@/features/profile/types";
import type { Contest, Judgment, RequirementCheck, Rule } from "./types";

/**
 * 판정 엔진.
 *
 * 요건 하나하나를 프로필과 대조해 결과를 내고, 그 결과를 모아 공모전의 판정을 낸다.
 * 판정 결과는 어디에도 저장하지 않는다 — 프로필이 바뀌면 전체가 자동으로 따라가야 한다.
 */

/** 요건 하나를 프로필과 대조한다. */
function check(rule: Rule, p: StudentProfile): { result: RequirementCheck["result"]; myValue: string } {
  switch (rule.kind) {
    case "age": {
      if (!p.personal.birthDate) return { result: "확인 필요", myValue: "생년월일 미입력" };
      const age = koreanAge(p.personal.birthDate);
      const tooYoung = rule.min !== undefined && age < rule.min;
      const tooOld = rule.max !== undefined && age > rule.max;
      return {
        result: tooYoung || tooOld ? "미충족" : "충족",
        myValue: `만 ${age}세`,
      };
    }

    case "region": {
      if (!p.personal.region) return { result: "확인 필요", myValue: "거주 지역 미입력" };
      return {
        result: rule.allowed.includes(p.personal.region) ? "충족" : "미충족",
        myValue: p.personal.region,
      };
    }

    case "enrolled":
      return {
        result: p.academic.isEnrolled ? "충족" : "미충족",
        myValue: p.academic.isEnrolled ? "재학" : "휴학",
      };

    case "gradeYear": {
      const year = p.academic.gradeYear;
      if (year === undefined) return { result: "확인 필요", myValue: "학년 미입력" };
      const tooLow = rule.min !== undefined && year < rule.min;
      const tooHigh = rule.max !== undefined && year > rule.max;
      return {
        result: tooLow || tooHigh ? "미충족" : "충족",
        myValue: `${year}학년`,
      };
    }

    case "skill": {
      if (p.skills.length === 0) return { result: "확인 필요", myValue: "보유 스킬 미입력" };
      // 대소문자·공백 차이로 판정이 흔들리지 않게 정규화해서 비교한다.
      const wanted = rule.anyOf.map((w) => w.trim().toLowerCase());
      const matched = p.skills.filter((s) => wanted.includes(s.name.trim().toLowerCase()));
      return matched.length > 0
        ? { result: "충족", myValue: matched.map((s) => `${s.name} ${s.level}`).join(", ") }
        : { result: "미충족", myValue: `보유: ${p.skills.map((s) => s.name).join(", ") || "없음"}` };
    }

    case "manual":
      // 시스템이 알 수 없는 조항. 판정을 막지 않고 사용자에게 확인을 맡긴다.
      return { result: "참고", myValue: "직접 확인" };
  }
}

export function checkAll(contest: Contest, profile: StudentProfile): RequirementCheck[] {
  return contest.requirements.map((requirement) => ({
    requirement,
    ...check(requirement.rule, profile),
  }));
}

/**
 * 요건별 결과를 모아 공모전 하나의 판정을 낸다.
 * "참고"는 판정에 영향을 주지 않는다 — 시스템이 판단할 수 없는 조항일 뿐이다.
 */
export function judge(checks: RequirementCheck[]): Judgment {
  if (checks.some((c) => c.result === "미충족")) return "자격 미달";
  if (checks.some((c) => c.result === "확인 필요")) return "확인 필요";
  return "지원 가능";
}

/** 판정이 보류된 이유 — 어떤 프로필 항목이 비어서인지. */
export function blockingFields(checks: RequirementCheck[]): string[] {
  return checks
    .filter((c) => c.result === "확인 필요")
    .map((c) => c.myValue.replace(" 미입력", ""))
    .filter((v, i, arr) => arr.indexOf(v) === i);
}

/** 목록의 "요건" 열 — 충족/전체. 참고 항목은 분모에서 뺀다. */
export function requirementRatio(checks: RequirementCheck[]): string {
  const counted = checks.filter((c) => c.result !== "참고");
  const met = counted.filter((c) => c.result === "충족").length;
  return `${met}/${counted.length}`;
}
