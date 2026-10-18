---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "syntaxes/jass.language-configuration.json"
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
    ],
    "indentationRules": {
        "increaseIndentPattern": "^\\s*(function|if|else|elseif|loop|globals)\\b",
        "decreaseIndentPattern": "^\\s*(endfunction|endif|else|elseif|endloop|endglobals)\\b"
    }
}
