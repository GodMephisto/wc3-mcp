---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "src/lng/w3r/mod.rs"
ref: HEAD
---

//! Binary reader for Warcraft III region files (`.w3r`).
//!
//! The `.w3r` format (`war3map.w3r`) stores map regions (rects / areas)
//! used by triggers, weather effects and ambient sounds.
//!
//! The format is described in `w3r.hexpat` (ImHex pattern) and at
//! <https://xgm.guru/p/wc3/w3-file-format>.
//!
//! All multi-byte integers are **little-endian**.

pub mod parse;
pub mod send;

pub use parse::*;

