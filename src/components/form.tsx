"use client";

/**
 * 입력 컨트롤 모음.
 * 디자인의 "필드 카드" 스타일(라벨은 작은 Mono, 값은 14px, 얇은 테두리)을 한 곳에 모아둔다.
 * 화면마다 같은 클래스를 반복해 쓰지 않기 위한 것이다.
 */

const inputClass =
  "w-full rounded-[2px] border border-line-input bg-surface px-[11px] py-[9px] " +
  "text-[13px] outline-none focus:border-ink";

/** 라벨 + 아무 입력 컨트롤. 미입력이면서 판정에 필요한 항목은 경고색으로 표시한다. */
export function Field({
  label,
  hint,
  required,
  filled,
  children,
}: {
  label: string;
  hint?: string;
  /** 자격 판정에 쓰이는 항목인지 */
  required?: boolean;
  /** 값이 채워졌는지 */
  filled?: boolean;
  children: React.ReactNode;
}) {
  const needsInput = required && !filled;

  return (
    <label
      className={[
        "block rounded-[2px] border p-[13px]",
        needsInput ? "border-line-warn bg-warn-surface" : "border-line bg-raised",
      ].join(" ")}
    >
      <span className="flex items-baseline gap-2">
        <span className="font-mono text-[11px] text-mono-label">{label}</span>
        {needsInput && <span className="text-[10.5px] text-fail-fg">입력 필요</span>}
      </span>
      <span className="mt-1.5 block">{children}</span>
      {hint && <span className="mt-1.5 block text-[11px] text-muted">{hint}</span>}
    </label>
  );
}

export function TextInput(props: React.InputHTMLAttributes<HTMLInputElement>) {
  return <input {...props} className={inputClass} />;
}

export function Select({
  children,
  ...props
}: React.SelectHTMLAttributes<HTMLSelectElement>) {
  return (
    <select {...props} className={inputClass}>
      {children}
    </select>
  );
}

export function TextArea(props: React.TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea {...props} rows={4} className={`${inputClass} resize-y`} />;
}

/** 섹션 제목 + 설명. 폼을 묶음으로 나눈다. */
export function Group({
  title,
  description,
  children,
}: {
  title: string;
  description?: string;
  children: React.ReactNode;
}) {
  return (
    <section className="mt-[22px] first:mt-0">
      <h2 className="text-[14.5px] font-bold">{title}</h2>
      {description && <p className="mt-1 text-[12px] text-muted-2">{description}</p>}
      <div className="mt-3">{children}</div>
    </section>
  );
}

/** 작은 보조 버튼 (항목 추가 / 삭제). */
export function MiniButton({
  children,
  ...props
}: React.ButtonHTMLAttributes<HTMLButtonElement>) {
  return (
    <button
      type="button"
      {...props}
      className="rounded-[2px] border border-line-dashed px-2.5 py-1.5 text-[12px] text-body-2 hover:border-ink"
    >
      {children}
    </button>
  );
}
