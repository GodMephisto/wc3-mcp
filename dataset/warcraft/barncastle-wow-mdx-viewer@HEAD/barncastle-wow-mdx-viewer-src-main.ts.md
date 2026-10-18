---
type: vendor-doc
source_repo: "https://github.com/barncastle/wow-mdx-viewer"
source_path: "src/main.ts"
ref: HEAD
---

import { app, BrowserWindow } from "electron";
import * as path from "path";

function createWindow() {
  const mainWindow = new BrowserWindow({
    height: 1024,
    width: 1280,
    webPreferences: {
      preload: path.join(__dirname, "preview.js"),
      worldSafeExecuteJavaScript: true,
      contextIsolation: true,
      nodeIntegration: true,
      enableRemoteModule: true
    },
  });

  mainWindow.loadFile(path.join(__dirname, "index.html"));
}

app.on("ready", () => {
  createWindow();

  app.on("activate", function () {
    if (BrowserWindow.getAllWindows().length === 0)
      createWindow();
  });
});

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") {
    app.quit();
  }
});
