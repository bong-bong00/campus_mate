import type { StudentProfile } from "./types";
import { emptyProfile } from "./defaults";

/**
 * 프로필 저장소.
 *
 * 지금은 브라우저 localStorage 에 넣는다. DB 를 붙이면 이 파일만 서버 호출로
 * 바꾸면 되고, 화면 코드는 건드릴 필요가 없다. 그래서 화면에서 localStorage 를
 * 직접 부르지 않는다.
 *
 * React 의 useSyncExternalStore 규격(subscribe / getSnapshot / getServerSnapshot)에
 * 맞춰 내보낸다. 화면이 뜬 뒤 useEffect 로 값을 넣으면 서버에서 그린 HTML 과
 * 브라우저가 그린 결과가 어긋날 수 있는데, 이 규격을 쓰면 React 가 알아서 맞춰준다.
 */
const KEY = "campus-signal:profile";

/** 다른 탭에서 값이 바뀌면 화면도 따라간다. */
export function subscribe(onChange: () => void): () => void {
  window.addEventListener("storage", onChange);
  return () => window.removeEventListener("storage", onChange);
}

/** 저장된 원본 문자열. React 가 이전 값과 비교하므로 문자열 그대로 돌려준다. */
export function getSnapshot(): string {
  try {
    return window.localStorage.getItem(KEY) ?? "";
  } catch {
    // 시크릿 모드 등 저장소를 못 읽는 상황에서도 앱은 떠야 한다.
    return "";
  }
}

/** 서버에는 저장소가 없으므로 빈 값. */
export function getServerSnapshot(): string {
  return "";
}

/** 저장된 문자열을 프로필로. 저장 당시와 구조가 달라졌을 수 있어 기본값 위에 덮어쓴다. */
export function parseProfile(raw: string): StudentProfile {
  if (!raw) return emptyProfile;
  try {
    return { ...emptyProfile, ...(JSON.parse(raw) as StudentProfile) };
  } catch {
    return emptyProfile;
  }
}

export function saveProfile(profile: StudentProfile): boolean {
  try {
    window.localStorage.setItem(KEY, JSON.stringify(profile));
    return true;
  } catch {
    return false;
  }
}
