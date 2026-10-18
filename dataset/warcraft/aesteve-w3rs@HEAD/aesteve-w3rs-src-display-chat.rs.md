---
type: vendor-doc
source_repo: "https://github.com/aesteve/w3rs"
source_path: "src/display/chat.rs"
ref: HEAD
---

use crate::blocks::chat::Addressee;
use std::fmt::{Display, Formatter};

impl Display for Addressee {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "[{:?}]", self)
    }
}
