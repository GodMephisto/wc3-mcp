---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/import-patchwork-dir.command.ts"
ref: HEAD
---

import { mkdirSync } from "node:fs";
import { resolve } from "node:path";
import { spawnSync } from "node:child_process";
import { Command } from "commander";
import { JsonProjectExporter } from "../../services/json-project-io";
import { PatchworkProjectImporter } from "../../services/patchwork-project-importer";
import { WorkspaceService } from "../../services/workspace.service";

export function registerImportPatchworkDirCommand(program: Command): void {
  const importer = new PatchworkProjectImporter();
  const exporter = new JsonProjectExporter();
  const workspaceService = new WorkspaceService();

  program
    .command("import-patchwork-dir")
    .description("Run Patchwork conversion and import result into workspace")
    .argument("<inputDir>", "Input directory for patchwork-mapconverter")
    .action((inputDir: string) => {
      try {
        const inputDirPath = resolve(process.cwd(), inputDir);
        const patchworkOutputDir = resolve(
          process.cwd(),
          "temp",
          "patchwork-import"
        );
        const workspacePath = resolve(process.cwd(), "temp", "project.json");

        mkdirSync(patchworkOutputDir, { recursive: true });

        const patchworkArgs = [
          "patchwork-mapconverter",
          "war2json",
          inputDirPath,
          patchworkOutputDir,
          "-d",
        ];
        const isWindows = process.platform === "win32";
        const result = spawnSync(
          isWindows ? "cmd" : "npx",
          isWindows ? ["/c", "npx", ...patchworkArgs] : patchworkArgs,
          {
            encoding: "utf-8",
            stdio: "pipe",
          }
        );

        if (result.error) {
          console.error(
            `Failed to run patchwork-mapconverter: ${result.error.message}`
          );
          return;
        }

        if (result.status !== 0) {
          const stderr =
            typeof result.stderr === "string" && result.stderr.trim().length > 0
              ? result.stderr.trim()
              : "No stderr output.";
          console.error(
            `patchwork-mapconverter failed with status ${result.status}: ${stderr}`
          );
          return;
        }

        const mapData = importer.importFromDirectory(patchworkOutputDir);
        exporter.exportToFile(mapData, workspacePath);
        workspaceService.setCurrentDataFile(workspacePath, patchworkOutputDir);

        console.log(`Imported project into workspace: ${workspacePath}`);
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "Failed to import Patchwork directory.";
        console.error(message);
      }
    });
}
