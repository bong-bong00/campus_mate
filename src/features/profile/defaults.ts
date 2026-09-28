import type { StudentProfile } from "./types";

/** 아무것도 입력되지 않은 프로필. 폼의 시작 상태이자, 저장된 값이 없을 때의 기본값. */
export const emptyProfile: StudentProfile = {
  personal: {},
  academic: { isEnrolled: true },
  skills: [],
  awards: [],
  interests: [],
  links: {},
};
