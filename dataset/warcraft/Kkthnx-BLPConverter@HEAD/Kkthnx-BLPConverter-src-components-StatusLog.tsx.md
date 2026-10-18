---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/components/StatusLog.tsx"
ref: HEAD
---

import { Trash2 } from "lucide-react";
import { useTranslation } from "react-i18next";
import { useAppStore } from "../store/useAppStore";

const LEVEL_STYLES = {
  info: "text-workspace-silver-muted",
  success: "text-emerald-400",
  error: "text-red-400",
} as const;

export function StatusLog() {
  const { t } = useTranslation();
  const logs = useAppStore((s) => s.logs);
  const clearLogs = useAppStore((s) => s.clearLogs);

  return (
    <footer className="flex shrink-0 flex-col border-t border-workspace-border bg-workspace-panel">
      <div className="flex items-center justify-between px-3 py-1.5">
        <span className="text-[10px] font-medium uppercase tracking-wider text-workspace-silver-muted">
          {t("activity.title")}
        </span>
        {logs.length > 0 && (
          <button
            type="button"
            onClick={clearLogs}
            className="flex items-center gap-1 text-[10px] text-workspace-silver-muted transition hover:text-workspace-silver"
          >
            <Trash2 className="h-3 w-3" />
            {t("activity.clear")}
          </button>
        )}
      </div>

      <div className="h-[72px] overflow-y-auto px-3 pb-2">
        {logs.length === 0 ? (
          <p className="py-2 text-[11px] text-workspace-silver-muted/60">
            {t("activity.empty")}
          </p>
        ) : (
          <ul className="space-y-0.5">
            {logs.map((entry) => (
              <li
                key={entry.id}
                className={`truncate text-[11px] leading-relaxed ${LEVEL_STYLES[entry.level]}`}
                title={entry.message}
              >
                <span className="mr-1.5 text-workspace-silver-muted/50">
                  {entry.time}
                </span>
                {entry.message}
              </li>
            ))}
          </ul>
        )}
      </div>
    </footer>
  );
}
