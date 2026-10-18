---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/components/AboutSection.tsx"
ref: HEAD
---

import { useTranslation } from "react-i18next";
import { APP_AUTHOR, APP_NAME, APP_VERSION } from "../constants/app";

export function AboutSection() {
  const { t } = useTranslation();

  return (
    <section>
      <h3 className="mb-2 text-[10px] font-semibold uppercase tracking-wider text-workspace-silver-muted">
        {t("about.title")}
      </h3>

      <div className="space-y-3 rounded-lg border border-workspace-border bg-workspace-surface p-3">
        <div className="flex items-center gap-3">
          <img
            src="/app-icon.svg"
            alt=""
            className="h-10 w-10 shrink-0 rounded-lg ring-1 ring-workspace-border"
          />
          <div>
            <p className="text-sm font-semibold text-white">{APP_NAME}</p>
            <p className="text-[10px] text-workspace-silver-muted">
              v{APP_VERSION}
            </p>
          </div>
        </div>

        <p className="text-[11px] leading-relaxed text-workspace-silver-muted">
          {t("app.tagline")}
        </p>

        <p className="text-[11px] leading-relaxed text-workspace-silver-muted/80">
          {t("app.description")}
        </p>

        <p className="border-t border-workspace-border pt-2 text-[10px] text-workspace-silver-muted">
          {t("app.madeBy", { author: APP_AUTHOR })}
        </p>
      </div>
    </section>
  );
}
