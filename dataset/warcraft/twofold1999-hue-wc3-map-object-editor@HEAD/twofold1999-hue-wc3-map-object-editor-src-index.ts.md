---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/index.ts"
ref: HEAD
---

import { Command } from "commander";
import { registerAbilityCommands } from "./cli/commands/ability.command";
import { registerExportPatchworkCommand } from "./cli/commands/export-patchwork.command";
import { registerExportPatchworkWarCommand } from "./cli/commands/export-patchwork-war.command";
import { registerExportProjectCommand } from "./cli/commands/export-project.command";
import { registerImportFakeMapCommand } from "./cli/commands/import-fake-map.command";
import { registerImportPatchworkDirCommand } from "./cli/commands/import-patchwork-dir.command";
import { registerImportPatchworkCommand } from "./cli/commands/import-patchwork.command";
import { registerHistoryCommand } from "./cli/commands/history.command";
import { registerImportProjectCommand } from "./cli/commands/import-project.command";
import { registerImportTranslatorSampleCommand } from "./cli/commands/import-translator-sample.command";
import { registerItemCommands } from "./cli/commands/item.command";
import { registerOpenCommand } from "./cli/commands/open.command";
import { registerProbeWc3MapTranslatorCommand } from "./cli/commands/probe-wc3maptranslator.command";
import { registerSaveAsCommand } from "./cli/commands/save-as.command";
import { registerStatusCommand } from "./cli/commands/status.command";
import { registerSummaryCommand } from "./cli/commands/summary.command";
import { registerUndoCommand } from "./cli/commands/undo.command";
import { registerUnitCommands } from "./cli/commands/unit.command";
import { registerValidateRoundtripCommand } from "./cli/commands/validate-roundtrip.command";
import { CLI_NAME, CLI_VERSION } from "./constants/cli";

const program = new Command();

program
  .name(CLI_NAME)
  .description("Warcraft III map unit editor CLI")
  .version(CLI_VERSION);

registerOpenCommand(program);
registerProbeWc3MapTranslatorCommand(program);
registerExportPatchworkCommand(program);
registerExportPatchworkWarCommand(program);
registerExportProjectCommand(program);
registerImportFakeMapCommand(program);
registerImportPatchworkDirCommand(program);
registerImportPatchworkCommand(program);
registerImportProjectCommand(program);
registerImportTranslatorSampleCommand(program);
registerSaveAsCommand(program);
registerStatusCommand(program);
registerSummaryCommand(program);
registerValidateRoundtripCommand(program);
registerHistoryCommand(program);
registerUndoCommand(program);
registerUnitCommands(program);
registerAbilityCommands(program);
registerItemCommands(program);

program.parse(process.argv);
