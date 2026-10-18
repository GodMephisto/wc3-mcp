---
type: vendor-doc
source_repo: "https://github.com/TrayHard/casc-modern"
source_path: "crates/casc-cli/Cargo.toml"
ref: HEAD
---

[package]
name = "casc-cli"
version.workspace = true
edition.workspace = true
license.workspace = true
authors.workspace = true

[[bin]]
name = "casc"
path = "src/main.rs"

[dependencies]
casc-core = { path = "../casc-core" }
clap = { workspace = true }
anyhow = { workspace = true }
indicatif = { workspace = true }
rayon = { workspace = true }
tracing = { workspace = true }
tracing-subscriber = { workspace = true }
serde_json = { workspace = true }
image = { workspace = true }
