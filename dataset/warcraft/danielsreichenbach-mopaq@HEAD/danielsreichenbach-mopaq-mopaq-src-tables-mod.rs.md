---
type: vendor-doc
source_repo: "https://github.com/danielsreichenbach/mopaq"
source_path: "mopaq/src/tables/mod.rs"
ref: HEAD
---

//! MPQ table structures (hash, block, HET, BET)

mod bet;
mod block;
mod common;
mod hash;
mod het;

// Re-export all public types
pub use bet::{BetFileInfo, BetHeader, BetTable};
pub use block::{BlockEntry, BlockTable, HiBlockTable};
pub use hash::{HashEntry, HashTable};
pub use het::{HetHeader, HetTable};

// Re-export common utilities if needed
