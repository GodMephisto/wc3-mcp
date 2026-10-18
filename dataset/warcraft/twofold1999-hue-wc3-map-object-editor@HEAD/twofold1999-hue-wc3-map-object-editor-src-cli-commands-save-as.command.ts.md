---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/save-as.command.ts"
ref: HEAD
---

import { copyFileSync, mkdirSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { Command } from "commander";
import { WorkspaceService } from "../../services/workspace.service";

export function registerSaveAsCommand(program: Command): void {
  const workspaceService = new WorkspaceService();

  program
    .command("save-as")
    .description("Save the current project to a new file path")
    .argument("<outputPath>", "Output file path")
    .action((outputPath: string) => {
      try {
        const currentDataFile = workspaceService.getCurrentDataFile();
        const absolutePath = resolve(process.cwd(), outputPath);

        mkdirSync(dirname(absolutePath), { recursive: true });
        copyFileSync(currentDataFile, absolutePath);

        workspaceService.setCurrentDataFile(absolutePath);
        console.log(`Saved project as: ${absolutePath}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to save project.";
        console.error(message);
      }
    });
}
