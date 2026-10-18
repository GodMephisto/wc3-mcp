---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/status.command.ts"
ref: HEAD
---

import { Command } from "commander";
import { WorkspaceService } from "../../services/workspace.service";

export function registerStatusCommand(program: Command): void {
  const workspaceService = new WorkspaceService();

  program
    .command("status")
    .description("Show the currently opened project data file")
    .action(() => {
      try {
        const currentDataFile = workspaceService.getCurrentDataFile();
        console.log(`Current project: ${currentDataFile}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to read project status.";
        console.error(message);
      }
    });
}
