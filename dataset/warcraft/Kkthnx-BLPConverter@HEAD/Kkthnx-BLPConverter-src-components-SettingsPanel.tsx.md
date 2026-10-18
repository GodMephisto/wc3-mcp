---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/components/SettingsPanel.tsx"
ref: HEAD
---

import { Settings } from "lucide-react";
import { useTranslation } from "react-i18next";
import type { CompressionFormat } from "../types";
import { useAppStore } from "../store/useAppStore";
import { pickOutputDirectory } from "../lib/tauri";
import { BlpViewSection } from "./BlpViewSection";
import { AboutSection } from "./AboutSection";
import {
  setAppLocale,
  SUPPORTED_LOCALES,
  type SupportedLocale,
} from "../i18n";

const COMPRESSION_OPTIONS: {
  value: CompressionFormat;
  labelKey: string;
}[] = [
  { value: "raw", labelKey: "settings.compressionRaw" },
  { value: "dxt1", labelKey: "settings.compressionDxt1" },
  { value: "dxt3", labelKey: "settings.compressionDxt3" },
  { value: "dxt5", labelKey: "settings.compressionDxt5" },
];

export function SettingsPanel() {
  const { t, i18n } = useTranslation();
  const settingsOpen = useAppStore((s) => s.settingsOpen);
  const settings = useAppStore((s) => s.settings);
  const setSettingsOpen = useAppStore((s) => s.setSettingsOpen);
  const updateSettings = useAppStore((s) => s.updateSettings);
  const currentLocale = i18n.language as SupportedLocale;

  if (!settingsOpen) {
    return null;
  }

  const handlePickOutput = async () => {
    const dir = await pickOutputDirectory();
    if (dir) {
      updateSettings({ outputDirectory: dir });
    }
  };

  const clearOutput = () => {
    updateSettings({ outputDirectory: "" });
  };

  const handleLocaleChange = (locale: SupportedLocale) => {
    setAppLocale(locale);
  };

  return (
    <>
      <div
        className="fixed inset-0 z-40 bg-black/60 backdrop-blur-sm"
        onClick={() => setSettingsOpen(false)}
        aria-hidden
      />
      <aside
        className="fixed right-0 top-0 z-50 flex h-full w-full max-w-xs flex-col border-l border-workspace-border bg-workspace-panel shadow-panel"
        role="dialog"
        aria-label={t("settings.title")}
      >
        <div className="flex items-center justify-between border-b border-workspace-border px-4 py-2.5">
          <div className="flex items-center gap-2">
            <Settings className="h-4 w-4 text-workspace-accent" />
            <h2 className="text-sm font-semibold text-white">
              {t("settings.title")}
            </h2>
          </div>
          <button
            type="button"
            onClick={() => setSettingsOpen(false)}
            className="text-xs text-workspace-silver-muted transition hover:text-white"
          >
            {t("settings.close")}
          </button>
        </div>

        <div className="flex-1 space-y-5 overflow-auto p-4">
          <section>
            <h3 className="mb-2 text-[10px] font-semibold uppercase tracking-wider text-workspace-silver-muted">
              {t("settings.language")}
            </h3>
            <select
              value={currentLocale}
              onChange={(e) =>
                handleLocaleChange(e.target.value as SupportedLocale)
              }
              className="w-full rounded-lg border border-workspace-border bg-workspace-surface px-3 py-2 text-xs text-workspace-silver outline-none transition focus:border-workspace-accent/40"
            >
              {SUPPORTED_LOCALES.map((locale) => (
                <option key={locale.code} value={locale.code}>
                  {locale.label}
                </option>
              ))}
            </select>
          </section>

          <section>
            <h3 className="mb-2 text-[10px] font-semibold uppercase tracking-wider text-workspace-silver-muted">
              {t("settings.outputFolder")}
            </h3>
            <p className="mb-2 text-[10px] leading-relaxed text-workspace-silver-muted">
              {t("settings.outputFolderHint")}
            </p>
            <button
              type="button"
              onClick={handlePickOutput}
              className="w-full rounded-lg border border-workspace-border px-3 py-2 text-left text-xs text-workspace-silver transition hover:border-workspace-accent/40"
            >
              {settings.outputDirectory || t("settings.outputDefault")}
            </button>
            {settings.outputDirectory && (
              <button
                type="button"
                onClick={clearOutput}
                className="mt-1.5 text-[10px] text-workspace-silver-muted transition hover:text-workspace-accent"
              >
                {t("settings.resetOutput")}
              </button>
            )}
          </section>

          <section>
            <h3 className="mb-2 text-[10px] font-semibold uppercase tracking-wider text-workspace-silver-muted">
              {t("settings.compression")}
            </h3>
            <div className="space-y-1.5">
              {COMPRESSION_OPTIONS.map((option) => (
                <label
                  key={option.value}
                  className={`flex cursor-pointer items-center gap-2 rounded-md border px-2.5 py-2 text-xs transition ${
                    settings.compression === option.value
                      ? "border-workspace-accent/50 bg-workspace-accent/10 text-workspace-accent"
                      : "border-workspace-border text-workspace-silver hover:border-workspace-silver/30"
                  }`}
                >
                  <input
                    type="radio"
                    name="compression"
                    value={option.value}
                    checked={settings.compression === option.value}
                    onChange={() =>
                      updateSettings({ compression: option.value })
                    }
                    className="accent-workspace-accent"
                  />
                  {t(option.labelKey)}
                </label>
              ))}
            </div>
          </section>

          <section>
            <label className="flex cursor-pointer items-center gap-2 rounded-md border border-workspace-border px-2.5 py-2 text-xs text-workspace-silver">
              <input
                type="checkbox"
                checked={settings.generateMipmaps}
                onChange={(e) =>
                  updateSettings({ generateMipmaps: e.target.checked })
                }
                className="accent-workspace-accent"
              />
              {t("settings.generateMipmaps")}
            </label>
          </section>

          <BlpViewSection />
          <AboutSection />
        </div>
      </aside>
    </>
  );
}
