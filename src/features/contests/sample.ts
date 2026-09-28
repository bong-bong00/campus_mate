import type { Contest } from "./types";

/**
 * 표본 공모전.
 *
 * 수집기(API · 크롤링 · 직접 등록)가 붙기 전까지 판정 엔진과 화면을 굴리는 데이터다.
 * 실제 공모전 요강이 어떤 형태로 자격 조건을 거는지에 맞춰 만들었다.
 * 수집기가 붙으면 이 파일은 지운다.
 */
export const sampleContests: Contest[] = [
  {
    id: "incheon-youth",
    title: "인천광역시 청년정책 아이디어 공모전",
    host: "인천광역시청",
    source: "공공데이터포털",
    deadline: "2026-10-16",
    prize: "대상 300만원",
    fields: ["기획", "정책"],
    requirements: [
      {
        label: "인천광역시 거주",
        evidence: '모집요강 1p · "인천광역시에 주민등록을 둔 자"',
        rule: { kind: "region", allowed: ["인천"] },
      },
      {
        label: "만 19~39세",
        evidence: '모집요강 1p · "공고일 기준 만 19세 이상 39세 이하"',
        rule: { kind: "age", min: 19, max: 39 },
      },
      {
        label: "타 공모전 수상작 제외",
        evidence: '모집요강 3p · "타 공모전에서 입상한 작품은 제출할 수 없음"',
        rule: { kind: "manual" },
      },
    ],
  },
  {
    id: "sw-univ",
    title: "전국 대학생 SW 개발 경진대회",
    host: "과학기술정보통신부",
    source: "링커리어",
    deadline: "2026-10-30",
    prize: "대상 500만원 · 장관상",
    fields: ["웹 개발", "앱 개발"],
    requirements: [
      {
        label: "대학 재학생",
        evidence: '모집요강 1p · "국내 대학에 재학 중인 학생"',
        rule: { kind: "enrolled" },
      },
      {
        label: "React 또는 Vue 사용 경험",
        evidence: '모집요강 2p · "웹 프론트엔드 프레임워크 활용 가능자"',
        rule: { kind: "skill", anyOf: ["React", "Vue"] },
      },
      {
        label: "만 29세 이하",
        evidence: '모집요강 1p · "만 29세 이하"',
        rule: { kind: "age", max: 29 },
      },
    ],
  },
  {
    id: "ux-design",
    title: "대학생 UX 디자인 공모전",
    host: "한국디자인진흥원",
    source: "위비티",
    deadline: "2026-11-13",
    prize: "최우수 200만원",
    fields: ["UI 디자인", "UX"],
    requirements: [
      {
        label: "대학 재학생",
        evidence: '모집요강 1p · "대학·대학원 재학생"',
        rule: { kind: "enrolled" },
      },
      {
        label: "3학년 이상",
        evidence: '모집요강 1p · "학부 3학년 이상 또는 대학원생"',
        rule: { kind: "gradeYear", min: 3 },
      },
      {
        label: "Figma 사용 가능",
        evidence: '모집요강 2p · "제출 원본은 Figma 링크로 제출"',
        rule: { kind: "skill", anyOf: ["Figma"] },
      },
    ],
  },
  {
    id: "public-data",
    title: "공공데이터 활용 창업 경진대회",
    host: "행정안전부",
    source: "공공데이터포털",
    deadline: "2026-11-27",
    prize: "대상 1,000만원",
    fields: ["데이터 분석", "창업"],
    requirements: [
      {
        label: "만 19세 이상",
        evidence: '모집요강 1p · "만 19세 이상 국민 누구나"',
        rule: { kind: "age", min: 19 },
      },
      {
        label: "공공데이터 활용 필수",
        evidence: '모집요강 2p · "공공데이터포털 제공 데이터를 1개 이상 활용"',
        rule: { kind: "manual" },
      },
    ],
  },
  {
    id: "grad-employ",
    title: "졸업예정자 취업 연계 아이디어 공모전",
    host: "한국산업인력공단",
    source: "직접 등록",
    deadline: "2026-12-11",
    prize: "채용 우대 · 상금 100만원",
    fields: ["기획"],
    requirements: [
      {
        label: "4학년 재학생",
        evidence: '모집요강 1p · "2027년 2월 졸업예정자에 한함"',
        rule: { kind: "gradeYear", min: 4 },
      },
      {
        label: "대학 재학생",
        evidence: "모집요강 1p",
        rule: { kind: "enrolled" },
      },
    ],
  },
];
