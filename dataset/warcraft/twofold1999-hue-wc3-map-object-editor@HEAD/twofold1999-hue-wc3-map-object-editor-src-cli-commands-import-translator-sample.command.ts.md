---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/import-translator-sample.command.ts"
ref: HEAD
---

import { resolve } from "node:path";
import { Command } from "commander";
import { JsonProjectExporter } from "../../services/json-project-io";
import { Wc3ProjectImporter } from "../../services/wc3-project-importer";
import { WorkspaceService } from "../../services/workspace.service";

export function registerImportTranslatorSampleCommand(program: Command): void {
  const importer = new Wc3ProjectImporter();
  const exporter = new JsonProjectExporter();
  const workspaceService = new WorkspaceService();

  program
    .command("import-translator-sample")
    .description("Import translator-style JSON into workspace temp/project.json")
    .argument("<jsonPath>", "Path to translator output JSON file")
    .action((jsonPath: string) => {
      try {
        const sourcePath = resolve(process.cwd(), jsonPath);
        const mapData = importer.importFromTranslatorJson(sourcePath);
        const workspacePath = resolve(process.cwd(), "temp", "project.json");

        exporter.exportToFile(mapData, workspacePath);
        workspaceService.setCurrentDataFile(workspacePath);

        console.log(`Imported project into workspace: ${workspacePath}`);
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "Failed to import translator sample.";
        console.error(message);
      }
    });
}
