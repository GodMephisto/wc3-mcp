---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/export-project.command.ts"
ref: HEAD
---

import { copyFileSync, mkdirSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { Command } from "commander";
import { WorkspaceService } from "../../services/workspace.service";

export function registerExportProjectCommand(program: Command): void {
  const workspaceService = new WorkspaceService();

  program
    .command("export-project")
    .description("Export the current project to a new file path")
    .argument("<outputPath>", "Output file path")
    .action((outputPath: string) => {
      try {
        const currentProjectPath = workspaceService.getCurrentDataFile();
        const absolutePath = resolve(process.cwd(), outputPath);

        mkdirSync(dirname(absolutePath), { recursive: true });
        copyFileSync(currentProjectPath, absolutePath);

        console.log(`Exported project to: ${absolutePath}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to export project.";
        console.error(message);
      }
    });
}
