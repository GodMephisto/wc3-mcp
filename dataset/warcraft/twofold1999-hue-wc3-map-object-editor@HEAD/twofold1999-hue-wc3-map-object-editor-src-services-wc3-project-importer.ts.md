---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/wc3-project-importer.ts"
ref: HEAD
---

import { mkdirSync, existsSync, readFileSync } from "node:fs";
import { resolve } from "node:path";
import { spawnSync } from "node:child_process";
import { MapDataFile } from "../models/map-data.model";
import { ProjectImporter } from "./project-io";

interface TranslatorConfig {
  command: string;
  args: string[];
}

// Future adapter for importing .w3x / .w3m map data via an external translator.
export class Wc3ProjectImporter implements ProjectImporter {
  public importFromFile(sourcePath: string): MapDataFile {
    this.validateSourcePath(sourcePath);

    // Future flow:
    // 1) run external translator command
    // 2) load translator JSON output
    // 3) map translator schema into internal MapDataFile shape
    const translatorOutputPath = this.runTranslator(sourcePath);
    const translatorData = this.loadTranslatorOutput(translatorOutputPath);

    return this.mapTranslatorDataToMapData(translatorData);
  }

  public importFromTranslatorJson(jsonPath: string): MapDataFile {
    this.validateSourcePath(jsonPath);
    const translatorData = this.loadTranslatorOutput(jsonPath);
    return this.mapTranslatorDataToMapData(translatorData);
  }

  private validateSourcePath(sourcePath: string): void {
    if (!existsSync(sourcePath)) {
      throw new Error(`WC3 map file not found: ${sourcePath}`);
    }
  }

  private runTranslator(sourcePath: string): string {
    const config = this.loadTranslatorConfig();

    if (config.command.trim().length === 0) {
      throw new Error("WC3 translator command is not configured.");
    }

    const outputPath = resolve(process.cwd(), "temp", "translator-output.json");
    mkdirSync(resolve(process.cwd(), "temp"), { recursive: true });

    const args = [...config.args, sourcePath, outputPath];
    const result = spawnSync(config.command, args, {
      encoding: "utf-8",
      stdio: "pipe",
    });

    if (result.error) {
      throw new Error(`WC3 translator execution failed: ${result.error.message}`);
    }

    if (result.status !== 0) {
      const stderr = typeof result.stderr === "string" ? result.stderr.trim() : "";
      const status = result.status ?? "unknown";
      const details = stderr.length > 0 ? ` (${stderr})` : "";
      throw new Error(`WC3 translator failed with status ${status}${details}`);
    }

    if (!existsSync(outputPath)) {
      throw new Error(`WC3 translator did not produce output file: ${outputPath}`);
    }

    return outputPath;
  }

  private loadTranslatorOutput(jsonPath: string): unknown {
    try {
      const raw = readFileSync(jsonPath, "utf-8");
      return JSON.parse(raw) as unknown;
    } catch {
      throw new Error(`Invalid translator output JSON: ${jsonPath}`);
    }
  }

