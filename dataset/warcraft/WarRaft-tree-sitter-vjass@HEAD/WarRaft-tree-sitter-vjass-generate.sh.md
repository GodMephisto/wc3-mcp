---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "generate.sh"
ref: HEAD
---

tree-sitter generate

#tree-sitter playground

# https://tree-sitter.github.io/tree-sitter/creating-parsers/5-writing-tests.html
#tree-sitter test
#tree-sitter parse vjass/test.vjass

tree-sitter build --wasm
tree-sitter playground
