---
type: vendor-doc
source_repo: "https://github.com/Trisard/wc3-battleships-companion"
source_path: "src-tauri/build.rs"
ref: HEAD
---

fn main() {
    // Force rebuild trigger to update icons (v2)
    tauri_build::build()
}

