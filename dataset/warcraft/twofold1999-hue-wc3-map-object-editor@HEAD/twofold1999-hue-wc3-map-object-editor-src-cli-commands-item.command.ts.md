---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/item.command.ts"
ref: HEAD
---

import { existsSync, readFileSync } from "node:fs";
import { resolve } from "node:path";
import { Command } from "commander";
import {
  EDITABLE_ITEM_FIELDS,
  EditableItemField,
} from "../../editors/item-editor";
import { ItemService } from "../../services/item.service";
import { WorkspaceService } from "../../services/workspace.service";

const PATCHWORK_MAPPED_ITEM_FIELDS = new Set([
  "unam",
  "igol",
  "ides",
  "ihtp",
  "utub",
  "iabi",
  "ipri",
  "istr",
  "ilev",
  "ilvo",
  "iarm",
]);

export function registerItemCommands(program: Command): void {
  const itemService = new ItemService();
  const workspaceService = new WorkspaceService();
  const itemCommand = new Command("item")
    .description("Work with Warcraft III items");

  itemCommand
    .command("list")
    .description("List items")
    .action(() => {
      try {
        const items = itemService.listItems();

        if (items.length === 0) {
          console.log("No items found.");
          return;
        }

        for (const item of items) {
          console.log(`${item.id}: ${item.name}`);
        }
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to list items.";
        console.log(message);
      }
    });

  itemCommand
    .command("show")
    .description("Show details for an item")
    .argument("<itemId>", "Item id")
    .action((itemId: string) => {
      try {
        const item = itemService.getItemById(itemId);

        if (!item) {
          console.log(`Item not found: ${itemId}`);
          return;
        }

        console.log(`ID: ${item.id}`);
        console.log(`Name: ${item.name}`);
        console.log(`Gold Cost: ${item.goldCost}`);
        console.log(`Cooldown: ${item.cooldown}`);
        console.log(`Description: ${item.description}`);
        if (item.tooltipText !== undefined) {
          console.log(`Tooltip Text: ${item.tooltipText}`);
        }
        if (item.abilityId !== undefined) {
          console.log(`Ability Id: ${item.abilityId}`);
        }
        if (item.priority !== undefined) {
          console.log(`Priority: ${item.priority}`);
        }
        if (item.strengthBonus !== undefined) {
          console.log(`Strength Bonus: ${item.strengthBonus}`);
        }
        if (item.level !== undefined) {
          console.log(`Level: ${item.level}`);
        }
        if (item.levelVariance !== undefined) {
          console.log(`Level Variance: ${item.levelVariance}`);
        }
        if (item.armorTypeText !== undefined) {
          console.log(`Armor Type Text: ${item.armorTypeText}`);
        }
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to show item.";
        console.log(message);
      }
    });

  itemCommand
    .command("raw")
    .description("Show raw Patchwork item field modifications for an item id")
    .argument("<itemId>", "Item id")
    .action((itemId: string) => {
      const patchworkSourceDirectory = workspaceService.getPatchworkSourceDirectory();
      if (!patchworkSourceDirectory) {
        console.log(
          "No Patchwork source is linked to the current project. Re-import with import-patchwork or import-patchwork-dir."
        );
        return;
      }

      const itemsFilePath = resolve(patchworkSourceDirectory, "war3map.w3t.json");
      if (!existsSync(itemsFilePath)) {
        console.log(`Patchwork items file not found: ${itemsFilePath}`);
        return;
      }

      let parsed: unknown;
      try {
        parsed = JSON.parse(readFileSync(itemsFilePath, "utf-8"));
      } catch {
        console.log(`Invalid Patchwork JSON file: ${itemsFilePath}`);
        return;
      }

      if (!parsed || typeof parsed !== "object" || Array.isArray(parsed)) {
        console.log(`Invalid Patchwork items JSON: ${itemsFilePath}`);
        return;
      }

      const custom = (parsed as { custom?: unknown }).custom;
      if (!custom || typeof custom !== "object" || Array.isArray(custom)) {
        console.log(`No custom item modifications found in: ${itemsFilePath}`);
        return;
      }

      const itemEntry = Object.entries(custom as Record<string, unknown>).find(
        ([compositeId]) => {
          const [id] = compositeId.split(":");
          return id === itemId;
        }
      );

      if (!itemEntry) {
        console.log(`No raw Patchwork modifications found for item: ${itemId}`);
        return;
      }

      const [compositeId, modifications] = itemEntry;
      if (!Array.isArray(modifications)) {
        console.log(`Invalid modifications format for item: ${compositeId}`);
        return;
      }

      console.log(`Raw Patchwork item fields for ${itemId} (${compositeId}):`);
      for (const entry of modifications) {
        if (!entry || typeof entry !== "object") {
          continue;
        }

        const fieldId = (entry as { id?: unknown }).id;
        if (typeof fieldId !== "string") {
          continue;
        }

        const value = (entry as { value?: unknown }).value;
        const status = PATCHWORK_MAPPED_ITEM_FIELDS.has(fieldId)
          ? "mapped"
          : "unmapped";
        console.log(`${fieldId}: ${JSON.stringify(value)} [${status}]`);
      }
    });

  itemCommand
    .command("set")
    .description("Set an editable field for an item")
    .argument("<itemId>", "Item id")
    .argument("<field>", "Editable field")
    .argument("<value>", "New value")
    .action((itemId: string, field: string, value: string) => {
      const resolvedField = field as EditableItemField;

      if (!EDITABLE_ITEM_FIELDS.includes(resolvedField)) {
        console.log(
          `Invalid field: ${field}. Allowed fields: ${EDITABLE_ITEM_FIELDS.join(
            ", "
          )}`
        );
        return;
      }

      try {
        const update = itemService.setItemField(itemId, resolvedField, value);

        if (update.oldValue === update.newValue) {
          console.log(
            `No change: ${itemId}.${resolvedField} is already ${update.newValue}`
          );
          return;
        }

        console.log(`Updated ${itemId}.${update.field}`);
        console.log(`Old: ${update.oldValue}`);
        console.log(`New: ${update.newValue}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to update item.";
        console.log(message);
      }
    });

  program.addCommand(itemCommand);
}
