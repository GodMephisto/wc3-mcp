---
type: vendor-doc
source_repo: "https://github.com/Pinta365/vsce-blp-viewer"
source_path: "src/BlpEditorProvider.ts"
ref: HEAD
---

import * as vscode from "vscode";
import * as fs from "fs";

export class BlpEditorProvider implements vscode.CustomReadonlyEditorProvider {

    public static register(context: vscode.ExtensionContext): vscode.Disposable {
        return vscode.window.registerCustomEditorProvider(
            "blpviewer.preview",
            new BlpEditorProvider(context),
            {
                webviewOptions: { retainContextWhenHidden: true },
                supportsMultipleEditorsPerDocument: false,
            }
        );
    }

    constructor(private readonly context: vscode.ExtensionContext) {}

    openCustomDocument(uri: vscode.Uri): vscode.CustomDocument {
        return { uri, dispose: () => {} };
    }

    async resolveCustomEditor(
        document: vscode.CustomDocument,
        webviewPanel: vscode.WebviewPanel
    ): Promise<void> {
        webviewPanel.webview.options = {
            enableScripts: true,
            localResourceRoots: [
                vscode.Uri.joinPath(this.context.extensionUri, "media"),
            ],
        };

        webviewPanel.webview.html = this.getHtml(webviewPanel.webview);

        try {
            const raw = await fs.promises.readFile(document.uri.fsPath);
            const { decodeBlpData, isBLP1Header, parseBlpHeader } = await import("@pinta365/blp");
            const bytes = new Uint8Array(raw);
            const decoded = decodeBlpData(bytes);
            const header = parseBlpHeader(bytes);
            const blp1 = isBLP1Header(header);

            const mipCount = header.mipOffsets.filter((o: number) => o > 0).length;

            webviewPanel.webview.postMessage({
                type: "image",
                width: decoded.width,
                height: decoded.height,
                pixels: Array.from(decoded.pixels),
                meta: {
                    version: header.magic,
                    content: blp1 ? header.content : undefined,
                    compression: blp1 ? undefined : header.compression,
                    alphaBits: blp1 ? header.alphaBitDepth : header.alphaSize,
                    preferredFormat: blp1 ? undefined : header.preferredFormat,
                    hasMips: blp1 ? header.hasMipmaps : header.hasMips,
                    mipCount,
                    fileSize: raw.length,
                },
            });
        } catch (err) {
            webviewPanel.webview.postMessage({
                type: "error",
                message: String(err),
            });
        }
    }

    private getHtml(webview: vscode.Webview): string {
        const scriptUri = webview.asWebviewUri(
            vscode.Uri.joinPath(this.context.extensionUri, "media", "preview.js")
        );

        return /* html */ `<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="Content-Security-Policy" content="default-src 'none'; script-src ${webview.cspSource}; style-src 'unsafe-inline';">
    <title>BLP Preview</title>
    <style>
        body {
            margin: 0;
            padding: 16px;
            background: var(--vscode-editor-background);
            color: var(--vscode-editor-foreground);
            font-family: var(--vscode-font-family);
            display: flex;
            flex-direction: column;
            align-items: center;
        }
        canvas {
            max-width: 100%;
            image-rendering: pixelated;
            border: 1px solid var(--vscode-panel-border);
        }
        #meta {
            margin-top: 12px;
            font-size: 12px;
            opacity: 0.7;
            display: grid;
            grid-template-columns: auto auto;
            gap: 2px 16px;
        }
        #meta dt { font-weight: bold; text-align: right; }
        #meta dd { margin: 0; }
        #error {
            color: var(--vscode-errorForeground);
        }
    </style>
</head>
<body>
    <canvas id="canvas" style="display:none;"></canvas>
    <dl id="meta" style="display:none;"></dl>
    <div id="error" style="display:none;"></div>
    <script src="${scriptUri}"></script>
</body>
</html>`;
    }
}
