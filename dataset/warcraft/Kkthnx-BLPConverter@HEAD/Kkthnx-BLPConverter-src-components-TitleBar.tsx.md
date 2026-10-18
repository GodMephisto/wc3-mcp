---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/components/TitleBar.tsx"
ref: HEAD
---

import { Settings } from "lucide-react";
import { useTranslation } from "react-i18next";
import { APP_NAME } from "../constants/app";
import { useAppStore } from "../store/useAppStore";

export function TitleBar() {
  const { t } = useTranslation();
  const setSettingsOpen = useAppStore((s) => s.setSettingsOpen);
  const outputDirectory = useAppStore((s) => s.settings.outputDirectory);
  const isConverting = useAppStore((s) => s.isConverting);

  return (
    <header className="flex shrink-0 items-center justify-between border-b border-workspace-border bg-workspace-panel px-4 py-2.5">
      <div className="flex items-center gap-2.5">
        <img
          src="/app-icon.svg"
          alt=""
          className="h-7 w-7 shrink-0 rounded-md ring-1 ring-workspace-border"
        />
        <div>
          <h1 className="text-sm font-semibold leading-none text-white">
            {APP_NAME}
          </h1>
          <p className="mt-0.5 text-[10px] text-workspace-silver-muted">
            {t("app.dropToConvert")}
          </p>
        </div>
      </div>

      <div className="flex items-center gap-2">
        {isConverting && (
          <span className="text-[10px] font-medium text-workspace-accent animate-pulse">
            {t("app.converting")}
          </span>
        )}
        {outputDirectory && (
          <span
            className="hidden max-w-[140px] truncate text-[10px] text-workspace-silver-muted sm:inline"
            title={outputDirectory}
          >
            {outputDirectory}
          </span>
        )}
        <button
          type="button"
          onClick={() => setSettingsOpen(true)}
          className="flex h-7 w-7 items-center justify-center rounded-md border border-workspace-border text-workspace-silver-muted transition hover:border-workspace-accent/40 hover:text-workspace-accent"
          aria-label={t("settings.title")}
        >
          <Settings className="h-3.5 w-3.5" />
        </button>
      </div>
    </header>
  );
}
