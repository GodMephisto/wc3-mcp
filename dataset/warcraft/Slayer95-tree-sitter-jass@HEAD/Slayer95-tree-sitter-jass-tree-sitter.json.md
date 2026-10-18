---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "tree-sitter.json"
ref: HEAD
---

{
  "$schema": "https://tree-sitter.github.io/tree-sitter/assets/schemas/config.schema.json",
  "grammars": [
    {
      "name": "jass",
      "camelcase": "JASS",
      "title": "JASS",
      "scope": "source.jass",
      "file-types": [
        "j"
      ],
      "injection-regex": "^jass$",
      "class-name": "TreeSitterJass"
    }
  ],
  "metadata": {
    "version": "0.1.0",
    "license": "MIT",
    "description": "Parser for the JASS language",
    "authors": [
      {
        "name": "Leonardo Julca",
        "email": "ivojulca@gmail.com",
        "url": ""
      }
    ],
    "links": {
      "repository": "https://github.com/Slayer95/tree-sitter-jass.git",
      "funding": ""
    }
  },
  "bindings": {
    "c": true,
    "go": false,
    "java": true,
    "node": true,
    "python": false,
    "rust": false,
    "swift": false,
    "zig": false
  }
}