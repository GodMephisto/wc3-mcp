---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/summary.command.ts"
ref: HEAD
---

import { Command } from "commander";
import { MapDataStorageService } from "../../services/map-data-storage.service";
import { WorkspaceService } from "../../services/workspace.service";

export function registerSummaryCommand(program: Command): void {
  const workspaceService = new WorkspaceService();
  const mapDataStorageService = new MapDataStorageService();

  program
    .command("summary")
    .description("Show current project and map object counts")
    .action(() => {
      try {
        const currentProjectPath = workspaceService.getCurrentDataFile();
        const units = mapDataStorageService.loadUnits();
        const abilities = mapDataStorageService.loadAbilities();
        const items = mapDataStorageService.loadItems();

        console.log(`Current project: ${currentProjectPath}`);
        console.log(`Units: ${units.length}`);
        console.log(`Abilities: ${abilities.length}`);
        console.log(`Items: ${items.length}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to show summary.";
        console.error(message);
      }
    });
}
