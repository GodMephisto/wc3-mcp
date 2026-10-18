---
type: vendor-doc
source_repo: "https://github.com/voces/w3xdata"
source_path: "src/mapUnitSpecs.ts"
ref: HEAD
---

import type { UnitSpec } from "wc3data";
import { units as baseUnits } from "wc3data";

import type { W3uW3tInput } from "./util";
import { applyDataFile } from "./util";

export const mapUnitSpecs = (
  w3u: W3uW3tInput,
  w3uSkin?: W3uW3tInput,
): Record<string, UnitSpec> => {
  const units = structuredClone(baseUnits);
  applyDataFile(units, w3u, "unit");
  if (w3uSkin) applyDataFile(units, w3uSkin, "unit");
  return units;
};
