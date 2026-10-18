---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/cli/commands/probe-wc3maptranslator.command.ts"
ref: HEAD
---

import { existsSync, mkdirSync } from "node:fs";
import { resolve } from "node:path";
import { spawnSync } from "node:child_process";
import { Command } from "commander";

export function registerProbeWc3MapTranslatorCommand(program: Command): void {
  program
    .command("probe-wc3maptranslator")
    .description("Development probe for real WC3MapTranslator JSON output")
    .argument("<inputDir>", "Input directory containing map data files")
    .argument("<outputDir>", "Output directory for translator JSON files")
    .action((inputDir: string, outputDir: string) => {
      try {
        const inputDirPath = resolve(process.cwd(), inputDir);
        const outputDirPath = resolve(process.cwd(), outputDir);
        const translatorCliPath = resolve(
          process.cwd(),
          "WC3MapTranslator",
          "dist",
          "src",
          "cli.js"
        );

        if (!existsSync(translatorCliPath)) {
          console.error(
            `WC3MapTranslator CLI not found: ${translatorCliPath}`
          );
          return;
        }

        mkdirSync(outputDirPath, { recursive: true });

        const result = spawnSync(
          "node",
          [
            translatorCliPath,
            inputDirPath,
            outputDirPath,
            "--toJson",
            "--force",
          ],
          {
            encoding: "utf-8",
            stdio: "pipe",
          }
        );

        if (result.error) {
          console.error(
            `Failed to run WC3MapTranslator: ${result.error.message}`
          );
          return;
        }

        if (result.status !== 0) {
          const stderr =
            typeof result.stderr === "string" && result.stderr.trim().length > 0
              ? result.stderr.trim()
              : "No stderr output.";
          console.error(
            `WC3MapTranslator failed with status ${result.status}: ${stderr}`
          );
          return;
        }

        console.log(`WC3MapTranslator probe output: ${outputDirPath}`);
      } catch (error) {
        const message =
          error instanceof Error ? error.message : "Failed to probe WC3MapTranslator.";
        console.error(message);
      }
    });
}
