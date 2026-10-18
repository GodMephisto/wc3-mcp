---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "syntaxes/as.language-configuration.json"
ref: HEAD
---

{
    "comments": {
        "lineComment": "//"
    },
    "brackets": [
        ["[", "]"],
        ["(", ")"]
    ],
    "autoClosingPairs": [
        { "open": "[", "close": "]", "notIn": ["string", "comment"] },
        { "open": "(", "close": ")", "notIn": ["string", "comment"] },
        { "open": "'", "close": "'", "notIn": ["string", "comment"] }
    ],
    "surroundingPairs": [
        ["[", "]"],
        ["(", ")"],
        ["'", "'"]
    ]
}
