---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/export-patchwork.command.ts"
ref: HEAD
---

import { resolve } from "node:path";
import { Command } from "commander";
import { MapDataStorageService } from "../../services/map-data-storage.service";
import { PatchworkProjectExporter } from "../../services/patchwork-project-exporter";
import { WorkspaceService } from "../../services/workspace.service";

export function registerExportPatchworkCommand(program: Command): void {
  const workspaceService = new WorkspaceService();
  const mapDataStorageService = new MapDataStorageService();
  const patchworkExporter = new PatchworkProjectExporter();

  program
    .command("export-patchwork")
    .description("Export the current workspace project into Patchwork JSON files")
    .argument("<outputDirectory>", "Output directory for Patchwork JSON files")
    .action((outputDirectory: string) => {
      try {
        const currentProjectPath = workspaceService.getCurrentDataFile();
        const mapData = mapDataStorageService.loadMapDataFromFile(currentProjectPath);
        const absoluteOutputDirectory = resolve(process.cwd(), outputDirectory);

        patchworkExporter.exportToDirectory(mapData, absoluteOutputDirectory);
        console.log(`Exported Patchwork project to: ${absoluteOutputDirectory}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to export Patchwork project.";
        console.error(message);
      }
    });
}
