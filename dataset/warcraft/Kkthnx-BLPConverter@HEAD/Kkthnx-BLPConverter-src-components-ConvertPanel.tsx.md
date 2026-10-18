---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/components/ConvertPanel.tsx"
ref: HEAD
---

import { ArrowRight, FileImage, ImageIcon } from "lucide-react";
import { useTranslation } from "react-i18next";
import type { ConvertDirection } from "../types";

interface ConvertPanelProps {
  direction: ConvertDirection;
  active: boolean;
}

export function ConvertPanel({ direction, active }: ConvertPanelProps) {
  const { t } = useTranslation();
  const isToPng = direction === "to-png";
  const Icon = isToPng ? FileImage : ImageIcon;

  return (
    <div
      className={`relative flex flex-1 flex-col items-center justify-center gap-3 rounded-xl border-2 border-dashed p-5 transition-all ${
        active
          ? "border-workspace-accent bg-workspace-accent/5 shadow-accent"
          : "border-workspace-border/80 bg-workspace-surface/40 hover:border-workspace-silver/30"
      }`}
    >
      <div className="relative flex items-center gap-3">
        <div
          className={`flex flex-col items-center rounded-lg border bg-workspace-panel px-6 py-4 transition ${
            active ? "border-workspace-accent/40" : "border-workspace-border"
          }`}
        >
          <Icon
            className={`mb-2 h-10 w-10 ${active ? "text-workspace-accent" : "text-workspace-silver"}`}
            strokeWidth={1.25}
          />
          <span className="text-lg font-bold tracking-wide text-white">
            {isToPng ? "BLP" : "PNG"}
          </span>
        </div>

        <ArrowRight className="h-8 w-8 shrink-0 text-workspace-accent/70" />

        <div className="flex flex-col items-center rounded-lg border border-workspace-border/60 bg-workspace-elevated/50 px-5 py-4">
          <span className="text-lg font-bold tracking-wide text-workspace-accent">
            {isToPng ? "PNG" : "BLP"}
          </span>
        </div>
      </div>

      <div className="text-center">
        <p className="text-xs font-medium text-workspace-silver">
          {t(isToPng ? "panels.toPng.hint" : "panels.toBlp.hint")}
        </p>
        <p className="mt-0.5 text-[10px] text-workspace-silver-muted">
          {t(isToPng ? "panels.toPng.subhint" : "panels.toBlp.subhint")}
        </p>
      </div>
    </div>
  );
}
