---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-project-importer.ts"
ref: HEAD
---

import { existsSync, readFileSync } from "node:fs";
import { resolve } from "node:path";
import { MapDataFile } from "../models/map-data.model";
import { PatchworkAbilitiesAdapter } from "./patchwork-abilities-adapter";
import { PatchworkItemsAdapter } from "./patchwork-items-adapter";
import { PatchworkStringTableService } from "./patchwork-string-table.service";
import { PatchworkUnitsAdapter } from "./patchwork-units-adapter";
import { StringResolverService } from "./string-resolver.service";

export class PatchworkProjectImporter {
  private readonly stringTableService = new PatchworkStringTableService();

  public importFromDirectory(directoryPath: string): MapDataFile {
    const unitsJson = this.readJsonFile(directoryPath, "war3map.w3u.json");
    const abilitiesJson = this.readJsonFile(directoryPath, "war3map.w3a.json");
    const itemsJson = this.readJsonFile(directoryPath, "war3map.w3t.json");
    const stringTable = this.stringTableService.loadFromDirectory(directoryPath);
    const stringResolver = new StringResolverService(stringTable);
    const unitsAdapter = new PatchworkUnitsAdapter(stringResolver);
    const abilitiesAdapter = new PatchworkAbilitiesAdapter(stringResolver);
    const itemsAdapter = new PatchworkItemsAdapter(stringResolver);

    const units = unitsAdapter.mapToUnits(unitsJson);
    const abilities = abilitiesAdapter.mapToAbilities(abilitiesJson);
    const items = itemsAdapter.mapToItems(itemsJson);

    return {
      meta: {
        source: "patchwork",
        version: "1.0",
      },
      units,
      abilities,
      items,
    };
  }

  private readJsonFile(directoryPath: string, filename: string): unknown {
    const filePath = resolve(directoryPath, filename);

    if (!existsSync(filePath)) {
      throw new Error(`Patchwork file not found: ${filePath}`);
    }

    try {
      const raw = readFileSync(filePath, "utf-8");
      return JSON.parse(raw) as unknown;
    } catch {
      throw new Error(`Invalid Patchwork JSON file: ${filePath}`);
    }
  }
}
