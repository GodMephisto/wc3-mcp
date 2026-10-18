---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/ability.command.ts"
ref: HEAD
---

import { existsSync, readFileSync } from "node:fs";
import { resolve } from "node:path";
import { Command } from "commander";
import {
  EDITABLE_ABILITY_FIELDS,
  EditableAbilityField,
} from "../../editors/ability-editor";
import { AbilityService } from "../../services/ability.service";
import { WorkspaceService } from "../../services/workspace.service";

const PATCHWORK_MAPPED_ABILITY_FIELDS = new Set([
  "anam",
  "amcs",
  "acdn",
  "aran",
  "aare",
  "alev",
  "mls1",
  "Inf1",
  "Inf2",
  "Inf3",
]);

export function registerAbilityCommands(program: Command): void {
  const abilityService = new AbilityService();
  const workspaceService = new WorkspaceService();
  const abilityCommand = new Command("ability")
    .description("Work with Warcraft III abilities");

  abilityCommand
    .command("list")
    .description("List abilities")
    .action(() => {
      try {
        const abilities = abilityService.listAbilities();

        if (abilities.length === 0) {
          console.log("No abilities found.");
          return;
        }

        for (const ability of abilities) {
          console.log(`${ability.id}: ${ability.name}`);
        }
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to list abilities.";
        console.log(message);
      }
    });

  abilityCommand
    .command("show")
    .description("Show details for an ability")
    .argument("<abilityId>", "Ability id")
    .action((abilityId: string) => {
      try {
        const ability = abilityService.getAbilityById(abilityId);

        if (!ability) {
          console.log(`Ability not found: ${abilityId}`);
          return;
        }

        console.log(`ID: ${ability.id}`);
        console.log(`Name: ${ability.name}`);
        console.log(`Mana Cost: ${ability.manaCost}`);
        if (ability.manaCostLevels && ability.manaCostLevels.length > 0) {
          console.log(`Mana Cost Levels: ${ability.manaCostLevels.join(", ")}`);
        }
        console.log(`Cooldown: ${ability.cooldown}`);
        console.log(`Cast Range: ${ability.castRange}`);
        if (ability.areaValues && ability.areaValues.length > 0) {
          console.log(`Area Values: ${ability.areaValues.join(", ")}`);
        }
        if (ability.maxLevel !== undefined) {
          console.log(`Max Level: ${ability.maxLevel}`);
        }
        if (ability.levelValues && ability.levelValues.length > 0) {
          console.log(`Level Values: ${ability.levelValues.join(", ")}`);
        }
        if (ability.inf1Values && ability.inf1Values.length > 0) {
          console.log(`Inf1 Values: ${ability.inf1Values.join(", ")}`);
        }
        if (ability.inf2Value !== undefined) {
          console.log(`Inf2 Value: ${ability.inf2Value}`);
        }
        if (ability.inf3Value !== undefined) {
          console.log(`Inf3 Value: ${ability.inf3Value}`);
        }
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to show ability.";
        console.log(message);
      }
    });

  abilityCommand
    .command("raw")
    .description("Show raw Patchwork ability field modifications for an ability id")
    .argument("<abilityId>", "Ability id")
    .action((abilityId: string) => {
      const patchworkSourceDirectory = workspaceService.getPatchworkSourceDirectory();
      if (!patchworkSourceDirectory) {
        console.log(
          "No Patchwork source is linked to the current project. Re-import with import-patchwork or import-patchwork-dir."
        );
        return;
      }

      const abilitiesFilePath = resolve(patchworkSourceDirectory, "war3map.w3a.json");
      if (!existsSync(abilitiesFilePath)) {
        console.log(`Patchwork abilities file not found: ${abilitiesFilePath}`);
        return;
      }

      let parsed: unknown;
      try {
        parsed = JSON.parse(readFileSync(abilitiesFilePath, "utf-8"));
      } catch {
        console.log(`Invalid Patchwork JSON file: ${abilitiesFilePath}`);
        return;
      }

      if (!parsed || typeof parsed !== "object" || Array.isArray(parsed)) {
        console.log(`Invalid Patchwork abilities JSON: ${abilitiesFilePath}`);
        return;
      }

      const custom = (parsed as { custom?: unknown }).custom;
      if (!custom || typeof custom !== "object" || Array.isArray(custom)) {
        console.log(`No custom ability modifications found in: ${abilitiesFilePath}`);
        return;
      }

      const abilityEntry = Object.entries(custom as Record<string, unknown>).find(
        ([compositeId]) => {
          const [id] = compositeId.split(":");
          return id === abilityId;
        }
      );

      if (!abilityEntry) {
        console.log(`No raw Patchwork modifications found for ability: ${abilityId}`);
        return;
      }

      const [compositeId, modifications] = abilityEntry;
      if (!Array.isArray(modifications)) {
        console.log(`Invalid modifications format for ability: ${compositeId}`);
        return;
      }

      console.log(`Raw Patchwork ability fields for ${abilityId} (${compositeId}):`);
      for (const entry of modifications) {
        if (!entry || typeof entry !== "object") {
          continue;
        }

        const fieldId = (entry as { id?: unknown }).id;
        if (typeof fieldId !== "string") {
          continue;
        }

        const value = (entry as { value?: unknown }).value;
        const status = PATCHWORK_MAPPED_ABILITY_FIELDS.has(fieldId)
          ? "mapped"
          : "unmapped";
        console.log(`${fieldId}: ${JSON.stringify(value)} [${status}]`);
      }
    });

  abilityCommand
    .command("set")
    .description("Set an editable field for an ability")
    .argument("<abilityId>", "Ability id")
    .argument("<field>", "Editable field")
    .argument("<value>", "New value")
    .action((abilityId: string, field: string, value: string) => {
      const resolvedField = field as EditableAbilityField;

      if (!EDITABLE_ABILITY_FIELDS.includes(resolvedField)) {
        console.log(
          `Invalid field: ${field}. Allowed fields: ${EDITABLE_ABILITY_FIELDS.join(
            ", "
          )}`
        );
        return;
      }

      try {
        const update = abilityService.setAbilityField(
          abilityId,
          resolvedField,
          value
        );

        if (update.oldValue === update.newValue) {
          console.log(
            `No change: ${abilityId}.${resolvedField} is already ${update.newValue}`
          );
          return;
        }

        console.log(`Updated ${abilityId}.${update.field}`);
        console.log(`Old: ${update.oldValue}`);
        console.log(`New: ${update.newValue}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to update ability.";
        console.log(message);
      }
    });

  program.addCommand(abilityCommand);
}
