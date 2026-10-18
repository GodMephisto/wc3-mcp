---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/lib/tauri.ts"
ref: HEAD
---

import { invoke } from "@tauri-apps/api/core";
import type {
  AssetKind,
  BatchConvertResult,
  BlpViewActionResult,
  BlpViewStatus,
  ConversionSettings,
} from "../types";
import i18n from "../i18n";

export async function convertDroppedPaths(
  paths: string[],
  kind: AssetKind,
  settings: ConversionSettings,
): Promise<BatchConvertResult> {
  return invoke<BatchConvertResult>("convert_paths", { paths, kind, settings });
}

export async function pickOutputDirectory(): Promise<string | null> {
  const { open } = await import("@tauri-apps/plugin-dialog");
  const selected = await open({
    directory: true,
    multiple: false,
    title: i18n.t("settings.pickOutputTitle"),
  });

  if (typeof selected === "string") {
    return selected;
  }

  return null;
}

export async function getBlpViewStatus(): Promise<BlpViewStatus> {
  return invoke<BlpViewStatus>("blpview_status");
}

export async function installBlpView(): Promise<BlpViewActionResult> {
  return invoke<BlpViewActionResult>("blpview_install");
}

export async function uninstallBlpView(): Promise<BlpViewActionResult> {
  return invoke<BlpViewActionResult>("blpview_uninstall");
}

export async function restartExplorer(): Promise<void> {
  return invoke("blpview_restart_explorer");
}
