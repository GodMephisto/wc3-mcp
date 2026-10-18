---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "src/util/uri_map.rs"
ref: HEAD
---

use dashmap::DashMap;
use once_cell::sync::Lazy;
use url::Url;

pub static LNG_URI_MAP: Lazy<DashMap<Url, String>> = Lazy::new(|| DashMap::new());
