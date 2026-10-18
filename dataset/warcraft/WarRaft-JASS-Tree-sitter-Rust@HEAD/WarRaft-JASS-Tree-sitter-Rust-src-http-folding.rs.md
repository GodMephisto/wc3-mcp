---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "src/http/folding.rs"
ref: HEAD
---

#[derive(Debug, Default, Clone)]
pub struct FoldingRange {
    pub start_line: usize,
    pub end_line: usize,
    pub kind: Option<FoldingRangeKind>,
}

#[derive(Debug, Clone, PartialEq, Eq)]
#[allow(dead_code)]
pub enum FoldingRangeKind {
    Comment,
    Imports,
    Region,
}

