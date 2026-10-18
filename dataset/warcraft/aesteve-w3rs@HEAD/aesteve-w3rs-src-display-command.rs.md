---
type: vendor-doc
source_repo: "https://github.com/aesteve/w3rs"
source_path: "src/display/command.rs"
ref: HEAD
---

use crate::blocks::command::Position;
use std::fmt::{Display, Formatter};

impl Display for Position {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "{{x={},y={}}}", self.x, self.y)
    }
}
