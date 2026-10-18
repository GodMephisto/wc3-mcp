---
type: vendor-doc
source_repo: "https://github.com/Pinta365/vsce-blp-viewer"
source_path: "src/extension.ts"
ref: HEAD
---

import * as vscode from "vscode";
import { BlpEditorProvider } from "./BlpEditorProvider";

export function activate(context: vscode.ExtensionContext) {
    context.subscriptions.push(BlpEditorProvider.register(context));
}

export function deactivate() {}
