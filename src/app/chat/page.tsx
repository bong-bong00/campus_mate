/**
 * AI 코칭 — 프로필 × 공모전 요강을 대조해 준비 방향을 알려준다.
 * 이 프로젝트를 다른 공모전 사이트와 구분 짓는 핵심 기능이다.
 */
export default function ChatPage() {
  return (
    <div className="px-[26px] py-5 pb-[34px]">
      <div className="rounded-[2px] border border-line p-6">
        <p className="text-[13px] text-muted">
          아직 연결되지 않았습니다. 프로필 구조가 확정되면 Claude API 를 붙입니다.
        </p>
        <p className="mt-3 text-[12.5px] leading-relaxed text-muted-2">
          단순히 답만 주는 챗봇이 아니라, <strong className="font-semibold text-body">요강의 어느 문장에서
          나온 판단인지 근거를 함께 보여주는 것</strong>이 목표입니다.
        </p>
      </div>
    </div>
  );
}
