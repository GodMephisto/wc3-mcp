---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "package.json"
ref: HEAD
---

{
    "name": "lost_world_dota",
    "private": true,
    "scripts": {
        "postinstall": "node scripts/install.js",
        "launch": "node scripts/launch.js",
        "build": "run-p build:*",
        "build:panorama": "tsc --project src/panorama/tsconfig.json",
        "build:vscripts": "tstl --project src/vscripts/tsconfig.json",
        "dev": "run-p dev:*",
        "dev:panorama": "tsc --project src/panorama/tsconfig.json --watch",
        "dev:vscripts": "tstl --project src/vscripts/tsconfig.json --watch"
    },
    "devDependencies": {
        "@moddota/find-steam-app": "^1.1.0",
        "@moddota/dota-lua-types": "^4.28.0",
        "@moddota/panorama-types": "^1.28.0",
        "fs-extra": "^9.0.0",
        "npm-run-all": "^4.1.5",
        "typescript": "^5.2.2",
        "typescript-to-lua": "^1.18.0"
    }
}
