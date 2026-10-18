---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/tsconfig.json"
ref: HEAD
---

{
  "extends": "@tsconfig/svelte/tsconfig.json",
  "compilerOptions": {
    "target": "ESNext",
    "useDefineForClassFields": true,
    "module": "ESNext",
    "moduleResolution": "bundler",
    "resolveJsonModule": true,
    "baseUrl": ".",
    "ignoreDeprecations": "6.0",
    "strict": false,
    "paths": {
      "$lib": ["./src/lib"],
      "$lib/*": ["./src/lib/*"],
      "$components": ["./src/lib/components"],
      "$components/*": ["./src/lib/components/*"]
    },
    /**
     * Typecheck JS in `.svelte` and `.js` files by default.
     * Disable checkJs if you'd like to use dynamic types in JS.
     * Note that setting allowJs false does not prevent the use
     * of JS in `.svelte` files.
     */
    "allowJs": true,
    "checkJs": true,
    "isolatedModules": true
  },
  "include": [
    "src/**/*.d.ts",
    "src/**/*.ts",
    "src/**/*.js",
    "src/**/*.svelte"
  ],
  "references": [
    {
      "path": "./tsconfig.node.json"
    }
  ]
}
