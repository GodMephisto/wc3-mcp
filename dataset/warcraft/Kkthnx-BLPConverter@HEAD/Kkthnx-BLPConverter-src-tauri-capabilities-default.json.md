---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/capabilities/default.json"
ref: HEAD
---

{
  "$schema": "https://schema.tauri.app/config/2",
  "identifier": "com.blpconverter.desktop",
  "description": "BLP Converter capabilities",
  "windows": ["main"],
  "permissions": [
    "core:default",
    "core:event:default",
    "core:webview:default",
    "core:window:default",
    "dialog:default"
  ]
}
