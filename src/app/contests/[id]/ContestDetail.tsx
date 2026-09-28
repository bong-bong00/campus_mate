"use client";

import Link from "next/link";
import { useSyncExternalStore } from "react";
import { DDay, JudgmentBadge, ResultMark } from "@/components/JudgmentBadge";
import { blockingFields, checkAll, judge } from "@/features/contests/judge";
import { sampleContests } from "@/features/contests/sample";
import { daysLeft, ddayText } from "@/features/contests/types";
import {
  getServerSnapshot,
  getSnapshot,
  parseProfile,
  subscribe,
} from "@/features/profile/storage";

/**
 * 공모전 상세 — 왜 이 판정이 나왔는지 근거를 보이는 화면.
 * 요건마다 "요강 어느 문장에서 뽑았는지"를 함께 보여주는 것이 이 화면의 핵심이다.
 */
export default function ContestDetail({ id }: { id: string }) {
  const stored = useSyncExternalStore(subscribe, getSnapshot, getServerSnapshot);
  const profile = parseProfile(stored);

  const contest = sampleContests.find((c) => c.id === id);
  if (!contest) {
    return (
      <p className="rounded-[2px] border border-line p-6 text-[13px] text-muted">
        그런 공모전이 없습니다.{" "}
        <Link href="/contests" className="text-ok-fg underline">목록으로</Link>
      </p>
    );
  }

  const checks = checkAll(contest, profile);
  const judgment = judge(checks);
  const blocked = blockingFields(checks);

  const bannerTone =
    judgment === "지원 가능"
      ? "border-line-accent bg-accent-bg"
      : judgment === "확인 필요"
        ? "border-[#e8dbb8] bg-[#fcf8ec]"
        : "border-line-warn bg-warn-surface";

  return (
    <>
      <Link href="/contests" className="text-[12.5px] text-muted hover:text-body">
        ← 공모전 목록
      </Link>

      <div className="mt-3.5 grid grid-cols-1 gap-6 lg:grid-cols-[1fr_330px]">
        <div>
          {/* 요약 배너 */}
          <div className={`rounded-[2px] border p-[22px] ${bannerTone}`}>
            <div className="flex items-center gap-2.5">
              <JudgmentBadge judgment={judgment} />
              <span className="font-mono text-[11.5px] text-[#5e6b55]">
                {contest.host} · {contest.source}
              </span>
            </div>
            <h2 className="mt-3 text-[24px] font-bold tracking-[-0.02em]">{contest.title}</h2>
            <div className="mt-3.5 flex flex-wrap gap-6 font-mono text-[12.5px] text-[#4a5544]">
              <span>마감 {contest.deadline}</span>
              <DDay text={ddayText(contest.deadline)} days={daysLeft(contest.deadline)} />
              {contest.prize && <span>{contest.prize}</span>}
            </div>
          </div>

          {/* 자격 요건 대조 */}
          <h3 className="mt-[22px] text-[15px] font-bold">자격 요건 대조</h3>
          <p className="mt-1 text-[12px] text-muted-2">
            요강에서 추출한 요건을 내 프로필과 겹쳐 판정했습니다
          </p>

          <div className="mt-3 overflow-hidden rounded-[2px] border border-line">
            {checks.map(({ requirement, result, myValue }, i) => (
              <div
                key={i}
                className="grid grid-cols-[28px_1fr_160px] items-center gap-3.5 border-b border-line-row px-4 py-3.5 last:border-b-0"
              >
                <span className="text-center">
                  <ResultMark result={result} />
                </span>
                <span>
                  <span className="block text-[13.5px] font-medium">{requirement.label}</span>
                  {/* 근거 표기 — 이 화면의 신뢰 장치다 */}
                  <span className="mt-1 block text-[11.5px] text-mono-label">
                    {requirement.evidence}
                  </span>
                </span>
                <span className="font-mono text-[12.5px] text-body-2">{myValue}</span>
              </div>
            ))}
          </div>
        </div>

        {/* 우측 — 판정을 막고 있는 것 */}
        <aside>
          {judgment === "확인 필요" && (
            <div className="rounded-[2px] border border-[#e8dbb8] bg-[#fcf8ec] p-4">
              <p className="font-mono text-[10.5px] tracking-[0.12em] text-check-fg">
                MISSING INPUT
              </p>
              <p className="mt-2 text-[14px] font-bold">프로필 항목이 판정을 막고 있습니다</p>
              <p className="mt-2 text-[12.5px] leading-relaxed text-[#6b584f]">
                {blocked.join(" · ")} 을(를) 채우면 이 공모전이 즉시 판정됩니다.
              </p>
              <Link
                href="/profile"
                className="mt-3 inline-block text-[12.5px] font-semibold text-ok-fg underline"
              >
                프로필 채우러 가기 →
              </Link>
            </div>
          )}

          {judgment === "자격 미달" && (
            <div className="rounded-[2px] border border-line-warn bg-warn-surface p-4">
              <p className="font-mono text-[10.5px] tracking-[0.12em] text-fail-fg">NOT ELIGIBLE</p>
              <p className="mt-2 text-[14px] font-bold">필수 요건에서 미달입니다</p>
              <p className="mt-2 text-[12.5px] leading-relaxed text-[#6b584f]">
                {checks
                  .filter((c) => c.result === "미충족")
                  .map((c) => `${c.requirement.label} (내 값: ${c.myValue})`)
                  .join(", ")}
                . 프로필 값이 바뀌면 자동으로 다시 판정합니다.
              </p>
            </div>
          )}

          {judgment === "지원 가능" && (
            <div className="rounded-[2px] bg-ink p-[18px]">
              <p className="font-mono text-[10.5px] tracking-[0.14em] text-lime">READY</p>
              <p className="mt-2.5 text-[15.5px] font-bold leading-relaxed text-[#efefe7]">
                자격 요건을 모두 충족합니다
              </p>
              <p className="mt-2 text-[12.5px] leading-relaxed text-[#a3a99a]">
                마감까지 {daysLeft(contest.deadline)}일 남았습니다.
              </p>
            </div>
          )}

          {checks.some((c) => c.result === "참고") && (
            <div className="mt-3.5 rounded-[2px] border border-line p-4">
              <p className="text-[12.5px] font-bold">직접 확인이 필요한 조항</p>
              <ul className="mt-2 flex flex-col gap-1.5">
                {checks
                  .filter((c) => c.result === "참고")
                  .map((c, i) => (
                    <li key={i} className="text-[12.5px] leading-relaxed text-muted">
                      {c.requirement.label}
                    </li>
                  ))}
              </ul>
              <p className="mt-2 text-[11.5px] text-muted-2">
                시스템이 판단할 수 없는 조항이라 판정에는 넣지 않았습니다.
              </p>
            </div>
          )}
        </aside>
      </div>
    </>
  );
}
