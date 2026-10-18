---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/import-project.command.ts"
ref: HEAD
---

import { copyFileSync, existsSync, mkdirSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { Command } from "commander";
import { MapDataStorageService } from "../../services/map-data-storage.service";
import { WorkspaceService } from "../../services/workspace.service";

export function registerImportProjectCommand(program: Command): void {
  const mapDataStorageService = new MapDataStorageService();
  const workspaceService = new WorkspaceService();

  program
    .command("import-project")
    .description("Import an external map-data file into workspace temp/project.json")
    .argument("<filePath>", "Path to source map-data JSON file")
    .action((filePath: string) => {
      try {
        const sourcePath = resolve(process.cwd(), filePath);

        if (!existsSync(sourcePath)) {
          console.error(`Data file not found: ${sourcePath}`);
          return;
        }

        mapDataStorageService.validateMapDataFile(sourcePath);

        const workspacePath = resolve(process.cwd(), "temp", "project.json");
        mkdirSync(dirname(workspacePath), { recursive: true });
        copyFileSync(sourcePath, workspacePath);

        workspaceService.setCurrentDataFile(workspacePath);
        console.log(`Imported project into workspace: ${workspacePath}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to import project.";
        console.error(message);
      }
    });
}
