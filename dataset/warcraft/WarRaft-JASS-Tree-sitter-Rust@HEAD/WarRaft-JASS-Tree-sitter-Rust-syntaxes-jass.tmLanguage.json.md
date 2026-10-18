---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "syntaxes/jass.tmLanguage.json"
ref: HEAD
---

{
    "$schema": "https://raw.githubusercontent.com/martinring/tmlanguage/master/tmlanguage.json",
    "name": "JASS",
    "scopeName": "source.jass",
    "patterns": [
        { "include": "#comment" },
        { "include": "#string" }
    ],
    "repository": {
        "comment": {
            "name": "comment.line.double-slash.jass",
            "match": "//.*$"
        },
        "string": {
            "name": "string.quoted.single.jass",
            "begin": "'",
            "end": "'|$",
            "patterns": [
                {
                    "name": "constant.character.escape.jass",
                    "match": "\\\\."
                }
            ]
        }
    }
}

