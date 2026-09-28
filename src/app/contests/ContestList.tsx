"use client";

import Link from "next/link";
import { useSyncExternalStore } from "react";
import { DDay, JudgmentBadge } from "@/components/JudgmentBadge";
import { checkAll, judge, requirementRatio } from "@/features/contests/judge";
import { sampleContests } from "@/features/contests/sample";
import { daysLeft, ddayText } from "@/features/contests/types";
import {
  getServerSnapshot,
  getSnapshot,
  parseProfile,
  subscribe,
} from "@/features/profile/storage";
import { missingFields } from "@/features/profile/types";

/**
 * 공모전 목록.
 * 프로필이 브라우저에만 있으므로 판정도 브라우저에서 한다.
 * 프로필을 고치면 이 화면의 판정이 곧바로 따라 바뀐다.
 */
export default function ContestList() {
  const stored = useSyncExternalStore(subscribe, getSnapshot, getServerSnapshot);
  const profile = parseProfile(stored);
  const missing = missingFields(profile);

  // 마감이 가까운 순. 판정보다 급한 것이 먼저다.
  const rows = sampleContests
    .map((contest) => {
      const checks = checkAll(contest, profile);
      return { contest, checks, judgment: judge(checks) };
    })
    .sort((a, b) => daysLeft(a.contest.deadline) - daysLeft(b.contest.deadline));

  return (
    <>
      {missing.length > 0 && (
        <div className="mb-4 rounded-[2px] border border-line-warn bg-warn-surface p-4">
          <p className="text-[13px] font-bold">프로필이 덜 채워져 판정이 보류된 항목이 있습니다</p>
          <p className="mt-1.5 text-[12.5px] text-[#6b584f]">
            {missing.join(" · ")} 를 채우면 정확한 판정을 받을 수 있습니다.{" "}
            <Link href="/profile" className="font-semibold text-ok-fg underline">
              프로필 채우러 가기 →
            </Link>
          </p>
        </div>
      )}

      <div className="overflow-hidden rounded-[2px] border border-line">
        <div className="grid grid-cols-[78px_1fr_96px_92px] gap-3 border-b border-line bg-sunken px-[22px] py-3 font-mono text-[10.5px] tracking-[0.08em] text-mono-label">
          <span>D-DAY</span>
          <span>공모전</span>
          <span>판정</span>
          <span>요건</span>
        </div>

        {rows.map(({ contest, checks, judgment }) => (
          <Link
            key={contest.id}
            href={`/contests/${contest.id}`}
            className="grid grid-cols-[78px_1fr_96px_92px] items-center gap-3 border-b border-line-row px-[22px] py-3.5 last:border-b-0 hover:bg-hover-row"
          >
            <DDay text={ddayText(contest.deadline)} days={daysLeft(contest.deadline)} />
            <span className="min-w-0">
              <span className="block truncate text-[14px] font-semibold">{contest.title}</span>
              <span className="mt-1 block truncate text-[12px] text-muted-2">
                {contest.host} · {contest.source}
              </span>
            </span>
            <span>
              <JudgmentBadge judgment={judgment} />
            </span>
            <span className="font-mono text-[11.5px] text-body-2">
              {requirementRatio(checks)}
            </span>
          </Link>
        ))}
      </div>

      <p className="mt-3 text-[11.5px] text-muted">
        표본 데이터 {sampleContests.length}건입니다. 수집기를 붙이면 실제 공모전으로 바뀝니다.
      </p>
    </>
  );
}
