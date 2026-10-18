---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/panorama/tsconfig.json"
ref: HEAD
---

{
    "compilerOptions": {
        "rootDir": ".",
        "outDir": "../../content/panorama/scripts/custom_game",
        "target": "es2017",
        "lib": ["es2017"],
        "types": ["@moddota/panorama-types"],
        "moduleResolution": "node",
        "strict": true
    },
    "include": ["**/*.ts", "../common/**/*.ts"]
}
