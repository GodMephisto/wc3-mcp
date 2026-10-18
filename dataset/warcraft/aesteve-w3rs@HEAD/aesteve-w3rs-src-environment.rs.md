---
type: vendor-doc
source_repo: "https://github.com/aesteve/w3rs"
source_path: "src/environment.rs"
ref: HEAD
---

#[derive(Debug, PartialEq, Clone)]
pub enum Environment {
    Tree,
    GoldMine,
}

impl Environment {
    pub(crate) fn from_str(str: &str) -> Option<Environment> {
        if "LTlt" == str {
            Some(Environment::Tree)
        } else {
            None
        }
    }
}
