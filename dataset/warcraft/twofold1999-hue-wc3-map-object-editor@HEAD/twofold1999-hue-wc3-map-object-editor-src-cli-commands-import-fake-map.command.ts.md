---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/import-fake-map.command.ts"
ref: HEAD
---

import { resolve } from "node:path";
import { Command } from "commander";
import { JsonProjectExporter } from "../../services/json-project-io";
import { Wc3ProjectImporter } from "../../services/wc3-project-importer";
import { WorkspaceService } from "../../services/workspace.service";

export function registerImportFakeMapCommand(program: Command): void {
  const importer = new Wc3ProjectImporter();
  const exporter = new JsonProjectExporter();
  const workspaceService = new WorkspaceService();

  program
    .command("import-fake-map")
    .description("Development command to import a map through fake WC3 translator")
    .argument("<mapPath>", "Path to input map file")
    .action((mapPath: string) => {
      try {
        const sourcePath = resolve(process.cwd(), mapPath);
        const mapData = importer.importFromFile(sourcePath);
        const workspacePath = resolve(process.cwd(), "temp", "project.json");

        exporter.exportToFile(mapData, workspacePath);
        workspaceService.setCurrentDataFile(workspacePath);

        console.log(`Imported project into workspace: ${workspacePath}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to import fake map.";
        console.error(message);
      }
    });
}
