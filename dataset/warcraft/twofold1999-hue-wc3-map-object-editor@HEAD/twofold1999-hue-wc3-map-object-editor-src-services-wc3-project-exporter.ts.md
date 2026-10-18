---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/wc3-project-exporter.ts"
ref: HEAD
---

import { MapDataFile } from "../models/map-data.model";
import { ProjectExporter } from "./project-io";

// Future adapter for exporting .w3x / .w3m map data.
export class Wc3ProjectExporter implements ProjectExporter {
  public exportToFile(_mapData: MapDataFile, _outputPath: string): void {
    throw new Error("WC3 map export is not implemented yet.");
  }
}
