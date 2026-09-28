import ContestDetail from "./ContestDetail";

/**
 * 공모전 상세.
 * Next 16 에서 params 는 Promise 다 — await 해야 값이 나온다.
 */
export default async function ContestDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;

  return (
    <div className="px-[26px] py-5 pb-[34px]">
      <ContestDetail id={id} />
    </div>
  );
}
