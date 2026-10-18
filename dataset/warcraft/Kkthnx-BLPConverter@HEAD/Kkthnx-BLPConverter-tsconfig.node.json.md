---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "tsconfig.node.json"
ref: HEAD
---

{
  "compilerOptions": {
    "target": "ES2022",
    "lib": ["ES2023"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "strict": true,
    "types": ["node"]
  },
  "include": ["vite.config.ts"]
}
