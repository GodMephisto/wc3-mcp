---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "package.json"
ref: HEAD
---

{
  "name": "tree-sitter-jass",
  "version": "12.4.0",
  "description": "Tree-sitter grammar and parser for JASS2 and JASS3",
  "main": "bindings/node",
  "type": "commonjs",
  "dependencies": {
    "node-addon-api": "^8.8.0"
  },
  "devDependencies": {
    "tree-sitter-cli": "^0.26.9"
  },
  "scripts": {
    "test": "echo \"Error: no test specified\" && exit 1",
    "install": "node-gyp rebuild"
  },
  "files": [
    "grammar.js",
    "grammar-defs.js",
    "src",
    "bindings/node",
    "index.js",
    "package.json",
	"tree-sitter.json",
	"pom.xml",
    "README.md",
	"CMakeLists.txt",
	"Makefile",
	"binding.gyp",
    "LICENSE"
  ],
  "repository": {
    "type": "git",
    "url": "git+https://github.com/Slayer95/tree-sitter-jass.git"
  },
  "keywords": [
    "tree-sitter",
    "jass",
    "parser",
    "syntax",
    "wc3"
  ],
  "author": "Leonardo Julca",
  "license": "MIT",
  "gypfile": true,
  "bugs": {
    "url": "https://github.com/Slayer95/tree-sitter-jass/issues"
  },
  "homepage": "https://github.com/Slayer95/tree-sitter-jass#readme"
}
