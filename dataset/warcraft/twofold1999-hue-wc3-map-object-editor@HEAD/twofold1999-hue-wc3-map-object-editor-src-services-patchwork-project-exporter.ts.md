---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/patchwork-project-exporter.ts"
ref: HEAD
---

import { mkdirSync, writeFileSync } from "node:fs";
import { resolve } from "node:path";
import { MapDataFile } from "../models/map-data.model";
import { PatchworkAbilitiesExporter } from "./patchwork-abilities-exporter";
import { PatchworkItemsExporter } from "./patchwork-items-exporter";
import { PatchworkUnitsExporter } from "./patchwork-units-exporter";

export class PatchworkProjectExporter {
  private readonly unitsExporter = new PatchworkUnitsExporter();
  private readonly abilitiesExporter = new PatchworkAbilitiesExporter();
  private readonly itemsExporter = new PatchworkItemsExporter();

  public exportToDirectory(mapData: MapDataFile, outputDirectory: string): void {
    const unitsJson = this.unitsExporter.export(mapData.units);
    const abilitiesJson = this.abilitiesExporter.export(mapData.abilities ?? []);
    const itemsJson = this.itemsExporter.export(mapData.items ?? []);

    mkdirSync(outputDirectory, { recursive: true });

    this.writeJsonFile(outputDirectory, "war3map.w3u.json", unitsJson);
    this.writeJsonFile(outputDirectory, "war3map.w3a.json", abilitiesJson);
    this.writeJsonFile(outputDirectory, "war3map.w3t.json", itemsJson);
  }

  private writeJsonFile(
    directoryPath: string,
    filename: string,
    value: unknown
  ): void {
    const filePath = resolve(directoryPath, filename);
    writeFileSync(filePath, `${JSON.stringify(value, null, 2)}\n`, "utf-8");
  }
}
