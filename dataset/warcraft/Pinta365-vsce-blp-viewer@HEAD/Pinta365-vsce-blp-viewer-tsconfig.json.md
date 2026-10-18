---
type: vendor-doc
source_repo: "https://github.com/Pinta365/vsce-blp-viewer"
source_path: "tsconfig.json"
ref: HEAD
---

{
  "compilerOptions": {
    "module": "node16",
    "moduleResolution": "node16",
    "target": "ES2020",
    "lib": ["ES2020"],
    "outDir": "./out",
    "rootDir": "./src",
    "strict": true,
    "esModuleInterop": true,
    "skipLibCheck": true
  },
  "include": ["src"],
  "exclude": ["node_modules", ".vscode-test"]
}
