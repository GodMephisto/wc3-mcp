---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/json-project-io.ts"
ref: HEAD
---

import { mkdirSync, writeFileSync } from "node:fs";
import { dirname } from "node:path";
import { MapDataFile } from "../models/map-data.model";
import { MapDataStorageService } from "./map-data-storage.service";
import { ProjectExporter, ProjectImporter } from "./project-io";

export class JsonProjectImporter implements ProjectImporter {
  private readonly mapDataStorageService = new MapDataStorageService();

  public importFromFile(sourcePath: string): MapDataFile {
    return this.mapDataStorageService.loadMapDataFromFile(sourcePath);
  }
}

export class JsonProjectExporter implements ProjectExporter {
  public exportToFile(mapData: MapDataFile, outputPath: string): void {
    mkdirSync(dirname(outputPath), { recursive: true });
    writeFileSync(outputPath, `${JSON.stringify(mapData, null, 2)}\n`, "utf-8");
  }
}
