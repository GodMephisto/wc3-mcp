---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "Cargo.toml"
ref: HEAD
---

[package]
name = "tree-sitter-vjass"
description = "Vjass grammar for tree-sitter"
version = "0.1.0"
authors = ["nazarpunk <nazarpunk@gmail.com>"]
license = "MIT"
readme = "README.md"
keywords = ["incremental", "parsing", "tree-sitter", "vjass"]
categories = ["parser-implementations", "parsing", "text-editors"]
repository = "https://github.com/tree-sitter/tree-sitter-vjass"
edition = "2024"
autoexamples = false

build = "bindings/rust/build.rs"
include = [
    "bindings/rust/*",
    "grammar.js",
    "queries/*",
    "src/*",
    "tree-sitter.json",
    "LICENSE",
]

[lib]
path = "bindings/rust/lib.rs"

[dependencies]
tree-sitter-language = "0.1.5"

[build-dependencies]
cc = "1.2.45"

[dev-dependencies]
tree-sitter = "0.25.10"
