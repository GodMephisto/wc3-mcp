---
type: vendor-doc
source_repo: "https://github.com/WarRaft/mpq-rs"
source_path: "src/bin/mpq-cli.rs"
ref: HEAD
---

use mpq_rs::run::run;

fn main() {
    if let Err(e) = run() {
        eprintln!("{e}");
        std::process::exit(1);
    }
}
