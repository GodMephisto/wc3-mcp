---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/store/useAppStore.ts"
ref: HEAD
---

import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { ConversionSettings, ConvertDirection, LogEntry } from "../types";

interface AppState {
  settingsOpen: boolean;
  isConverting: boolean;
  dragTarget: ConvertDirection | null;
  logs: LogEntry[];
  settings: ConversionSettings;

  setSettingsOpen: (open: boolean) => void;
  setDragTarget: (target: ConvertDirection | null) => void;
  setConverting: (converting: boolean) => void;
  addLog: (message: string, level?: LogEntry["level"]) => void;
  clearLogs: () => void;
  updateSettings: (partial: Partial<ConversionSettings>) => void;
}

let logCounter = 0;

export const useAppStore = create<AppState>()(
  persist(
    (set) => ({
      settingsOpen: false,
      isConverting: false,
      dragTarget: null,
      logs: [],
      settings: {
        compression: "dxt5",
        generateMipmaps: true,
        outputDirectory: "",
      },

      setSettingsOpen: (open) => set({ settingsOpen: open }),

      setDragTarget: (target) => set({ dragTarget: target }),

      setConverting: (converting) => set({ isConverting: converting }),

      addLog: (message, level = "info") =>
        set((state) => ({
          logs: [
            {
              id: String(++logCounter),
              time: new Date().toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
                second: "2-digit",
              }),
              message,
              level,
            },
            ...state.logs,
          ].slice(0, 50),
        })),

      clearLogs: () => set({ logs: [] }),

      updateSettings: (partial) =>
        set((state) => ({
          settings: { ...state.settings, ...partial },
        })),
    }),
    {
      name: "blp-converter-settings",
      version: 1,
      partialize: (state) => ({ settings: state.settings }),
    },
  ),
);
