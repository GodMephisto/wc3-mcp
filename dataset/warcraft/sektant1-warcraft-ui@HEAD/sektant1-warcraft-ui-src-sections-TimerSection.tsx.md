---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/TimerSection.tsx"
ref: HEAD
---

import {
  useTimerRunning,
  useElapsedSeconds,
  startTimer,
  pauseTimer,
  resetTimer,
} from "../../state/timer";

function formatTime(seconds: number): string {
  const m = Math.floor(seconds / 60);
  const s = seconds % 60;
  return `${m}:${s.toString().padStart(2, "0")}`;
}

export default function TimerSection() {
  const running = useTimerRunning();
  const elapsed = useElapsedSeconds();
  return (
    <section className="section-card timer-section">
      <h3>Timer</h3>
      <div className="timer-display">{formatTime(elapsed)}</div>
      <div className="timer-controls">
        {running ? (
          <button
            type="button"
            className="esc-option-preview"
            onClick={pauseTimer}
          >
            <span className="esc-option-label">Pause</span>
          </button>
        ) : (
          <button
            type="button"
            className="esc-option-preview"
            onClick={startTimer}
          >
            <span className="esc-option-label">Start</span>
          </button>
        )}
        <button
          type="button"
          className="esc-option-preview"
          onClick={resetTimer}
        >
          <span className="esc-option-label">Reset</span>
        </button>
      </div>
    </section>
  );
}
