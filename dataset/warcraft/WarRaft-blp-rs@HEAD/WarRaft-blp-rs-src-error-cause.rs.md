---
type: vendor-doc
source_repo: "https://github.com/WarRaft/blp-rs"
source_path: "src/error/cause.rs"
ref: HEAD
---

use crate::error::error::BlpError;
use std::error::Error;
use std::sync::Arc;

#[derive(Debug, Clone)]
pub(crate) enum Cause {
    Blp(BlpError),
    Std(Arc<dyn Error + Send + Sync>),
}
