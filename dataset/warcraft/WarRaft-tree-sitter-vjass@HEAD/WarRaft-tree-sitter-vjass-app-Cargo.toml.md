---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "app/Cargo.toml"
ref: HEAD
---

[package]
name = "app"
version = "0.2.0"
authors = ["nazarpunk <nazarpunk@gmail.com>"]
edition = "2021"

[dependencies]
tree-sitter = "0.25.3"
tree-sitter-vjass = { path = "./.." }

[build-dependencies]
