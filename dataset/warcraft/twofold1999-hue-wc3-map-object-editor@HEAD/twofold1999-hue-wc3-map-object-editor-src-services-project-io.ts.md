---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/project-io.ts"
ref: HEAD
---

import { MapDataFile } from "../models/map-data.model";

export interface ProjectImporter {
  importFromFile(sourcePath: string): MapDataFile;
}

export interface ProjectExporter {
  exportToFile(mapData: MapDataFile, outputPath: string): void;
}
