---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/hooks/useDragDrop.ts"
ref: HEAD
---

import { useEffect } from "react";
import { getCurrentWebview } from "@tauri-apps/api/webview";
import { useTranslation } from "react-i18next";
import { convertPaths } from "../lib/conversion";
import { useAppStore } from "../store/useAppStore";
import type { ConvertDirection } from "../types";

function directionFromX(x: number): ConvertDirection {
  return x < window.innerWidth / 2 ? "to-png" : "to-blp";
}

export function useDragDrop() {
  const { t } = useTranslation();
  const setDragTarget = useAppStore((s) => s.setDragTarget);
  const addLog = useAppStore((s) => s.addLog);

  useEffect(() => {
    let unlisten: (() => void) | undefined;

    const setup = async () => {
      if (!("__TAURI_INTERNALS__" in window)) {
        return;
      }

      unlisten = await getCurrentWebview().onDragDropEvent(async (event) => {
        if (event.payload.type === "over" || event.payload.type === "enter") {
          if ("position" in event.payload) {
            setDragTarget(directionFromX(event.payload.position.x));
          }
          return;
        }

        if (event.payload.type === "leave") {
          setDragTarget(null);
          return;
        }

        if (event.payload.type === "drop") {
          setDragTarget(null);

          if (useAppStore.getState().isConverting) {
            addLog(t("conversion.busy"), "error");
            return;
          }

          const paths = event.payload.paths;
          if (paths.length === 0) {
            return;
          }

          const direction =
            "position" in event.payload
              ? directionFromX(event.payload.position.x)
              : "to-png";

          try {
            await convertPaths(paths, direction);
          } catch (err) {
            addLog(
              err instanceof Error
                ? err.message
                : t("conversion.dropFailed"),
              "error",
            );
          }
        }
      });
    };

    void setup();

    return () => {
      unlisten?.();
    };
  }, [addLog, setDragTarget, t]);
}
