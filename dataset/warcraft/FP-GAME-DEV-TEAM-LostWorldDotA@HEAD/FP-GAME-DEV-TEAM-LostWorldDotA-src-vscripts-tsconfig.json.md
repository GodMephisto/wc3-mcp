---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/tsconfig.json"
ref: HEAD
---

{
    "compilerOptions": {
        "rootDir": ".",
        "outDir": "../../game/scripts/vscripts",
        "target": "esnext",
        "lib": ["esnext"],
        "types": ["@moddota/dota-lua-types/normalized"],
        "plugins": [{ "transform": "@moddota/dota-lua-types/transformer" }],
        "moduleResolution": "node",
        "strict": true
    },
    "tstl": {
        "luaTarget": "JIT",
        "sourceMapTraceback": true
    },
    "include": ["**/*.ts", "../common/**/*.ts"]
}
