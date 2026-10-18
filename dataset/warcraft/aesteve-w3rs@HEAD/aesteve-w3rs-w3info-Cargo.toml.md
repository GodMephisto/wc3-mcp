---
type: vendor-doc
source_repo: "https://github.com/aesteve/w3rs"
source_path: "w3info/Cargo.toml"
ref: HEAD
---

[package]
name = "w3info"
version = "0.1.0"
edition = "2021"

# See more keys and their definitions at https://doc.rust-lang.org/cargo/reference/manifest.html

[dependencies]
itertools = "0.10"
serde = "1.0"
serde_json = "1.0"
serde_derive = "1.0"
chrono = { value = "0.4", features = ["serde"] }
w3rs = { path = ".." }
