---
type: vendor-doc
source_repo: "https://github.com/zloy-tulen/model-mdx"
source_path: "model-mdx/Cargo.toml"
ref: HEAD
---

[package]
name = "model-mdx"
version = "0.1.0"
edition = "2021"
license = "MIT"
description = "Library to encode/decode Blizzard MDX model format"
repository = "https://github.com/zloy-tulen/model-mdx.git"
documentation = "https://docs.rs/model-mdx"
authors = ["zloy_tulen <zloy_tulen@proton.me>"]
readme = "../README.md"
keywords = ["mdx", "model", "asset", "war3", "warcraft"]
categories = ["multimedia::models", "multimedia::encoding"]

# See more keys and their definitions at https://doc.rust-lang.org/cargo/reference/manifest.html

[dependencies]
bitflags = "1.3.2"
image-blp = { version = "1" }
log = "0.4.17"
nom = "7.1.1"
thiserror = "1.0.33"

[dev-dependencies]
env_logger = "0.9.0"
include_dir = { version = "0.7.2", features = ["glob"] }
test-log = "0.2.8"
