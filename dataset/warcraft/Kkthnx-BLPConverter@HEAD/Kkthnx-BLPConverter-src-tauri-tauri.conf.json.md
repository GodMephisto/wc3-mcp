---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/tauri.conf.json"
ref: HEAD
---

{
  "$schema": "https://schema.tauri.app/config/2",
  "productName": "BLP Converter",
  "version": "1.2.1",
  "identifier": "com.blpconverter.desktop",
  "build": {
    "beforeDevCommand": "npm run dev",
    "devUrl": "http://localhost:1420",
    "beforeBuildCommand": "node scripts/prebuild-shell-ext.mjs && npm run build",
    "frontendDist": "../dist"
  },
  "app": {
    "withGlobalTauri": false,
    "windows": [
      {
        "title": "BLP Converter",
        "width": 570,
        "height": 340,
        "minWidth": 480,
        "minHeight": 300,
        "resizable": true,
        "fullscreen": false,
        "decorations": true,
        "dragDropEnabled": true
      }
    ],
    "security": {
      "csp": "default-src 'self'; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'"
    }
  },
  "bundle": {
    "active": true,
    "targets": "all",
    "icon": [
      "icons/32x32.png",
      "icons/128x128.png",
      "icons/128x128@2x.png",
      "icons/icon.icns",
      "icons/icon.ico"
    ]
  }
}
