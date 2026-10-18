---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/sections/CounterSection.tsx"
ref: HEAD
---

import { useCount, increment, decrement, reset } from '../../state/counter';


interface Props {
  counterPressed: boolean;
  counterCanvasRef: React.RefObject<HTMLCanvasElement | null>;
  onCounterMouseEnter: () => void;
  onCounterMouseLeave: () => void;
  onCounterMouseDown: () => void;
  onCounterMouseUp: () => void;
}

export default function CounterSection(props: Props) {
const count = useCount();

  return (
    <section className="section-card counter-section">
      <h2>Counter</h2>
      <p className="counter-value">{count}</p>
      <div className="counter-buttons">
        <button
          type="button"
          className="esc-option-preview counter-btn"
          onMouseEnter={props.onCounterMouseEnter}
          onMouseLeave={props.onCounterMouseLeave}
          onMouseDown={() => {
            props.onCounterMouseDown();
            increment;
          }}
          onMouseUp={props.onCounterMouseUp}
        >
          <canvas ref={props.counterCanvasRef} className="esc-option-canvas" />
          <span
            className="esc-option-label"
            style={{
              transform: props.counterPressed ? "translate(2px, 2px)" : "none",
            }}
          >
            Plus 1
          </span>
        </button>
      </div>
    </section>
  );
}
