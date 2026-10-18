---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "bindings/node/index.js"
ref: HEAD
---

const { readFileSync } = require('fs');
const { resolve } = require('path');

const root = resolve(__dirname, '..', '..');

const binding = require("node-gyp-build")(root);

try {
  const nodeTypes = require('../../src/node-types.json');
  binding.nodeTypeInfo = nodeTypes;
} catch { }

const queries = [
  ["HIGHLIGHTS_QUERY", `${root}/queries/highlights.scm`],
  ["INJECTIONS_QUERY", `${root}/queries/injections.scm`],
  ["LOCALS_QUERY", `${root}/queries/locals.scm`],
  ["TAGS_QUERY", `${root}/queries/tags.scm`],
];

for (const [prop, path] of queries) {
  Object.defineProperty(binding, prop, {
    configurable: true,
    enumerable: true,
    get() {
      delete binding[prop];
      try {
        binding[prop] = readFileSync(path, "utf8");
      } catch { }
      return binding[prop];
    }
  });
}

module.exports = binding;
