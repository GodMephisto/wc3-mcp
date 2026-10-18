---
type: vendor-doc
source_repo: "https://github.com/barncastle/wow-mdx-viewer"
source_path: "tsconfig.json"
ref: HEAD
---

{
  "compilerOptions": {
    "module": "commonjs",
    "target": "esnext",
    "sourceMap": false,
    "outDir": "dist",
    "baseUrl": ".",
    "allowJs": true,
    "esModuleInterop": true,
    "paths": {
      "*": [
        "node_modules/*"
      ]
    }
  },
  "include": [
    "src/**/*"
  ]
}