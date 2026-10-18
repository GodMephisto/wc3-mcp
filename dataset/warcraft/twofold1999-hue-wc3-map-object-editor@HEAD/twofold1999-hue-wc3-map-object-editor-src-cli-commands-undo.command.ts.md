---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/undo.command.ts"
ref: HEAD
---

import { Command } from "commander";
import { EDITABLE_UNIT_FIELDS, EditableUnitField } from "../../editors/unit-editor";
import { ChangeLogService } from "../../services/change-log.service";
import { UnitService } from "../../services/unit.service";

export function registerUndoCommand(program: Command): void {
  const changeLogService = new ChangeLogService();
  const unitService = new UnitService();

  program
    .command("undo")
    .description("Revert the most recent unit field change")
    .action(() => {
      try {
        const latest = changeLogService.getLatest();

        if (!latest) {
          console.log("No change history to undo.");
          return;
        }

        if (latest.objectType !== "unit") {
          console.error(`Unsupported change type for undo: ${latest.objectType}`);
          return;
        }

        const field = latest.field as EditableUnitField;
        if (!EDITABLE_UNIT_FIELDS.includes(field)) {
          console.error(`Cannot revert unsupported field: ${latest.field}`);
          return;
        }

        const unit = unitService.getUnitById(latest.objectId);
        if (!unit) {
          console.error(`Cannot undo: unit not found: ${latest.objectId}`);
          return;
        }

        unitService.setUnitField(latest.objectId, field, String(latest.oldValue));
        changeLogService.removeLatest();

        console.log(
          `Reverted unit ${latest.objectId} ${latest.field}: ${latest.newValue} -> ${latest.oldValue}`
        );
      } catch (error) {
        const message = error instanceof Error ? error.message : "Failed to undo change.";
        console.error(message);
      }
    });
}
