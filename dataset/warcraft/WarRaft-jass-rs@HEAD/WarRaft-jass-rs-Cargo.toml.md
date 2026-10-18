---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "Cargo.toml"
ref: HEAD
---

[package]
name = "jass-rs"
version = "0.1.0"
edition = "2021"
description = "A custom parser and linter for JASS and vJASS"
license = "MIT"

[[bin]]
name = "jass-rs"
path = "src/main.rs"

[lib]
name = "jass_rs"
path = "src/lib.rs"

[dependencies]
parser-rs = { path = "../parser-rs" }
