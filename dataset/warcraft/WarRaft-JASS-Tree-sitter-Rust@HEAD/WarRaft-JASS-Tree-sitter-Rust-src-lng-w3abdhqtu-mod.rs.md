---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: "src/lng/w3abdhqtu/mod.rs"
ref: HEAD
---

//! Binary reader for Warcraft III object-data files
//! (`.w3a`, `.w3b`, `.w3d`, `.w3h`, `.w3q`, `.w3t`, `.w3u`).
//!
//! The format is described in `w3abdhqtu.hexpat` (ImHex pattern).
//!
//! All multi-byte integers are **little-endian**.

pub mod parse;
pub mod send;


