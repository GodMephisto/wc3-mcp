---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/workspace.command.ts"
ref: HEAD
---

import { existsSync } from "node:fs";
import { resolve } from "node:path";
import { Command } from "commander";
import { WorkspaceService } from "../../services/workspace.service";

export function registerWorkspaceCommands(program: Command): void {
  const workspaceService = new WorkspaceService();

  program
    .command("open")
    .description("Open a unit data file as the current project")
    .argument("<filePath>", "Path to unit data JSON file")
    .action((filePath: string) => {
      const resolvedPath = resolve(process.cwd(), filePath);

      if (!existsSync(resolvedPath)) {
        console.log(`Data file not found: ${resolvedPath}`);
        return;
      }

      workspaceService.setCurrentDataFile(resolvedPath);
      console.log(`Opened project: ${resolvedPath}`);
    });
}
