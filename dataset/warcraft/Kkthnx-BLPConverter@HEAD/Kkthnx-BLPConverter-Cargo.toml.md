---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "Cargo.toml"
ref: HEAD
---

[workspace]
members = ["src-tauri", "blp-shell-ext"]
resolver = "2"

[profile.release]
# The BLPView shell extension runs inside Explorer (DisableProcessIsolation), so
# it catches panics at the COM boundary and returns an error. That needs unwind,
# abort would take Explorer down on a bad file.
panic = "unwind"
codegen-units = 1
lto = true
opt-level = "s"
strip = true
