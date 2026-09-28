/** 공모전 목록 — 판정 결과·출처로 걸러가며 훑는 화면. */
export default function ContestsPage() {
  return (
    <div className="px-[26px] py-5 pb-[34px]">
      <div className="rounded-[2px] border border-line">
        <div className="border-b border-line bg-sunken px-[22px] py-3">
          <p className="font-mono text-[10.5px] tracking-[0.08em] text-mono-label">
            D-DAY · 공모전 · 판정 · 출처
          </p>
        </div>
        <p className="px-[22px] py-10 text-center text-[13px] text-muted">
          아직 수집된 공모전이 없습니다.
          <br />
          수집 기능(API · 크롤링)을 붙이면 여기에 목록이 채워집니다.
        </p>
      </div>
    </div>
  );
}
