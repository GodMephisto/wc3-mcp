---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/validate-roundtrip.command.ts"
ref: HEAD
---

import { spawnSync } from "node:child_process";
import { mkdirSync, rmSync } from "node:fs";
import { resolve } from "node:path";
import { Command } from "commander";
import { AbilityService } from "../../services/ability.service";
import { ItemService } from "../../services/item.service";
import { JsonProjectExporter } from "../../services/json-project-io";
import { MapDataStorageService } from "../../services/map-data-storage.service";
import { PatchworkProjectExporter } from "../../services/patchwork-project-exporter";
import { PatchworkProjectImporter } from "../../services/patchwork-project-importer";
import { UnitService } from "../../services/unit.service";
import { WorkspaceService } from "../../services/workspace.service";

export function registerValidateRoundtripCommand(program: Command): void {
  const patchworkImporter = new PatchworkProjectImporter();
  const jsonProjectExporter = new JsonProjectExporter();
  const patchworkProjectExporter = new PatchworkProjectExporter();
  const workspaceService = new WorkspaceService();
  const mapDataStorageService = new MapDataStorageService();
  const unitService = new UnitService();
  const abilityService = new AbilityService();
  const itemService = new ItemService();

  program
    .command("validate-roundtrip")
    .description("Run full Patchwork round-trip validation flow")
    .argument("<inputDir>", "Input directory for patchwork-mapconverter")
    .action((inputDir: string) => {
      let currentStage = "initialization";
      try {
        const inputDirPath = resolve(process.cwd(), inputDir);
        const patchworkImportDir = resolve(process.cwd(), "temp", "patchwork-import");
        const roundtripExportDir = resolve(process.cwd(), "temp", "roundtrip-export");
        const roundtripWarDir = resolve(process.cwd(), "temp", "roundtrip-war");
        const workspacePath = resolve(process.cwd(), "temp", "project.json");

        currentStage = "Stage 1/4: importing source";
        console.log("Stage 1/4: importing source...");
        console.log(`  Source input directory: ${inputDirPath}`);
        console.log(`  Patchwork json output: ${patchworkImportDir}`);
        resetDirectory(patchworkImportDir);
        runPatchwork("war2json", inputDirPath, patchworkImportDir);
        const sourceMapData = patchworkImporter.importFromDirectory(patchworkImportDir);
        jsonProjectExporter.exportToFile(sourceMapData, workspacePath);
        workspaceService.setCurrentDataFile(workspacePath, patchworkImportDir);

        currentStage = "Stage 2/4: exporting patchwork json";
        console.log("Stage 2/4: exporting patchwork json...");
        console.log(`  Workspace project source: ${workspacePath}`);
        console.log(`  Round-trip export directory: ${roundtripExportDir}`);
        resetDirectory(roundtripExportDir);
        const currentMapData =
          mapDataStorageService.loadMapDataFromFile(workspacePath);
        patchworkProjectExporter.exportToDirectory(currentMapData, roundtripExportDir);

        currentStage = "Stage 3/4: exporting war files";
        console.log("Stage 3/4: exporting war files...");
        console.log(`  Patchwork json input: ${roundtripExportDir}`);
        console.log(`  War output directory: ${roundtripWarDir}`);
        resetDirectory(roundtripWarDir);
        runPatchwork("json2war", roundtripExportDir, roundtripWarDir);

        currentStage = "Stage 4/4: re-importing generated war files";
        console.log("Stage 4/4: re-importing generated war files...");
        console.log(`  War input directory: ${roundtripWarDir}`);
        console.log(`  Patchwork json output: ${patchworkImportDir}`);
        resetDirectory(patchworkImportDir);
        runPatchwork("war2json", roundtripWarDir, patchworkImportDir);
        const roundtripMapData = patchworkImporter.importFromDirectory(patchworkImportDir);
        jsonProjectExporter.exportToFile(roundtripMapData, workspacePath);
        workspaceService.setCurrentDataFile(workspacePath, patchworkImportDir);

        const units = unitService.listUnits();
        const abilities = abilityService.listAbilities();
        const items = itemService.listItems();

        console.log("Final Object Counts:");
        console.log(`Units: ${units.length}`);
        console.log(`Abilities: ${abilities.length}`);
        console.log(`Items: ${items.length}`);

        console.log("Units:");
        for (const unit of units) {
          console.log(`${unit.id}: ${unit.name} (${unit.combatType})`);
        }

        console.log("Abilities:");
        for (const ability of abilities) {
          console.log(`${ability.id}: ${ability.name}`);
        }

        console.log("Items:");
        for (const item of items) {
          console.log(`${item.id}: ${item.name}`);
        }

        console.log("Validation Complete:");
        console.log(`Final workspace project: ${workspacePath}`);
        console.log(`Round-trip export directory: ${roundtripExportDir}`);
        console.log(`Round-trip war directory: ${roundtripWarDir}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to validate round-trip.";
        console.error(`Round-trip validation failed at ${currentStage}`);
        console.error(message);
      }
    });
}

function resetDirectory(directoryPath: string): void {
  rmSync(directoryPath, { recursive: true, force: true });
  mkdirSync(directoryPath, { recursive: true });
}

function runPatchwork(
  mode: "war2json" | "json2war",
  inputDirPath: string,
  outputDirPath: string
): void {
  const patchworkArgs = [
    "patchwork-mapconverter",
    mode,
    inputDirPath,
    outputDirPath,
    "-d",
  ];

  const isWindows = process.platform === "win32";
  const result = spawnSync(
    isWindows ? "cmd" : "npx",
    isWindows ? ["/c", "npx", ...patchworkArgs] : patchworkArgs,
    {
      encoding: "utf-8",
      stdio: "pipe",
    }
  );

  if (result.error) {
    throw new Error(`Failed to run patchwork-mapconverter: ${result.error.message}`);
  }

  if (result.status !== 0) {
    const stderr =
      typeof result.stderr === "string" && result.stderr.trim().length > 0
        ? result.stderr.trim()
        : "No stderr output.";
    throw new Error(
      `patchwork-mapconverter failed with status ${result.status}: ${stderr}`
    );
  }
}
