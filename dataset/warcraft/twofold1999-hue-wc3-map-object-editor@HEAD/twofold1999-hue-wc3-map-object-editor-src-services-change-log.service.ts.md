---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/change-log.service.ts"
ref: HEAD
---

import { existsSync, mkdirSync, readFileSync, writeFileSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { ChangeLogEntry } from "../models/change-log.model";

export class ChangeLogService {
  private readonly logFilePath = resolve(process.cwd(), "temp", "change-log.json");

  public append(entry: ChangeLogEntry): void {
    const entries = this.list();
    entries.push(entry);
    this.ensureDirectory();
    writeFileSync(this.logFilePath, `${JSON.stringify(entries, null, 2)}\n`, "utf-8");
  }

  public list(): ChangeLogEntry[] {
    if (!existsSync(this.logFilePath)) {
      return [];
    }

    const raw = readFileSync(this.logFilePath, "utf-8");
    return JSON.parse(raw) as ChangeLogEntry[];
  }

  public getLatest(): ChangeLogEntry | undefined {
    const entries = this.list();
    return entries.at(-1);
  }

  public removeLatest(): void {
    const entries = this.list();

    if (entries.length === 0) {
      return;
    }

    entries.pop();
    this.ensureDirectory();
    writeFileSync(this.logFilePath, `${JSON.stringify(entries, null, 2)}\n`, "utf-8");
  }

  private ensureDirectory(): void {
    mkdirSync(dirname(this.logFilePath), { recursive: true });
  }
}
