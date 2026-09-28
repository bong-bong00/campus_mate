"use client"; // 입력값을 다루므로 브라우저에서 도는 컴포넌트다.

import { useState, useSyncExternalStore } from "react";
import { Field, Group, MiniButton, Select, TextArea, TextInput } from "@/components/form";
import {
  getServerSnapshot,
  getSnapshot,
  parseProfile,
  saveProfile,
  subscribe,
} from "@/features/profile/storage";
import { missingFields, type Region, type StudentProfile } from "@/features/profile/types";

const REGIONS: Region[] = [
  "서울", "부산", "대구", "인천", "광주", "대전", "울산", "세종",
  "경기", "강원", "충북", "충남", "전북", "전남", "경북", "경남", "제주",
];

const LEVELS = ["입문", "초급", "중급", "고급"] as const;

export default function ProfileForm() {
  // 저장소에 있는 값. 아직 아무것도 안 고쳤으면 이걸 그대로 보여준다.
  const stored = useSyncExternalStore(subscribe, getSnapshot, getServerSnapshot);

  // 사용자가 고치기 시작하면 draft 가 화면의 주인이 된다.
  const [draft, setDraft] = useState<StudentProfile | null>(null);
  const [saved, setSaved] = useState(false);

  const profile = draft ?? parseProfile(stored);

  /** 일부 필드만 바꿔 새 프로필을 만든다. React 는 객체를 통째로 교체해야 화면을 다시 그린다. */
  function patch(changes: Partial<StudentProfile>) {
    setDraft({ ...profile, ...changes });
    setSaved(false);
  }

  function onSave() {
    setSaved(saveProfile(profile));
  }

  const missing = missingFields(profile);

  return (
    <div className="grid grid-cols-1 gap-6 lg:grid-cols-[1fr_330px]">
      {/* ── 입력 ───────────────────────────────────────────────── */}
      <div>
        <Group title="개인" description="나이·지역 제한이 걸린 공모전을 판정할 때 씁니다">
          <div className="grid grid-cols-1 gap-2.5 sm:grid-cols-2">
            <Field label="생년월일" required filled={!!profile.personal.birthDate}>
              <TextInput
                type="date"
                value={profile.personal.birthDate ?? ""}
                onChange={(e) =>
                  patch({ personal: { ...profile.personal, birthDate: e.target.value } })
                }
              />
            </Field>
            <Field label="거주 지역" required filled={!!profile.personal.region}>
              <Select
                value={profile.personal.region ?? ""}
                onChange={(e) =>
                  patch({
                    personal: {
                      ...profile.personal,
                      region: (e.target.value || undefined) as Region | undefined,
                    },
                  })
                }
              >
                <option value="">선택하세요</option>
                {REGIONS.map((r) => (
                  <option key={r} value={r}>{r}</option>
                ))}
              </Select>
            </Field>
          </div>
        </Group>

        <Group title="학적" description="재학생 한정·학년 제한 공모전 판정의 기준입니다">
          <div className="grid grid-cols-1 gap-2.5 sm:grid-cols-2">
            <Field label="대학" filled={!!profile.academic.university}>
              <TextInput
                value={profile.academic.university ?? ""}
                placeholder="한국폴리텍대학"
                onChange={(e) =>
                  patch({ academic: { ...profile.academic, university: e.target.value } })
                }
              />
            </Field>
            <Field label="학과" required filled={!!profile.academic.department}>
              <TextInput
                value={profile.academic.department ?? ""}
                placeholder="컴퓨터정보과"
                onChange={(e) =>
                  patch({ academic: { ...profile.academic, department: e.target.value } })
                }
              />
            </Field>
            <Field label="학년" required filled={profile.academic.gradeYear !== undefined}>
              <Select
                value={profile.academic.gradeYear ?? ""}
                onChange={(e) =>
                  patch({
                    academic: {
                      ...profile.academic,
                      gradeYear: e.target.value ? Number(e.target.value) : undefined,
                    },
                  })
                }
              >
                <option value="">선택하세요</option>
                {[1, 2, 3, 4].map((y) => (
                  <option key={y} value={y}>{y}학년</option>
                ))}
              </Select>
            </Field>
            <Field label="재학 여부" filled>
              <Select
                value={profile.academic.isEnrolled ? "재학" : "휴학"}
                onChange={(e) =>
                  patch({
                    academic: { ...profile.academic, isEnrolled: e.target.value === "재학" },
                  })
                }
              >
                <option value="재학">재학</option>
                <option value="휴학">휴학</option>
              </Select>
            </Field>
          </div>
        </Group>

        <Group
          title="보유 스킬"
          description="수준까지 있어야 '기획 역할로 지원하세요' 같은 구체적인 코칭이 나옵니다"
        >
          <div className="flex flex-col gap-2">
            {profile.skills.map((skill, i) => (
              <div key={i} className="flex items-center gap-2">
                <TextInput
                  value={skill.name}
                  placeholder="React, Figma, Python…"
                  onChange={(e) => {
                    const skills = [...profile.skills];
                    skills[i] = { ...skill, name: e.target.value };
                    patch({ skills });
                  }}
                />
                <div className="w-[110px] shrink-0">
                  <Select
                    value={skill.level}
                    onChange={(e) => {
                      const skills = [...profile.skills];
                      skills[i] = { ...skill, level: e.target.value as typeof LEVELS[number] };
                      patch({ skills });
                    }}
                  >
                    {LEVELS.map((l) => (
                      <option key={l} value={l}>{l}</option>
                    ))}
                  </Select>
                </div>
                <MiniButton
                  onClick={() => patch({ skills: profile.skills.filter((_, j) => j !== i) })}
                >
                  삭제
                </MiniButton>
              </div>
            ))}
            {profile.skills.length === 0 && (
              <p className="rounded-[2px] border border-line-warn bg-warn-surface p-3 text-[12px] text-fail-fg">
                스킬이 하나도 없으면 대부분의 공모전 판정이 보류됩니다.
              </p>
            )}
            <div>
              <MiniButton
                onClick={() =>
                  patch({ skills: [...profile.skills, { name: "", level: "초급" }] })
                }
              >
                ＋ 스킬 추가
              </MiniButton>
            </div>
          </div>
        </Group>

        <Group title="수상 · 참가 이력" description="코칭이 '이 경험을 어필하라'고 말할 근거가 됩니다">
          <div className="flex flex-col gap-2">
            {profile.awards.map((award, i) => (
              <div key={i} className="flex items-center gap-2">
                <TextInput
                  value={award.contestName}
                  placeholder="공모전 이름"
                  onChange={(e) => {
                    const awards = [...profile.awards];
                    awards[i] = { ...award, contestName: e.target.value };
                    patch({ awards });
                  }}
                />
                <div className="w-[100px] shrink-0">
                  <TextInput
                    value={award.prize ?? ""}
                    placeholder="장려상"
                    onChange={(e) => {
                      const awards = [...profile.awards];
                      awards[i] = { ...award, prize: e.target.value };
                      patch({ awards });
                    }}
                  />
                </div>
                <div className="w-[90px] shrink-0">
                  <TextInput
                    type="number"
                    value={award.year || ""}
                    placeholder="2026"
                    onChange={(e) => {
                      const awards = [...profile.awards];
                      awards[i] = { ...award, year: Number(e.target.value) };
                      patch({ awards });
                    }}
                  />
                </div>
                <MiniButton
                  onClick={() => patch({ awards: profile.awards.filter((_, j) => j !== i) })}
                >
                  삭제
                </MiniButton>
              </div>
            ))}
            <div>
              <MiniButton
                onClick={() =>
                  patch({
                    awards: [
                      ...profile.awards,
                      { contestName: "", year: new Date().getFullYear() },
                    ],
                  })
                }
              >
                ＋ 이력 추가
              </MiniButton>
            </div>
          </div>
        </Group>

        <Group title="그 밖에">
          <div className="flex flex-col gap-2.5">
            <Field
              label="관심 분야"
              hint="쉼표로 구분해서 적으세요"
              required
              filled={profile.interests.length > 0}
            >
              <TextInput
                value={profile.interests.join(", ")}
                placeholder="웹 개발, UI 디자인, 데이터 분석"
                onChange={(e) =>
                  patch({
                    interests: e.target.value
                      .split(",")
                      .map((s) => s.trim())
                      .filter(Boolean),
                  })
                }
              />
            </Field>

            <div className="grid grid-cols-1 gap-2.5 sm:grid-cols-2">
              <Field label="포트폴리오" filled={!!profile.links.portfolio}>
                <TextInput
                  value={profile.links.portfolio ?? ""}
                  placeholder="https://"
                  onChange={(e) =>
                    patch({ links: { ...profile.links, portfolio: e.target.value } })
                  }
                />
              </Field>
              <Field label="GitHub" filled={!!profile.links.github}>
                <TextInput
                  value={profile.links.github ?? ""}
                  placeholder="https://github.com/"
                  onChange={(e) =>
                    patch({ links: { ...profile.links, github: e.target.value } })
                  }
                />
              </Field>
            </div>

            <Field label="자기소개" hint="코칭할 때 그대로 참고합니다" filled={!!profile.bio}>
              <TextArea
                value={profile.bio ?? ""}
                placeholder="어떤 걸 해봤고, 어떤 공모전에 나가고 싶은지 편하게 적어주세요."
                onChange={(e) => patch({ bio: e.target.value })}
              />
            </Field>
          </div>
        </Group>

        <div className="mt-6 flex items-center gap-3">
          <button
            type="button"
            onClick={onSave}
            className="rounded-[2px] bg-ink px-5 py-3 text-[13.5px] font-semibold text-on-dark"
          >
            저장
          </button>
          {saved && <span className="text-[12.5px] text-ok-fg">저장했습니다</span>}
        </div>
      </div>

      {/* ── 우측: 무엇이 판정을 막고 있는지 ─────────────────────── */}
      <aside>
        {missing.length > 0 ? (
          <div className="rounded-[2px] border border-line-warn bg-warn-surface p-4">
            <p className="text-[13.5px] font-bold">비어 있는 항목이 판정을 막고 있습니다</p>
            <p className="mt-2 text-[12.5px] leading-relaxed text-[#6b584f]">
              아래 {missing.length}개를 채우면 공모전 자격 판정이 가능해집니다.
            </p>
            <ul className="mt-3 flex flex-wrap gap-1.5">
              {missing.map((f) => (
                <li
                  key={f}
                  className="rounded-[2px] bg-fail-bg px-2 py-1 text-[11.5px] text-fail-fg"
                >
                  {f}
                </li>
              ))}
            </ul>
          </div>
        ) : (
          <div className="rounded-[2px] border border-line-accent bg-accent-bg p-4">
            <p className="text-[13.5px] font-bold">판정에 필요한 항목이 모두 찼습니다</p>
            <p className="mt-2 text-[12.5px] leading-relaxed text-[#4a5544]">
              공모전을 수집하면 바로 자격 판정을 받을 수 있습니다.
            </p>
          </div>
        )}

        <p className="mt-3 rounded-[2px] border border-line p-4 text-[11.5px] leading-relaxed text-muted">
          입력한 값은 지금 <strong className="font-semibold text-body">이 브라우저에만</strong> 저장됩니다.
          로그인과 데이터베이스를 붙이면 기기가 바뀌어도 유지됩니다.
        </p>
      </aside>
    </div>
  );
}
