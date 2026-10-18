---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/unit.command.ts"
ref: HEAD
---

import { existsSync, readFileSync } from "node:fs";
import { resolve } from "node:path";
import { Command } from "commander";
import {
  EDITABLE_UNIT_FIELDS,
  EditableUnitField,
} from "../../editors/unit-editor";
import { ChangeLogService } from "../../services/change-log.service";
import { UnitService } from "../../services/unit.service";
import { WorkspaceService } from "../../services/workspace.service";

const PATCHWORK_MAPPED_UNIT_FIELDS = new Set([
  "unam",
  "uhpm",
  "umvs",
  "ugol",
  "udef",
  "uacq",
  "uhpr",
  "ua1b",
  "uaen",
  "ua1z",
  "ua1r",
  "ua2r",
  "ua1w",
  "ua2w",
  "ua2t",
  "ua1g",
  "ua1p",
  "usr1",
  "usr2",
  "utc1",
  "utc2",
  "uamn",
  "usd1",
  "usd2",
  "ulum",
  "umpi",
  "umpr",
]);

export function registerUnitCommands(program: Command): void {
  const unitService = new UnitService();
  const changeLogService = new ChangeLogService();
  const workspaceService = new WorkspaceService();
  const fieldAliases: Record<string, EditableUnitField> = {
    hp: "hitPoints",
    atk: "attackType",
    armor: "armorType",
  };
  const unitCommand = new Command("unit")
    .description("Work with Warcraft III units");

  unitCommand
    .command("list")
    .description("List units")
    .action(() => {
      try {
        const units = unitService.listUnits();

        if (units.length === 0) {
          console.log("No units found.");
          return;
        }

        for (const unit of units) {
          console.log(`${unit.id}: ${unit.name} (${unit.combatType})`);
        }
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to list units.";
        console.log(message);
      }
    });

  unitCommand
    .command("show")
    .description("Show details for a unit")
    .argument("<unitId>", "Unit id")
    .action((unitId: string) => {
      try {
        const unit = unitService.getUnitById(unitId);

        if (!unit) {
          console.log(`Unit not found: ${unitId}`);
          return;
        }

        console.log(`ID: ${unit.id}`);
        console.log(`Name: ${unit.name}`);
        console.log(`Combat Type: ${unit.combatType}`);
        console.log(`Race: ${unit.race}`);
        console.log(`Hit Points: ${unit.hitPoints}`);
        if (unit.moveSpeed !== undefined) {
          console.log(`Move Speed: ${unit.moveSpeed}`);
        }
        if (unit.goldCost !== undefined) {
          console.log(`Gold Cost: ${unit.goldCost}`);
        }
        if (unit.armor !== undefined) {
          console.log(`Armor: ${unit.armor}`);
        }
        if (unit.acquisitionRange !== undefined) {
          console.log(`Acquisition Range: ${unit.acquisitionRange}`);
        }
        if (unit.hitPointRegen !== undefined) {
          console.log(`Hit Point Regen: ${unit.hitPointRegen}`);
        }
        if (unit.attackBaseDamage !== undefined) {
          console.log(`Attack Base Damage: ${unit.attackBaseDamage}`);
        }
        if (unit.attackEnabled !== undefined) {
          console.log(`Attack Enabled: ${unit.attackEnabled}`);
        }
        if (unit.secondaryAttackBase !== undefined) {
          console.log(`Secondary Attack Base: ${unit.secondaryAttackBase}`);
        }
        if (unit.primaryAttackRange !== undefined) {
          console.log(`Primary Attack Range: ${unit.primaryAttackRange}`);
        }
        if (unit.secondaryAttackRange !== undefined) {
          console.log(`Secondary Attack Range: ${unit.secondaryAttackRange}`);
        }
        if (unit.primaryAttackSpeed !== undefined) {
          console.log(`Primary Attack Speed: ${unit.primaryAttackSpeed}`);
        }
        if (unit.secondaryAttackSpeed !== undefined) {
          console.log(`Secondary Attack Speed: ${unit.secondaryAttackSpeed}`);
        }
        if (unit.primaryWeaponTypeText !== undefined) {
          console.log(`Primary Weapon Type Text: ${unit.primaryWeaponTypeText}`);
        }
        if (unit.secondaryWeaponTypeText !== undefined) {
          console.log(`Secondary Weapon Type Text: ${unit.secondaryWeaponTypeText}`);
        }
        if (unit.secondaryAttackTypeText !== undefined) {
          console.log(`Secondary Attack Type Text: ${unit.secondaryAttackTypeText}`);
        }
        if (unit.primaryTargetFlags && unit.primaryTargetFlags.length > 0) {
          console.log(`Primary Target Flags: ${unit.primaryTargetFlags.join(", ")}`);
        }
        if (
          unit.primaryTargetPreferences &&
          unit.primaryTargetPreferences.length > 0
        ) {
          console.log(
            `Primary Target Preferences: ${unit.primaryTargetPreferences.join(", ")}`
          );
        }
        if (unit.turnRate !== undefined) {
          console.log(`Turn Rate: ${unit.turnRate}`);
        }
        if (unit.secondaryTurnRate !== undefined) {
          console.log(`Secondary Turn Rate: ${unit.secondaryTurnRate}`);
        }
        if (unit.minimumAttackRange !== undefined) {
          console.log(`Minimum Attack Range: ${unit.minimumAttackRange}`);
        }
        if (unit.primaryDiceSides !== undefined) {
          console.log(`Primary Dice Sides: ${unit.primaryDiceSides}`);
        }
        if (unit.secondaryDiceSides !== undefined) {
          console.log(`Secondary Dice Sides: ${unit.secondaryDiceSides}`);
        }
        if (unit.lumberCost !== undefined) {
          console.log(`Lumber Cost: ${unit.lumberCost}`);
        }
        if (unit.foodCost !== undefined) {
          console.log(`Food Cost: ${unit.foodCost}`);
        }
        if (unit.movementPriority !== undefined) {
          console.log(`Movement Priority: ${unit.movementPriority}`);
        }
        console.log(`Attack Type: ${unit.attackType}`);
        console.log(`Armor Type: ${unit.armorType}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to show unit.";
        console.log(message);
      }
    });

  unitCommand
    .command("export")
    .description("Export a unit as formatted JSON")
    .argument("<unitId>", "Unit id")
    .action((unitId: string) => {
      try {
        const unit = unitService.getUnitById(unitId);

        if (!unit) {
          console.log(`Unit not found: ${unitId}`);
          return;
        }

        console.log(JSON.stringify(unit, null, 2));
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to export unit.";
        console.log(message);
      }
    });

  unitCommand
    .command("raw")
    .description("Show raw Patchwork unit field modifications for a unit id")
    .argument("<unitId>", "Unit id")
    .action((unitId: string) => {
      const patchworkSourceDirectory = workspaceService.getPatchworkSourceDirectory();
      if (!patchworkSourceDirectory) {
        console.log(
          "No Patchwork source is linked to the current project. Re-import with import-patchwork or import-patchwork-dir."
        );
        return;
      }

      const unitsFilePath = resolve(patchworkSourceDirectory, "war3map.w3u.json");
      if (!existsSync(unitsFilePath)) {
        console.log(`Patchwork units file not found: ${unitsFilePath}`);
        return;
      }

      let parsed: unknown;
      try {
        parsed = JSON.parse(readFileSync(unitsFilePath, "utf-8"));
      } catch {
        console.log(`Invalid Patchwork JSON file: ${unitsFilePath}`);
        return;
      }

      if (!parsed || typeof parsed !== "object" || Array.isArray(parsed)) {
        console.log(`Invalid Patchwork units JSON: ${unitsFilePath}`);
        return;
      }

      const custom = (parsed as { custom?: unknown }).custom;
      if (!custom || typeof custom !== "object" || Array.isArray(custom)) {
        console.log(`No custom unit modifications found in: ${unitsFilePath}`);
        return;
      }

      const unitEntry = Object.entries(custom as Record<string, unknown>).find(
        ([compositeId]) => {
          const [id] = compositeId.split(":");
          return id === unitId;
        }
      );

      if (!unitEntry) {
        console.log(`No raw Patchwork modifications found for unit: ${unitId}`);
        return;
      }

      const [compositeId, modifications] = unitEntry;
      if (!Array.isArray(modifications)) {
        console.log(`Invalid modifications format for unit: ${compositeId}`);
        return;
      }

      console.log(`Raw Patchwork unit fields for ${unitId} (${compositeId}):`);
      for (const entry of modifications) {
        if (!entry || typeof entry !== "object") {
          continue;
        }

        const fieldId = (entry as { id?: unknown }).id;
        if (typeof fieldId !== "string") {
          continue;
        }

        const value = (entry as { value?: unknown }).value;
        const status = PATCHWORK_MAPPED_UNIT_FIELDS.has(fieldId)
          ? "mapped"
          : "unmapped";
        console.log(`${fieldId}: ${JSON.stringify(value)} [${status}]`);
      }
    });

  unitCommand
    .command("search")
    .description("Search units by keyword")
    .argument("<keyword>", "Keyword to search")
    .action((keyword: string) => {
      try {
        const units = unitService.searchUnits(keyword);

        if (units.length === 0) {
          console.log(`No units match keyword: ${keyword}`);
          return;
        }

        for (const unit of units) {
          console.log(`${unit.id}: ${unit.name} (${unit.combatType})`);
        }
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to search units.";
        console.log(message);
      }
    });

  unitCommand
    .command("fields")
    .description("List editable unit fields")
    .action(() => {
      for (const field of EDITABLE_UNIT_FIELDS) {
        console.log(field);
      }
    });

  unitCommand
    .command("set")
    .description("Set an editable field for a unit")
    .argument("<unitId>", "Unit id")
    .argument("<field>", "Editable field")
    .argument("<value>", "New value")
    .action((unitId: string, field: string, value: string) => {
      const resolvedField =
        fieldAliases[field.toLowerCase()] ?? (field as EditableUnitField);

      if (!EDITABLE_UNIT_FIELDS.includes(resolvedField)) {
        console.log(
          `Invalid field: ${field}. Allowed fields: ${EDITABLE_UNIT_FIELDS.join(
            ", "
          )}`
        );
        return;
      }

      try {
        const update = unitService.setUnitField(
          unitId,
          resolvedField,
          value
        );

        if (update.oldValue === update.newValue) {
          console.log(
            `No change: ${unitId}.${resolvedField} is already ${update.newValue}`
          );
          return;
        }

        changeLogService.append({
          objectType: "unit",
          objectId: unitId,
          field: resolvedField,
          oldValue: update.oldValue,
          newValue: update.newValue,
          changedAt: new Date().toISOString(),
        });

        console.log(`Updated ${unitId}.${update.field}`);
        console.log(`Old: ${update.oldValue}`);
        console.log(`New: ${update.newValue}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to update unit.";
        console.error(message);
      }
    });

  program.addCommand(unitCommand);
}
