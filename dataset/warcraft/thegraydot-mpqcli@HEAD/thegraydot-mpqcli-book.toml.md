---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: "book.toml"
ref: HEAD
---

[book]
title = "mpqcli"
description = "A command-line tool to create, add, remove, list, extract, read, and verify MPQ archives."
src = "docs"

[output.html]
git-repository-url = "https://github.com/thegraydot/mpqcli"
git-repository-icon = "fab-github"
additional-js = ["docs/js/copy-command.js", "mermaid.min.js", "mermaid-init.js"]

[preprocessor.mermaid]
command = "mdbook-mermaid"
