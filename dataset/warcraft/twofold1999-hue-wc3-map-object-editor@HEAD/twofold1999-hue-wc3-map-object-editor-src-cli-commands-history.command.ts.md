---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/history.command.ts"
ref: HEAD
---

import { Command } from "commander";
import { ChangeLogService } from "../../services/change-log.service";

export function registerHistoryCommand(program: Command): void {
  const changeLogService = new ChangeLogService();

  program
    .command("history")
    .description("Show unit change history")
    .action(() => {
      try {
        const entries = changeLogService.list();

        if (entries.length === 0) {
          console.log("No change history found.");
          return;
        }

        for (const entry of entries) {
          console.log(
            `[${entry.changedAt}] ${entry.objectType} ${entry.objectId} ${entry.field}: ${entry.oldValue} -> ${entry.newValue}`
          );
        }
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to read change history.";
        console.error(message);
      }
    });
}
