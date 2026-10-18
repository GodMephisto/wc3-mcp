---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "tree-sitter.json"
ref: HEAD
---

{
  "$schema": "https://tree-sitter.github.io/tree-sitter/assets/schemas/config.schema.json",
  "grammars": [
    {
      "name": "vjass",
      "camelcase": "VJASS",
      "scope": "source.vjass",
      "file-types": [
        "vj"
      ]
    }
  ],
  "metadata": {
    "version": "0.1.0",
    "license": "MIT",
    "description": "VJASS grammar for tree-sitter",
    "authors": [
      {
        "name": "nazarpunk",
        "email": "nazarpunk@gmail.com"
      }
    ],
    "links": {
      "repository": "https://github.com/WarRaft/tree-sitter-vjass"
    }
  },
  "bindings": {
    "go": false,
    "python": false,
    "swift": false,
    "node": true
  }
}
