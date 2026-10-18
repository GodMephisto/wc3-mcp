---
type: vendor-doc
source_repo: "https://github.com/InsanityAI/Workbench-vJASS"
source_path: "jass.config.json"
ref: HEAD
---

{
    "excludes": [
        "**/node_modules/**",
        "**/.git/**",
        "**/dist/**",
        "**/build/**"
    ],
    "includes": [
        "**/*.j",
        "**/*.jass",
        "**/*.ai",
        "**/*.zn"
    ],
    "parsing": {
        "enableTextMacro": true,
        "enablePreprocessor": true,
        "enableLuaBlocks": false,
        "strictMode": false
    },
    "standardLibraries": {
        "common.j": "./libs/common.j",
        "common.ai": "./libs/common.ai",
        "blizzard.j": "./libs/blizzard.j"
    },
    "diagnostics": {
        "enable": true,
        "severity": {
            "errors": "error",
            "warnings": "warning"
        },
        "checkTypes": true,
        "checkUndefined": true,
        "checkUnused": false,
        "checkArrayBounds": true
    }
}