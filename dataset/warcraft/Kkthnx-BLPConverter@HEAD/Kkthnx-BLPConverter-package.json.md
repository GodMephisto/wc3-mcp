---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "package.json"
ref: HEAD
---

{
  "name": "blp-converter",
  "private": true,
  "version": "1.2.1",
  "license": "UNLICENSED",
  "author": "Kkthnx",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc -b && vite build",
    "preview": "vite preview",
    "tauri": "tauri",
    "build:icons": "powershell -NoProfile -File scripts/render-icon.ps1 && tauri icon app-icon-square.png -o src-tauri/icons",
    "build:shell-ext": "cargo build -p blp-shell-ext --release",
    "prebuild:shell-ext": "node scripts/prebuild-shell-ext.mjs",
    "prebuild:shell-ext:dev": "node scripts/prebuild-shell-ext-dev.mjs",
    "tauri:dev": "node scripts/prebuild-shell-ext-dev.mjs && tauri dev",
    "tauri:build": "tauri build"
  },
  "dependencies": {
    "@tauri-apps/api": "^2.9.1",
    "@tauri-apps/plugin-dialog": "^2.4.2",
    "i18next": "^26.3.1",
    "lucide-react": "^0.525.0",
    "react": "^19.1.0",
    "react-dom": "^19.1.0",
    "react-i18next": "^17.0.8",
    "zustand": "^5.0.6"
  },
  "devDependencies": {
    "@tauri-apps/cli": "^2.9.1",
    "@types/node": "^24.13.2",
    "@types/react": "^19.1.8",
    "@types/react-dom": "^19.1.6",
    "@vitejs/plugin-react": "^4.6.0",
    "autoprefixer": "^10.4.21",
    "postcss": "^8.5.6",
    "tailwindcss": "^3.4.17",
    "typescript": "^5.8.3",
    "vite": "^7.0.4"
  }
}
