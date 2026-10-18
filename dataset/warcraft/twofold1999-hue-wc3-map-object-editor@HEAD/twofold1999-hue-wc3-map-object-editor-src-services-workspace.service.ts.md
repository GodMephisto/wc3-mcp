---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/workspace.service.ts"
ref: HEAD
---

import { existsSync, mkdirSync, readFileSync, writeFileSync } from "node:fs";
import { dirname, resolve } from "node:path";

interface CurrentProjectState {
  currentDataFile: string;
  patchworkSourceDirectory?: string;
}

export class WorkspaceService {
  private readonly stateFilePath = resolve(
    process.cwd(),
    "temp",
    "current-project.json"
  );

  public setCurrentDataFile(
    filePath: string,
    patchworkSourceDirectory?: string
  ): void {
    const directory = dirname(this.stateFilePath);
    mkdirSync(directory, { recursive: true });

    const state: CurrentProjectState = { currentDataFile: filePath };
    if (patchworkSourceDirectory) {
      state.patchworkSourceDirectory = patchworkSourceDirectory;
    }
    writeFileSync(this.stateFilePath, `${JSON.stringify(state, null, 2)}\n`, "utf-8");
  }

  public getCurrentDataFile(): string {
    if (!existsSync(this.stateFilePath)) {
      throw new Error("No project is open. Run: open <filePath>");
    }

    const raw = readFileSync(this.stateFilePath, "utf-8");
    const state = JSON.parse(raw) as CurrentProjectState;

    if (!state.currentDataFile) {
      throw new Error("No project is open. Run: open <filePath>");
    }

    return state.currentDataFile;
  }

  public getPatchworkSourceDirectory(): string | undefined {
    if (!existsSync(this.stateFilePath)) {
      return undefined;
    }

    try {
      const raw = readFileSync(this.stateFilePath, "utf-8");
      const state = JSON.parse(raw) as CurrentProjectState;
      return typeof state.patchworkSourceDirectory === "string"
        ? state.patchworkSourceDirectory
        : undefined;
    } catch {
      return undefined;
    }
  }
}
