---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "bindings/node/binding_test.js"
ref: HEAD
---

import assert from "node:assert";
import { test } from "node:test";
import Parser from "tree-sitter";

test("can load grammar", () => {
  const parser = new Parser();
  assert.doesNotReject(async () => {
    const { default: language } = await import("./index.js");
    parser.setLanguage(language);
  });
});