  private mapTranslatorDataToMapData(input: unknown): MapDataFile {
    if (!input || typeof input !== "object") {
      throw new Error("Invalid translator output: expected root object.");
    }

    const root = input as Record<string, unknown>;
    const map = this.asObject(root.map, "map");
    const objects = this.asObject(root.objects, "objects");

    const unitsInput = this.asArrayOrEmpty(objects.units, "objects.units");
    const abilitiesInput = this.asArrayOrEmpty(
      objects.abilities,
      "objects.abilities"
    );
    const itemsInput = this.asArrayOrEmpty(objects.items, "objects.items");

    const units = unitsInput.map((entry, index) => {
      const obj = this.asObject(entry, `objects.units[${index}]`);
      const id = this.readRequiredString(obj.rawcode, `objects.units[${index}].rawcode`);
      const hitPoints = this.readNumber(obj.hp, `objects.units[${index}].hp`, 0);

      return {
        id,
        name: this.readString(obj.name, id),
        combatType: this.readString(obj.combatType ?? obj.combat_type, "Unknown"),
        race: this.readString(obj.race, "Unknown"),
        hitPoints,
        attackType: this.readString(obj.attackType ?? obj.attack_type, "Unknown"),
        armorType: this.readString(obj.armorType ?? obj.armor_type, "Unknown"),
      };
    });

    const abilities = abilitiesInput.map((entry, index) => {
      const obj = this.asObject(entry, `objects.abilities[${index}]`);
      const id = this.readRequiredString(
        obj.rawcode,
        `objects.abilities[${index}].rawcode`
      );

      return {
        id,
        name: this.readString(obj.name, id),
        manaCost: this.readNumber(
          obj.manaCost ?? obj.mana_cost,
          `objects.abilities[${index}].manaCost`,
          0
        ),
        cooldown: this.readNumber(obj.cooldown, `objects.abilities[${index}].cooldown`, 0),
        castRange: this.readNumber(
          obj.castRange ?? obj.cast_range,
          `objects.abilities[${index}].castRange`,
          0
        ),
      };
    });

    const items = itemsInput.map((entry, index) => {
      const obj = this.asObject(entry, `objects.items[${index}]`);
      const id = this.readRequiredString(obj.rawcode, `objects.items[${index}].rawcode`);

      return {
        id,
        name: this.readString(obj.name, id),
        goldCost: this.readNumber(
          obj.goldCost ?? obj.gold_cost,
          `objects.items[${index}].goldCost`,
          0
        ),
        cooldown: this.readNumber(obj.cooldown, `objects.items[${index}].cooldown`, 0),
        description: this.readString(obj.description, ""),
      };
    });

    const mapData: MapDataFile = {
      meta: {
        source: "wc3-translator",
        version: "1.0",
      },
      units,
      abilities,
      items,
    };

    const mapName = map?.name;
    if (typeof mapName === "string" && mapName.length > 0) {
      mapData.meta.mapName = mapName;
    }

    return mapData;
  }

  private asObject(value: unknown, path: string): Record<string, unknown> {
    if (value === undefined) {
      return {};
    }

    if (!value || typeof value !== "object" || Array.isArray(value)) {
      throw new Error(`Invalid translator output: ${path} must be an object.`);
    }

    return value as Record<string, unknown>;
  }

  private asArrayOrEmpty(value: unknown, path: string): unknown[] {
    if (value === undefined) {
      return [];
    }

    if (!Array.isArray(value)) {
      throw new Error(`Invalid translator output: ${path} must be an array.`);
    }

    return value;
  }

  private readRequiredString(value: unknown, path: string): string {
    if (typeof value !== "string" || value.trim().length === 0) {
      throw new Error(`Invalid translator output: ${path} must be a non-empty string.`);
    }

    return value;
  }

  private readString(value: unknown, fallback: string): string {
    return typeof value === "string" ? value : fallback;
  }

  private readNumber(value: unknown, path: string, fallback: number): number {
    if (value === undefined || value === null) {
      return fallback;
    }

    if (typeof value !== "number" || !Number.isFinite(value)) {
      throw new Error(`Invalid translator output: ${path} must be a number.`);
    }

    return value;
  }

  private loadTranslatorConfig(): TranslatorConfig {
    const configPath = resolve(process.cwd(), "config", "translator.json");

    if (!existsSync(configPath)) {
      throw new Error(`Translator config file not found: ${configPath}`);
    }

    let parsed: unknown;
    try {
      const raw = readFileSync(configPath, "utf-8");
      parsed = JSON.parse(raw);
    } catch {
      throw new Error(`Invalid translator config JSON: ${configPath}`);
    }

    if (!parsed || typeof parsed !== "object" || Array.isArray(parsed)) {
      throw new Error(`Invalid translator config: ${configPath}`);
    }

    const config = parsed as Record<string, unknown>;
    if (typeof config.command !== "string") {
      throw new Error(`Invalid translator config: command must be a string in ${configPath}`);
    }

    if (!Array.isArray(config.args) || !config.args.every((arg) => typeof arg === "string")) {
      throw new Error(`Invalid translator config: args must be an array of strings in ${configPath}`);
    }

    return { command: config.command, args: config.args as string[] };
  }
}
