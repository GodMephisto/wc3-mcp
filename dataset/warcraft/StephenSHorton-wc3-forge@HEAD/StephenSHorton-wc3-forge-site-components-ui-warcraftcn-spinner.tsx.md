---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/spinner.tsx"
ref: HEAD
---

import type * as React from "react";

import { cn } from "@/lib/utils";

import { SPINNER_PATH } from "@/components/ui/warcraftcn/assets/spinner-path";
import "@/components/ui/warcraftcn/styles/warcraft.css";

const SPINNER_VIEW_BOX = "14 15.946284 187.21483 333.9404";
const SPINNER_CENTER_X = 107.607415;
const SPINNER_CENTER_Y = 182.916484;
const SPINNER_STATIC_TRANSFORM = `translate(${SPINNER_CENTER_X} ${SPINNER_CENTER_Y}) rotate(-2) translate(${-SPINNER_CENTER_X} ${-SPINNER_CENTER_Y})`;

/** Renders the Warcraft-themed summoning glyph loading spinner. */
function Spinner({ className, ...props }: React.ComponentProps<"svg">) {
  return (
    <svg
      data-slot="spinner"
      role="status"
      aria-label="Loading"
      className={cn("wc-spinner size-10", className)}
      viewBox={SPINNER_VIEW_BOX}
      preserveAspectRatio="none"
      xmlns="http://www.w3.org/2000/svg"
      {...props}
    >
      <g transform={SPINNER_STATIC_TRANSFORM}>
        <g className="wc-spinner-wrap">
          <path
            className="wc-spinner-glow"
            d={SPINNER_PATH}
            fill="currentColor"
          />
          <path
            className="wc-spinner-core"
            d={SPINNER_PATH}
            fill="currentColor"
          />
        </g>
      </g>
    </svg>
  );
}

export { Spinner };
