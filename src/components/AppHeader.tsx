"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

/**
 * 상단 고정 헤더. 모든 화면에서 같고 제목·부제만 바뀐다.
 * 디자인 근거: docs/design/README.md → "공통 셸 (Shell)"
 */
const NAV = [
  { href: "/", label: "대시보드" },
  { href: "/contests", label: "공모전" },
  { href: "/chat", label: "AI 코칭" },
  { href: "/profile", label: "프로필" },
] as const;

const HEADINGS: Record<string, { title: string; subtitle: string }> = {
  "/": {
    title: "지금 챙기지 않으면 놓칩니다",
    subtitle: "마감이 가까운 공모전부터 보여드립니다",
  },
  "/contests": {
    title: "공모전",
    subtitle: "내 프로필과 대조해 판정한 결과입니다",
  },
  "/chat": {
    title: "AI 코칭",
    subtitle: "내 전공·스킬과 공모전 요강을 대조해 준비 방향을 잡아드립니다",
  },
  "/profile": {
    title: "프로필",
    subtitle: "여기 채운 값이 모든 판정의 기준이 됩니다",
  },
};

export default function AppHeader() {
  const pathname = usePathname();

  // 하위 경로(예: /contests/3)에서도 상위 탭이 활성 상태를 유지한다.
  const active =
    NAV.map((n) => n.href)
      .filter((href) => href !== "/" && pathname.startsWith(href))
      .at(0) ?? "/";

  const heading = HEADINGS[active] ?? HEADINGS["/"];

  return (
    <header className="bg-ink px-[26px] pt-[14px] pb-[18px]">
      <div className="flex items-end justify-between gap-6">
        <div className="min-w-0">
          <h1 className="text-[30px] font-bold leading-tight tracking-[-0.03em] text-on-dark">
            {heading.title}
          </h1>
          <p className="mt-2 text-[13px] text-on-dark-muted">{heading.subtitle}</p>
        </div>

        <nav className="flex shrink-0 gap-2">
          {NAV.map((item) => {
            const isActive = item.href === active;
            return (
              <Link
                key={item.href}
                href={item.href}
                className={[
                  "rounded-[2px] border px-[13px] py-[7px] text-[12.5px] whitespace-nowrap",
                  isActive
                    ? "border-lime bg-lime text-ink"
                    : "border-line-dark text-[#cfd4c6] hover:border-lime",
                ].join(" ")}
              >
                {item.label}
              </Link>
            );
          })}
        </nav>
      </div>
    </header>
  );
}
