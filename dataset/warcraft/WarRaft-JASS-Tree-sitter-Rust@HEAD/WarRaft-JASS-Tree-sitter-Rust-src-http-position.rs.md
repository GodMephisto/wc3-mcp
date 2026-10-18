---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "src/http/position.rs"
ref: HEAD
---

use serde::{Deserialize, Serialize};

/// https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#position
#[derive(Debug, Default, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Position {
    pub line: usize,
    pub character: usize,
}
