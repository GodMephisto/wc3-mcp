---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/context/RendererContext.tsx"
ref: HEAD
---

import type { Race } from "../utils/types";
import { setCurrentRace } from "../state/race";
import { useEffect } from "react";

/**
 * @deprecated Components are now self-contained.
 * WarcraftRenderer is kept for backward compatibility — it just sets the active race.
 */
export function useRenderer() {
  console.warn(
    "useRenderer() is deprecated. Components are now self-contained and no longer need a renderer.",
  );
  return null;
}

interface Props {
  race: Race;
  children: React.ReactNode;
}

/**
 * @deprecated Components are now self-contained and render their own textures.
 * WarcraftRenderer is kept for backward compatibility — it just sets the active race
 * and renders children directly.
 */
export function WarcraftRenderer({ race, children }: Props) {
  useEffect(() => {
    setCurrentRace(race);
  }, [race]);

  return <>{children}</>;
}
