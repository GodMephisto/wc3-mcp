---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/export-patchwork-war.command.ts"
ref: HEAD
---

import { spawnSync } from "node:child_process";
import { mkdirSync } from "node:fs";
import { resolve } from "node:path";
import { Command } from "commander";

export function registerExportPatchworkWarCommand(program: Command): void {
  program
    .command("export-patchwork-war")
    .description(
      "Convert Patchwork JSON files back into Warcraft III binary object files"
    )
    .argument("<inputDirectory>", "Directory containing Patchwork JSON files")
    .argument("<outputDirectory>", "Directory for Warcraft III binary object files")
    .action((inputDirectory: string, outputDirectory: string) => {
      try {
        const inputDirPath = resolve(process.cwd(), inputDirectory);
        const outputDirPath = resolve(process.cwd(), outputDirectory);
        mkdirSync(outputDirPath, { recursive: true });

        const patchworkArgs = [
          "patchwork-mapconverter",
          "json2war",
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
          console.error(
            `Failed to run patchwork-mapconverter: ${result.error.message}`
          );
          return;
        }

        if (result.status !== 0) {
          const stderr =
            typeof result.stderr === "string" && result.stderr.trim().length > 0
              ? result.stderr.trim()
              : "No stderr output.";
          console.error(
            `patchwork-mapconverter failed with status ${result.status}: ${stderr}`
          );
          return;
        }

        console.log(`Exported Warcraft III object files to: ${outputDirPath}`);
      } catch (error) {
        const message =
          error instanceof Error
            ? error.message
            : "Failed to export Patchwork JSON to Warcraft III object files.";
        console.error(message);
      }
    });
}
