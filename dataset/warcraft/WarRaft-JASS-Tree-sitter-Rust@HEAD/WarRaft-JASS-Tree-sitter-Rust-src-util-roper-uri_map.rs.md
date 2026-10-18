---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "src/util/roper/uri_map.rs"
ref: HEAD
---

use dashmap::DashMap;
use lapce_xi_rope::Rope;
use once_cell::sync::Lazy;
use url::Url;

pub static ROPE_MAP: Lazy<DashMap<Url, Rope>> = Lazy::new(|| DashMap::new());
