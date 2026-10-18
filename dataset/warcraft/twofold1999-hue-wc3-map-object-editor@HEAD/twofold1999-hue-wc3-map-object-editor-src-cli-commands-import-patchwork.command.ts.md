---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/import-patchwork.command.ts"
ref: HEAD
---

import { resolve } from "node:path";
import { Command } from "commander";
import { JsonProjectExporter } from "../../services/json-project-io";
import { PatchworkProjectImporter } from "../../services/patchwork-project-importer";
import { WorkspaceService } from "../../services/workspace.service";

export function registerImportPatchworkCommand(program: Command): void {
  const importer = new PatchworkProjectImporter();
  const exporter = new JsonProjectExporter();
  const workspaceService = new WorkspaceService();

  program
    .command("import-patchwork")
    .description("Import Patchwork output directory into workspace temp/project.json")
    .argument("<directoryPath>", "Path to Patchwork output directory")
    .action((directoryPath: string) => {
      try {
        const sourceDirPath = resolve(process.cwd(), directoryPath);
        const mapData = importer.importFromDirectory(sourceDirPath);
        const workspacePath = resolve(process.cwd(), "temp", "project.json");

        exporter.exportToFile(mapData, workspacePath);
        workspaceService.setCurrentDataFile(workspacePath, sourceDirPath);

        console.log(`Imported project into workspace: ${workspacePath}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to import Patchwork project.";
        console.error(message);
      }
    });
}
