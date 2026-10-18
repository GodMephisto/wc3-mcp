---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "package.json"
ref: HEAD
---

{
  "name": "tree-sitter-vjass",
  "version": "0.1.0",
  "description": "Vjass grammar for tree-sitter",
  "repository": "https://github.com/tree-sitter/tree-sitter-vjass",
  "license": "MIT",
  "author": {
    "name": "nazarpunk",
    "email": "nazarpunk@gmail.com"
  },
  "main": "bindings/node",
  "types": "bindings/node",
  "keywords": [
    "incremental",
    "parsing",
    "tree-sitter",
    "vjass"
  ],
  "files": [
    "grammar.js",
    "tree-sitter.json",
    "binding.gyp",
    "prebuilds/**",
    "bindings/node/*",
    "queries/*",
    "src/**",
    "*.wasm"
  ],
  "dependencies": {
    "node-addon-api": "^8.2.1",
    "node-gyp-build": "^4.8.2"
  },
  "devDependencies": {
    "prebuildify": "^6.0.1",
    "tree-sitter-cli": "^0.25.3"
  },
  "peerDependencies": {
    "tree-sitter": "^0.21.1"
  },
  "peerDependenciesMeta": {
    "tree-sitter": {
      "optional": true
    }
  },
  "scripts": {
    "install": "node-gyp-build",
    "prestart": "tree-sitter build --wasm",
    "start": "tree-sitter playground",
    "test": "node --test bindings/node/*_test.js"
  }
}
