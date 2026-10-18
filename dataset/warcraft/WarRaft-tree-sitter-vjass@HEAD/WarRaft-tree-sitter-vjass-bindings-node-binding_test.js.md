---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "bindings/node/binding_test.js"
ref: HEAD
---

const assert = require("node:assert");
const { test } = require("node:test");

const Parser = require("tree-sitter");

test("can load grammar", () => {
  const parser = new Parser();
  assert.doesNotThrow(() => parser.setLanguage(require(".")));
});
