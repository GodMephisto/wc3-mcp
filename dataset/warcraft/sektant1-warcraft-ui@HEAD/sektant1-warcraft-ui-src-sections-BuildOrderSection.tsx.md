---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/BuildOrderSection.tsx"
ref: HEAD
---

import { useCallback, useEffect, useRef, useState } from "react";
import type { BuildStep } from "../../data/buildOrders";
import {
  useTimerRunning,
  useElapsedSeconds,
  startTimer,
  pauseTimer,
  resetTimer,
} from "../../state/timer";
import { useCurrentRace, setCurrentRace } from "../../state/race";
import type { Race } from "../../lib/types";
import GlueSmallButton from "../GlueSmallButton";
import GlueBorderedButton from "../GlueBorderedButton";
import GlueDropdown from "../GlueDropdown";
import GlueScrollbar from "../GlueScrollbar";
import BuildCheckbox from "../BuildCheckbox";
import { useBuildOrders, importBuilds } from "../../state/buildOrders";
import type { BuildOrder } from "../../data/buildOrders";
import {
  getPendingBuildSelect,
  clearPendingBuildSelect,
} from "../../state/pendingBuildSelect";

const RACES: Race[] = ["Human", "Orc", "NightElf", "Undead"];
const RACE_LABELS: Record<Race, string> = {
  Human: "Human",
  Orc: "Orc",
  NightElf: "Night Elf",
  Undead: "Undead",
};
import {
  loadBlpDataUrl,
  loadDataUrlImage,
  composeBorderImageFromAtlas,
} from "../../utils/glueButton";
import HeroIconImg from "../HeroIconImg";
import SkillOrderSection from "../SkillOrderSection";
import { setPage } from "../../state/page";
import FooterSection from "./FooterSection";

