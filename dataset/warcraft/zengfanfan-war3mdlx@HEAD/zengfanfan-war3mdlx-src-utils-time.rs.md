---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/utils/time.rs"
ref: HEAD
---

use crate::*;

// region: timestamp

#[allow(unused)]
pub fn timestamp_ms() -> i128 {
    let now = std::time::SystemTime::now();
    let duration = now.duration_since(std::time::UNIX_EPOCH).unwrap();
    return duration.as_millis() as i128;
}

#[allow(unused)]
pub fn timestamp_logstr() -> String {
    let ms = timestamp_ms();
    let s = ms / 1000;
    let m = s / 60 % 60;
    return F!("{:02}:{:02}.{:03}", m, s % 60, ms % 1000);
}

// endregion
