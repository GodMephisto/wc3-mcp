---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/map-data-storage.service.ts"
ref: HEAD
---

import { readFileSync, writeFileSync } from "node:fs";
import { AbilityDetails } from "../models/ability.model";
import { ItemDetails } from "../models/item.model";
import { MapDataFile } from "../models/map-data.model";
import { UnitDetails } from "../models/unit.model";
import { WorkspaceService } from "./workspace.service";

export class MapDataStorageService {
  private readonly workspaceService = new WorkspaceService();

  public validateMapDataFile(filePath: string): void {
    this.loadMapData(filePath);
  }

  public loadMapDataFromFile(filePath: string): MapDataFile {
    return this.loadMapData(filePath);
  }

  public loadUnits(): UnitDetails[] {
    const filePath = this.workspaceService.getCurrentDataFile();
    const mapData = this.loadMapData(filePath);
    return mapData.units;
  }

  public saveUnits(units: UnitDetails[]): void {
    const filePath = this.workspaceService.getCurrentDataFile();
    const mapData = this.loadMapData(filePath);
    mapData.units = units;
    const content = JSON.stringify(mapData, null, 2);
    writeFileSync(filePath, `${content}\n`, "utf-8");
  }

  public loadAbilities(): AbilityDetails[] {
    const filePath = this.workspaceService.getCurrentDataFile();
    const mapData = this.loadMapData(filePath);
    return mapData.abilities ?? [];
  }

  public saveAbilities(abilities: AbilityDetails[]): void {
    const filePath = this.workspaceService.getCurrentDataFile();
    const mapData = this.loadMapData(filePath);
    mapData.abilities = abilities;
    const content = JSON.stringify(mapData, null, 2);
    writeFileSync(filePath, `${content}\n`, "utf-8");
  }

  public loadItems(): ItemDetails[] {
    const filePath = this.workspaceService.getCurrentDataFile();
    const mapData = this.loadMapData(filePath);
    return mapData.items ?? [];
  }

  public saveItems(items: ItemDetails[]): void {
    const filePath = this.workspaceService.getCurrentDataFile();
    const mapData = this.loadMapData(filePath);
    mapData.items = items;
    const content = JSON.stringify(mapData, null, 2);
    writeFileSync(filePath, `${content}\n`, "utf-8");
  }

  private loadMapData(filePath: string): MapDataFile {
    let parsed: unknown;

    try {
      const raw = readFileSync(filePath, "utf-8");
      parsed = JSON.parse(raw);
    } catch {
      throw new Error(`Invalid map data file: ${filePath}`);
    }

    if (!parsed || typeof parsed !== "object" || !Array.isArray((parsed as MapDataFile).units)) {
      throw new Error(`Invalid map data file: missing units array in ${filePath}`);
    }

    const mapData = parsed as MapDataFile;

    if (!mapData.meta || typeof mapData.meta !== "object") {
      mapData.meta = {
        source: "local-json",
        version: "1.0",
      };
    }

    if (mapData.abilities === undefined) {
      mapData.abilities = [];
    } else if (!Array.isArray(mapData.abilities)) {
      throw new Error(`Invalid map data file: abilities must be an array in ${filePath}`);
    }

    if (mapData.items === undefined) {
      mapData.items = [];
    } else if (!Array.isArray(mapData.items)) {
      throw new Error(`Invalid map data file: items must be an array in ${filePath}`);
    }

    return mapData;
  }
}
