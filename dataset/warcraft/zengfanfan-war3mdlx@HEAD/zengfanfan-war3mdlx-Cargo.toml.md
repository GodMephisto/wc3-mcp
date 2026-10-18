---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "Cargo.toml"
ref: HEAD
---

[package]
name = "War3Mdlx"
version = "1.1.0"
edition = "2024"
keywords = ["warcraft", "model", "mdx", "mdl"]
categories = ["command-line-utilities"]
authors = ["zengfanfan"]
homepage = "https://github.com/zengfanfan/war3mdlx"
repository = "https://github.com/zengfanfan/war3mdlx"
license = "GPL-3.0"
description = "A command line tool to convert Warcraft 3 model file (mdl <-> mdx)."
exclude = [
  "test/**",
]

[[bin]]
name = "war3mdlx"
path = "src/main.rs"

[dependencies]
clap = { version = "4", features = ["derive"], registry = "tuna" }
byteorder = { version = "1", registry = "tuna" }
walkdir = { version = "2", registry = "tuna" }
glam = { version = "0.30", registry = "tuna" }
derive-debug = { version = "0.1", registry = "tuna" }
pretty-hex = { version = "0.4", registry = "tuna" }
paste = { version = "1.0", registry = "tuna" }
bitflags = { version = "2.10", registry = "tuna" }
regex = { version = "1.12", registry = "tuna" }
path-clean = { version = "1.0", registry = "tuna" }
pest = { version = "2.8", registry = "tuna" }
pest_derive = { version = "2.8", registry = "tuna" }
chrono = { version = "0.4", registry = "tuna" }
smart-default = { version = "0.7", registry = "tuna" }
lazy_static = { version = "1.5", registry = "tuna" }
backtrace = { version = "0.3", registry = "tuna" }
