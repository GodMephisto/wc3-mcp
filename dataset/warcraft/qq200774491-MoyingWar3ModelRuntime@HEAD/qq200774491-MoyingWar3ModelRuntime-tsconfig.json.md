---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "tsconfig.json"
ref: HEAD
---

{
  "compilerOptions": {
    "target": "ES2022",
    "useDefineForClassFields": true,
    "module": "ESNext",
    "moduleResolution": "Bundler",
    "baseUrl": ".",
    "paths": {
      "@moying/war3-model-runtime": ["./src/index.ts"]
    },
    "lib": ["ES2022", "DOM", "DOM.Iterable"],
    "types": ["@webgpu/types"],
    "strict": true,
    "noEmit": true,
    "noFallthroughCasesInSwitch": true,
    "skipLibCheck": true,
    "verbatimModuleSyntax": false
  },
  "include": ["src/**/*.ts", "examples/**/*.ts", "vite.config.ts"]
}
