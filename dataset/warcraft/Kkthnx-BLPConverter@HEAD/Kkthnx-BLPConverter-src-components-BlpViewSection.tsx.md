---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/components/BlpViewSection.tsx"
ref: HEAD
---

import { useCallback, useEffect, useState } from "react";
import { CheckCircle2, Monitor, RefreshCw, Unplug } from "lucide-react";
import { useTranslation } from "react-i18next";
import type { BlpViewStatus } from "../types";
import {
  getBlpViewStatus,
  installBlpView,
  restartExplorer,
  uninstallBlpView,
} from "../lib/tauri";

export function BlpViewSection() {
  const { t } = useTranslation();
  const [status, setStatus] = useState<BlpViewStatus | null>(null);
  const [loading, setLoading] = useState(false);
  const [feedback, setFeedback] = useState<string | null>(null);

  const refreshStatus = useCallback(async () => {
    try {
      setStatus(await getBlpViewStatus());
    } catch {
      setFeedback(t("blpview.statusError"));
    }
  }, [t]);

  useEffect(() => {
    void refreshStatus();
  }, [refreshStatus]);

  const handleInstall = async () => {
    setLoading(true);
    setFeedback(null);
    try {
      const result = await installBlpView();
      setFeedback(result.message);
      await refreshStatus();
      if (result.restartRequired) {
        await restartExplorer();
      }
    } catch (err) {
      setFeedback(
        err instanceof Error ? err.message : t("blpview.installFailed"),
      );
    } finally {
      setLoading(false);
    }
  };

  const handleUninstall = async () => {
    setLoading(true);
    setFeedback(null);
    try {
      const result = await uninstallBlpView();
      setFeedback(result.message);
      await refreshStatus();
      if (result.restartRequired) {
        await restartExplorer();
      }
    } catch (err) {
      setFeedback(
        err instanceof Error ? err.message : t("blpview.uninstallFailed"),
      );
    } finally {
      setLoading(false);
    }
  };

  const unsupported = status !== null && !status.supported;
  const controlsDisabled = loading || unsupported || status === null;

  return (
    <section>
      <h3 className="mb-2 text-[10px] font-semibold uppercase tracking-wider text-workspace-silver-muted">
        {t("blpview.title")}
      </h3>

      <div
        className={`space-y-2 rounded-lg border border-workspace-border bg-workspace-surface p-2.5 ${
          unsupported ? "opacity-50" : ""
        }`}
      >
        <div className="flex items-start gap-2">
          {status?.installed ? (
            <CheckCircle2 className="mt-0.5 h-3.5 w-3.5 shrink-0 text-emerald-400" />
          ) : (
            <Monitor className="mt-0.5 h-3.5 w-3.5 shrink-0 text-workspace-silver-muted" />
          )}
          <p className="text-[11px] leading-relaxed text-workspace-silver-muted">
            {unsupported
              ? t("blpview.unsupportedReason")
              : (status?.message ?? t("blpview.loadingStatus"))}
          </p>
        </div>

        <div className="flex flex-wrap gap-1.5">
          {!status?.installed ? (
            <button
              type="button"
              disabled={controlsDisabled}
              onClick={handleInstall}
              className="rounded-md bg-workspace-accent/15 px-2.5 py-1.5 text-[11px] font-medium text-workspace-accent ring-1 ring-workspace-accent/30 disabled:cursor-not-allowed disabled:opacity-40"
            >
              {t("blpview.install")}
            </button>
          ) : (
            <button
              type="button"
              disabled={controlsDisabled}
              onClick={handleUninstall}
              className="rounded-md border border-workspace-border px-2.5 py-1.5 text-[11px] text-workspace-silver-muted hover:text-red-400 disabled:cursor-not-allowed disabled:opacity-40"
            >
              <Unplug className="mr-1 inline h-3 w-3" />
              {t("blpview.uninstall")}
            </button>
          )}
          <button
            type="button"
            disabled={controlsDisabled}
            onClick={async () => {
              try {
                await restartExplorer();
              } catch (err) {
                setFeedback(
                  err instanceof Error
                    ? err.message
                    : t("blpview.restartFailed"),
                );
              }
            }}
            className="rounded-md border border-workspace-border px-2.5 py-1.5 text-[11px] text-workspace-silver-muted hover:text-workspace-accent disabled:cursor-not-allowed disabled:opacity-40"
          >
            <RefreshCw className="mr-1 inline h-3 w-3" />
            {t("blpview.restartExplorer")}
          </button>
        </div>

        {feedback && (
          <p className="text-[10px] text-workspace-accent">{feedback}</p>
        )}
      </div>
    </section>
  );
}
