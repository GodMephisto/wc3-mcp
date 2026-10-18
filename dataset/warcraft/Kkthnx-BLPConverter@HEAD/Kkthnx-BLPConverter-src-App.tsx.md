---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/App.tsx"
ref: HEAD
---

import { useTranslation } from "react-i18next";
import { TitleBar } from "./components/TitleBar";
import { ConvertPanel } from "./components/ConvertPanel";
import { StatusLog } from "./components/StatusLog";
import { SettingsPanel } from "./components/SettingsPanel";
import { useDragDrop } from "./hooks/useDragDrop";
import { useAppStore } from "./store/useAppStore";

function App() {
  useDragDrop();

  const { t } = useTranslation();
  const dragTarget = useAppStore((s) => s.dragTarget);
  const isConverting = useAppStore((s) => s.isConverting);

  return (
    <div className="flex h-full flex-col">
      <TitleBar />

      <div
        className="h-0.5 w-full shrink-0 overflow-hidden bg-workspace-border/30"
        role="progressbar"
        aria-label={t("app.converting")}
        aria-hidden={!isConverting}
      >
        {isConverting && (
          <div className="h-full w-1/4 rounded-full bg-workspace-accent shadow-accent animate-indeterminate" />
        )}
      </div>

      <main className="flex flex-1 gap-3 overflow-hidden p-3">
        <ConvertPanel direction="to-png" active={dragTarget === "to-png"} />
        <ConvertPanel direction="to-blp" active={dragTarget === "to-blp"} />
      </main>

      <StatusLog />
      <SettingsPanel />
    </div>
  );
}

export default App;
