/** 프로필 — 판정의 기준값을 채우는 화면. */
export default function ProfilePage() {
  return (
    <div className="px-[26px] py-5 pb-[34px]">
      <div className="rounded-[2px] border border-line-accent bg-accent-bg p-[18px]">
        <p className="text-[14.5px] font-bold">여기 채운 값이 모든 판정의 기준이 됩니다</p>
        <p className="mt-1.5 text-[12.5px] text-[#4a5544]">
          전공 · 스킬 · 수상 이력 · 관심 분야
        </p>
      </div>
      <p className="mt-[22px] rounded-[2px] border border-line bg-raised p-4 text-[12.5px] text-muted">
        입력 폼은 프로필 데이터 구조가 확정된 뒤에 만듭니다.
        초안은 <code className="font-mono">src/features/profile/types.ts</code> 에 있습니다.
      </p>
    </div>
  );
}
