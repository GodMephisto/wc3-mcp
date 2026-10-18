---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/utils/types.ts"
ref: HEAD
---

export type Race = "Human" | "Orc" | "NightElf" | "Undead";

export interface RacePrefix {
  tile: string;
  esc: string;
  lower: string;
  display: string;
}

export interface TextureEntry {
  tex: WebGLTexture;
  width: number;
  height: number;
  loaded: boolean;
}
