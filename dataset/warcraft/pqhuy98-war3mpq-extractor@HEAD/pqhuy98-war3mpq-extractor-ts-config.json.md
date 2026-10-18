---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "ts-config.json"
ref: HEAD
---

{
  "compilerOptions": {
    "target": "es2020",
    "module": "esnext",
    "lib": ["es2020"],
    "esModuleInterop": true,
    "strict": true,
    "skipLibCheck": true
  },
  "ts-node": {
    "compilerOptions": {
      "module": "nodenext"
    },
    "esm": true
  },
  "include": ["src/**/*.ts"]
}