function formatTime(seconds: number): string {
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, "0")}`;
}

function parseStepTime(time: string): number {
  const [m, s] = time.split(":").map(Number);
  return m * 60 + (s ?? 0);
}

function getCardBorderPath(race: Race | null): string {
  if (race === "Orc") return "./buttons/esc/orc/orc-options-menu-border.blp";
  if (race === "NightElf")
    return "./buttons/esc/nightelf/nightelf-options-menu-border.blp";
  if (race === "Undead")
    return "./buttons/esc/undead/undead-options-menu-border.blp";
  return "./buttons/esc/human/human-options-menu-border.blp";
}

function useCardBorderImage(race: Race | null): string {
  const [borderImageUrl, setBorderImageUrl] = useState("");

  useEffect(() => {
    let cancelled = false;
    const path = getCardBorderPath(race);
    void loadBlpDataUrl(path)
      .then((url) => loadDataUrlImage(url))
      .then((img) => {
        if (cancelled) return;
        setBorderImageUrl(composeBorderImageFromAtlas(img));
      })
      .catch((err) => console.error(err));
    return () => {
      cancelled = true;
    };
  }, [race]);

  return borderImageUrl;
}

function useSupplyIcon(): string {
  const [url, setUrl] = useState("");
  useEffect(() => {
    void loadBlpDataUrl("./resources/ResourceSupply.blp").then(setUrl);
  }, []);
  return url;
}

function playSound(name: string) {
  const audio = new Audio(`./sound/${name}.wav`);
  audio.play().catch(() => {
    if (name !== "key_moment") {
      new Audio("./sound/key_moment.wav").play().catch(() => {});
    }
  });
}

function KeyMomentToast({
  step,
  onDismiss,
}: {
  step: BuildStep;
  onDismiss: () => void;
}) {
  useEffect(() => {
    const t = setTimeout(onDismiss, 4500);
    return () => clearTimeout(t);
  }, [onDismiss]);

  return (
    <div className="key-moment-toast" onClick={onDismiss}>
      <span className="key-moment-label">⚡ Key Moment</span>
      <span className="key-moment-text">{step.instruction}</span>
      <span className="key-moment-time">{step.time}</span>
    </div>
  );
}

const DIFFICULTY_CLASS: Record<string, string> = {
  Beginner: "build-card-difficulty--beginner",
  Intermediate: "build-card-difficulty--intermediate",
  Advanced: "build-card-difficulty--advanced",
};


interface BuildCardProps {
  build: BuildOrder;
  elapsed: number;
  running: boolean;
  borderImageUrl: string;
}

function BuildCard({
  build,
  elapsed,
  running,
  borderImageUrl,
}: BuildCardProps) {
  const tableRef = useRef<HTMLDivElement>(null);
  const [scrollPct, setScrollPct] = useState(0);
  const [containerHalf, setContainerHalf] = useState(0);
  const [centerScroll, setCenterScroll] = useState(true);
  const [keyToast, setKeyToast] = useState<BuildStep | null>(null);
  const [toastKey, setToastKey] = useState(0);
  const scrollingFromBar = useRef(false);
  const prevActiveRef = useRef(-1);
  const supplyIconUrl = useSupplyIcon();

  const activeStepIndex = (() => {
    let idx = -1;
    for (let i = 0; i < build.steps.length; i++) {
      if (parseStepTime(build.steps[i].time) <= elapsed) idx = i;
    }
    return idx;
  })();

  // Measure container height for centering spacers
  useEffect(() => {
    const el = tableRef.current;
    if (!el) return;
    const ro = new ResizeObserver(() => setContainerHalf(el.clientHeight / 2));
    ro.observe(el);
    return () => ro.disconnect();
  }, []);

  // Sync native scroll → scrollbar percentage
  useEffect(() => {
    const el = tableRef.current;
    if (!el) return;
    const handler = () => {
      if (scrollingFromBar.current) return;
      const maxScroll = el.scrollHeight - el.clientHeight;
      if (maxScroll <= 0) {
        setScrollPct(0);
        return;
      }
      setScrollPct((el.scrollTop / maxScroll) * 100);
    };
    el.addEventListener("scroll", handler, { passive: true });
    return () => el.removeEventListener("scroll", handler);
  }, []);

  // Scrollbar → native scroll
  const handleScrollbarChange = useCallback((pct: number) => {
    setScrollPct(pct);
    const el = tableRef.current;
    if (!el) return;
    const maxScroll = el.scrollHeight - el.clientHeight;
    if (maxScroll <= 0) return;
    scrollingFromBar.current = true;
    el.scrollTop = (pct / 100) * maxScroll;
    requestAnimationFrame(() => {
      scrollingFromBar.current = false;
    });
  }, []);

  // Clear toast on reset
  useEffect(() => {
    if (elapsed === 0) {
      setKeyToast(null);
      prevActiveRef.current = -1;
    }
  }, [elapsed]);

  // Key moment trigger
  useEffect(() => {
    const prev = prevActiveRef.current;
    prevActiveRef.current = activeStepIndex;
    if (activeStepIndex > prev && activeStepIndex >= 0) {
      const step = build.steps[activeStepIndex];
      if (step?.isKey) {
        setKeyToast(step);
        setToastKey((k) => k + 1);
        playSound(step.sound ?? "key_moment");
      }
    }
  }, [activeStepIndex, build.steps]);

  // Auto-scroll active step
  useEffect(() => {
    if (activeStepIndex < 0 || !tableRef.current) return;
    const el = tableRef.current;
    const row = el.querySelectorAll(".build-step-row")[activeStepIndex] as
      | HTMLElement
      | undefined;
    if (!row) return;
    if (centerScroll) {
      const rowRect = row.getBoundingClientRect();
      const elRect = el.getBoundingClientRect();
      const targetScrollTop =
        el.scrollTop + (rowRect.top - elRect.top) - el.clientHeight / 2 + row.offsetHeight / 2;
      el.scrollTo({ top: targetScrollTop, behavior: "smooth" });
    } else {
      row.scrollIntoView({ block: "nearest", behavior: "smooth" });
    }
  }, [activeStepIndex, centerScroll]);

  // border-image applied only to the active row; all rows already reserve
  // 6px transparent border space in CSS so nothing shifts.
  const activeBorderStyle: React.CSSProperties | undefined = borderImageUrl
    ? {
        borderImageSource: `url("${borderImageUrl}")`,
        borderImageSlice: 64,
        borderImageWidth: 1,
        borderImageRepeat: "round",
      }
    : undefined;

  return (
    <div className="build-card">
      {/* ── Full-width header banner ── */}
      <div className="build-card-header">
        {/* Heroes column — 1st hero prominent, rest secondary, fallback if none */}
        <div className="build-card-heroes-col">
          {build.heroes && build.heroes.length > 0 ? (
            <>
              <div className="build-card-hero-primary">
                <HeroIconImg
                  heroName={build.heroes[0]}
                  size={44}
                  className="hero-icon--primary"
                />
                <span className="build-card-hero-name">{build.heroes[0]}</span>
              </div>
              {build.heroes.slice(1).map((h) => (
                <div key={h} className="build-card-hero-secondary">
                  <HeroIconImg
                    heroName={h}
                    size={28}
                    className="hero-icon--secondary"
                  />
                  <span className="build-card-hero-name build-card-hero-name--secondary">
                    {h}
                  </span>
                </div>
              ))}
            </>
          ) : (
            <div className="build-card-hero-none">
              <HeroIconImg size={36} className="hero-icon--fallback" />
              <span className="build-card-hero-name build-card-hero-name--muted">
                Any Hero
              </span>
            </div>
          )}
        </div>
        <div className="build-card-header-main">
          <h4 className="build-card-title">{build.name}</h4>
          <div className="build-card-badges">
            {build.vs && (
              <span className="build-card-matchup">vs {build.vs}</span>
            )}
            {build.difficulty && (
              <span
                className={`build-card-difficulty ${DIFFICULTY_CLASS[build.difficulty] ?? ""}`}
              >
                {build.difficulty}
              </span>
            )}
          </div>
        </div>

        {/* Timer — right-most element in header */}
        <div className="build-card-timer">
          <span className="build-card-elapsed">{formatTime(elapsed)}</span>
          <div className="build-card-controls">
            <GlueSmallButton onClick={running ? pauseTimer : startTimer}>
              {running ? "Pause" : "Start"}
            </GlueSmallButton>
            <GlueSmallButton
              onClick={resetTimer}
              disabled={elapsed === 0 && !running}
            >
              Reset
            </GlueSmallButton>
          </div>
          <BuildCheckbox
            checked={centerScroll}
            onChange={setCenterScroll}
            label="Center Scroll"
          />
        </div>
      </div>

      {/* Key moment slot — always present to reserve space */}
      <div className="key-moment-slot">
        {keyToast && (
          <KeyMomentToast
            key={toastKey}
            step={keyToast}
            onDismiss={() => setKeyToast(null)}
          />
        )}
      </div>

      {/* Steps — full width */}
      <div className="build-card-steps-wrapper">
        <div className="build-card-steps" ref={tableRef}>
          <table className="build-steps-table">
            <thead>
              <tr>
                <th>#</th>
                <th>Time</th>
                <th>Instruction</th>
                <th>
                  {supplyIconUrl && (
                    <img
                      src={supplyIconUrl}
                      alt=""
                      className="build-step-food-icon"
                    />
                  )}
                  Food
                </th>
              </tr>
            </thead>
            <tbody>
              {centerScroll && (
                <tr aria-hidden="true" style={{ height: containerHalf }}>
                  <td colSpan={4} />
                </tr>
              )}
              {build.steps.map((step, j) => {
                const isActive = j === activeStepIndex;
                const isPast = j < activeStepIndex;
                return (
                  <tr
                    key={j}
                    className={`build-step-row${isPast ? " build-step-row--past" : ""}${isActive ? " build-step-row--active" : ""}`}
                    style={isActive ? activeBorderStyle : undefined}
                  >
                    <td className="build-step-order">{step.order}</td>
                    <td className="build-step-time">{step.time}</td>
                    <td className="build-step-instruction">
                      {step.instruction}
                    </td>
                    <td className="build-step-food">
                      {supplyIconUrl && (
                        <img
                          src={supplyIconUrl}
                          alt=""
                          className="build-step-food-icon"
                        />
                      )}
                      {step.food}
                    </td>
                  </tr>
                );
              })}
              {centerScroll && (
                <tr aria-hidden="true" style={{ height: containerHalf }}>
                  <td colSpan={4} />
                </tr>
              )}
            </tbody>
          </table>
        </div>
        <GlueScrollbar value={scrollPct} onChange={handleScrollbarChange} />
      </div>
    </div>
  );
}

export default function BuildOrdersSection() {
  const running = useTimerRunning();
  const elapsed = useElapsedSeconds();
  const currentRace = useCurrentRace();
  const borderImageUrl = useCardBorderImage(currentRace);
  const allBuilds = useBuildOrders();

  const relevantBuilds = allBuilds.filter(
    (b) => !currentRace || b.race === currentRace,
  );

  const [selectedIndex, setSelectedIndex] = useState(0);
  const [importStatus, setImportStatus] = useState("");
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Apply pending build selection from the home page list
  useEffect(() => {
    const pending = getPendingBuildSelect();
    if (!pending) return;
    clearPendingBuildSelect();
    setCurrentRace(pending.race as Race);
    const idx = allBuilds
      .filter((b) => b.race === pending.race)
      .findIndex((b) => b.name === pending.name);
    if (idx >= 0) {
      setSelectedIndex(idx);
      resetTimer();
    }
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  // Reset selection and timer when race changes
  useEffect(() => {
    setSelectedIndex(0);
    resetTimer();
  }, [currentRace]);

  const selectedBuild =
    relevantBuilds.length > 0
      ? relevantBuilds[Math.min(selectedIndex, relevantBuilds.length - 1)]
      : null;

  // Build dropdown option keys — use indices as stable identifiers
  const optionKeys = relevantBuilds.map((_, i) => String(i));

  const handleImport = useCallback(() => {
    fileInputRef.current?.click();
  }, []);

  const handleExport = useCallback(() => {
    if (!selectedBuild) return;
    // Build a clean object with properties ordered to match the BuildOrder interface
    const clean: Record<string, unknown> = {
      name: selectedBuild.name,
      race: selectedBuild.race,
    };
    if (selectedBuild.vs) clean.vs = selectedBuild.vs;
    clean.description = selectedBuild.description;
    if (selectedBuild.difficulty) clean.difficulty = selectedBuild.difficulty;
    clean.steps = selectedBuild.steps;
    if (selectedBuild.note) clean.note = selectedBuild.note;
    if (selectedBuild.skillOrder && selectedBuild.skillOrder.length > 0) {
      clean.skillOrder = selectedBuild.skillOrder;
    }
    const json = JSON.stringify([clean], null, 2);
    const blob = new Blob([json], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `${selectedBuild.name.replace(/[^a-zA-Z0-9_-]/g, "_")}.json`;
    a.click();

    URL.revokeObjectURL(url);
  }, [selectedBuild]);

  const handleFileChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files?.[0];
      if (!file) return;

      const reader = new FileReader();
      reader.onload = () => {
        try {
          const json = JSON.parse(reader.result as string);
          const count = importBuilds(json);
          if (count > 0) {
            setImportStatus(`Imported ${count} build${count > 1 ? "s" : ""}`);
          } else {
            setImportStatus("No valid builds found in file");
          }
        } catch {
          setImportStatus("Invalid JSON file");
        }
        // Clear so the same file can be re-imported
        e.target.value = "";
        setTimeout(() => setImportStatus(""), 4000);
      };
      reader.readAsText(file);
    },
    [],
  );

  return (
    <div className="tab-content">
      <section className="section-card build-orders-section">
        <div className="build-orders-toolbar">
          <h3>Build Orders</h3>
          <div className="build-orders-toolbar-actions">
            <div className="build-orders-race-picker">
              <GlueDropdown
                options={RACES}
                value={currentRace}
                onChange={(v) => setCurrentRace(v as Race)}
                label={(v) => RACE_LABELS[v as Race] ?? v}
              />
            </div>
            {relevantBuilds.length > 1 && (
              <GlueDropdown
                options={optionKeys}
                value={String(
                  Math.min(selectedIndex, relevantBuilds.length - 1),
                )}
                onChange={(v) => {
                  setSelectedIndex(Number(v));
                  resetTimer();
                }}
                label={(v) => relevantBuilds[Number(v)]?.name ?? ""}
              />
            )}
            <GlueBorderedButton onClick={handleImport}>
              Import
            </GlueBorderedButton>
            <GlueBorderedButton
              onClick={handleExport}
              disabled={!selectedBuild}
            >
              Export
            </GlueBorderedButton>
            <input
              ref={fileInputRef}
              type="file"
              accept=".json"
              className="build-import-input"
              onChange={handleFileChange}
            />
          </div>
        </div>
        {importStatus && (
          <div className="build-import-status">{importStatus}</div>
        )}

        {!selectedBuild ? (
          <p className="build-card-empty">
            No build orders available for this race.
          </p>
        ) : (
          <>
            <BuildCard
              key={`${currentRace}-${selectedIndex}`}
              build={selectedBuild}
              elapsed={elapsed}
              running={running}
              borderImageUrl={borderImageUrl}
            />
            {/* {selectedBuild.skillOrder && */}
            {/*   selectedBuild.skillOrder.length > 0 && ( */}
            {/*     <SkillOrderSection skillOrder={selectedBuild.skillOrder} /> */}
            {/*   )} */}
            <div className="build-create-cta">
              <GlueBorderedButton onClick={() => setPage("create-build")}>
                + Create Build Order
              </GlueBorderedButton>
            </div>
          </>
        )}
      </section>
    </div>
  );
}
