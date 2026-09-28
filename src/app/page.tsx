/**
 * 대시보드 — 앱을 켠 직후 "오늘 뭘 해야 하는지"를 한 화면에 담는다.
 * 지금은 뼈대만 있고, 수집기와 판정 엔진이 붙으면 실제 데이터가 들어온다.
 */
export default function DashboardPage() {
  return (
    <div className="px-[26px] py-5 pb-[34px]">
      <section className="rounded-[2px] border border-line-accent bg-accent-bg p-6">
        <p className="font-mono text-[11.5px] text-[#5e6b55]">가장 급한 한 건</p>
        <h2 className="mt-3 text-[22px] font-bold tracking-[-0.02em]">
          아직 수집된 공모전이 없습니다
        </h2>
        <p className="mt-2 text-[13.5px] leading-relaxed text-[#4a5544]">
          공모전 수집 기능을 붙이면 마감이 가장 가까운 한 건이 여기에 크게 표시됩니다.
        </p>
      </section>

      <div className="mt-[26px] grid grid-cols-1 gap-[26px] lg:grid-cols-[1fr_330px]">
        <section>
          <h3 className="text-[16px] font-bold">그다음 할 일</h3>
          <p className="mt-3 rounded-[2px] border border-line bg-raised p-4 text-[12.5px] text-muted">
            프로필을 채우고 공모전을 수집하면, 서류 준비 기간을 역산해 착수일 순으로 정렬됩니다.
          </p>
        </section>

        <aside className="rounded-[2px] bg-ink p-[18px]">
          <p className="font-mono text-[10.5px] tracking-[0.14em] text-lime">NEXT STEP</p>
          <p className="mt-2.5 text-[15.5px] font-bold leading-relaxed text-[#efefe7]">
            먼저 프로필을 채워주세요
          </p>
          <p className="mt-2 text-[12.5px] leading-relaxed text-[#a3a99a]">
            전공·스킬·수상 이력이 있어야 AI 코칭이 공모전 요강과 대조할 수 있습니다.
          </p>
        </aside>
      </div>
    </div>
  );
}
