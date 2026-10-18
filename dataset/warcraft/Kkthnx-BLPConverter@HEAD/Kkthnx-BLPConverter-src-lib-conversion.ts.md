---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/lib/conversion.ts"
ref: HEAD
---

import type { AssetKind, ConvertDirection } from "../types";
import { convertDroppedPaths } from "./tauri";
import { useAppStore } from "../store/useAppStore";
import i18n from "../i18n";

function kindFromDirection(direction: ConvertDirection): AssetKind {
  return direction === "to-png" ? "blp" : "png";
}

export async function convertPaths(
  paths: string[],
  direction: ConvertDirection,
): Promise<void> {
  const store = useAppStore.getState();
  const settings = store.settings;
  const kind = kindFromDirection(direction);

  const label =
    kind === "blp"
      ? i18n.t("conversion.labelBlpToPng")
      : i18n.t("conversion.labelPngToBlp");
  store.addLog(
    i18n.t("conversion.start", { count: paths.length, label }),
  );

  if (!settings.outputDirectory) {
    store.addLog(i18n.t("conversion.outputSameFolder"));
  } else {
    store.addLog(
      i18n.t("conversion.outputCustomFolder", {
        path: settings.outputDirectory,
      }),
    );
  }

  store.setConverting(true);

  try {
    const result = await convertDroppedPaths(paths, kind, settings);

    for (const error of result.scanErrors) {
      store.addLog(i18n.t("conversion.scanError", { error }), "error");
    }

    if (result.results.length === 0) {
      if (result.scanErrors.length === 0) {
        const expected = kind === "blp" ? "BLP" : "PNG";
        store.addLog(i18n.t("conversion.noFiles", { format: expected }), "error");
      }
      return;
    }

    for (const item of result.results) {
      if (item.queueStatus === "completed" && item.outputPath) {
        store.addLog(
          i18n.t("conversion.fileResult", {
            name: item.name,
            path: item.outputPath,
          }),
          "success",
        );
      } else if (item.queueStatus === "failed") {
        store.addLog(
          `${item.name}: ${item.errorMessage ?? i18n.t("conversion.failed")}`,
          "error",
        );
      }
    }

    store.addLog(
      i18n.t("conversion.done", {
        succeeded: result.succeeded,
        failed: result.failed,
      }),
      result.failed > 0 ? "error" : "success",
    );
  } catch (err) {
    store.addLog(
      err instanceof Error ? err.message : i18n.t("conversion.failed"),
      "error",
    );
  } finally {
    store.setConverting(false);
  }
}
