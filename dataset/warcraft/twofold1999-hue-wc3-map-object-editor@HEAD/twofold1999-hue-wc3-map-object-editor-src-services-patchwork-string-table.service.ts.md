---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-string-table.service.ts"
ref: HEAD
---

import { existsSync, readFileSync } from "node:fs";
import { resolve } from "node:path";

export type PatchworkStringTable = Record<string, string>;

export class PatchworkStringTableService {
  public loadFromDirectory(
    directoryPath: string
  ): PatchworkStringTable | undefined {
    const filePath = resolve(directoryPath, "war3map.wts.json");

    if (!existsSync(filePath)) {
      return undefined;
    }

    let parsed: unknown;
    try {
      parsed = JSON.parse(readFileSync(filePath, "utf-8")) as unknown;
    } catch {
      throw new Error(`Invalid Patchwork JSON file: ${filePath}`);
    }

    if (!parsed || typeof parsed !== "object" || Array.isArray(parsed)) {
      throw new Error(
        `Invalid Patchwork WTS JSON: expected root object in ${filePath}`
      );
    }

    const table: PatchworkStringTable = {};
    for (const [key, value] of Object.entries(parsed as Record<string, unknown>)) {
      if (typeof value === "string") {
        table[key] = value;
      }
    }

    return table;
  }
}
