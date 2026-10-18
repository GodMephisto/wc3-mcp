---
type: vendor-doc
source_repo: "https://github.com/voces/w3xdata"
source_path: "tsconfig.json"
ref: HEAD
---

{
  "compilerOptions": {
    "rootDir": "src",
    "outDir": "dist",
    "target": "es2022",
    "lib": ["es2022", "dom"],
    "module": "Node16",
    "moduleResolution": "Node16",
    "strict": true,
    "isolatedModules": true,
    "declaration": true,
    "types": ["@types/node", "@types/jest"],
    "esModuleInterop": true,
    "skipLibCheck": true
  },
  "include": ["src"]
}
