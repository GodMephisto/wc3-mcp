---
type: vendor-doc
source_repo: "https://github.com/Trisard/wc3-battleships-companion"
source_path: "src-tauri/tauri.conf.json"
ref: HEAD
---

{
  "$schema": "https://schema.tauri.app/config/2",
  "productName": "Battleships Companion",
  "version": "0.1.0",
  "identifier": "com.tristan.battleships-companion",
  "build": {
    "beforeDevCommand": "npm run dev",
    "devUrl": "http://localhost:1420",
    "beforeBuildCommand": "npm run build",
    "frontendDist": "../dist"
  },
  "app": {
    "windows": [
      {
        "title": "Battleships Companion",
        "width": 1000,
        "height": 700
      }
    ],
    "security": {
      "csp": null
    }
  },
  "bundle": {
    "active": true,
    "targets": [],
    "icon": [
      "icons/32x32.png",
      "icons/128x128.png",
      "icons/128x128@2x.png",
      "icons/icon.icns",
      "icons/icon.ico"
    ]
  }
}
