---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/utils/error.rs"
ref: HEAD
---

use crate::*;
use std::num::{ParseFloatError, ParseIntError};

#[derive(Debug)]
pub enum MyError {
    String(String),
    Io(ioError),
    ParseInt(ParseIntError),
    ParseFloat(ParseFloatError),
}

// region: trait: Display

impl Display for MyError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            Self::String(s) => write!(f, "{}", s),
            Self::Io(e) => write!(f, "{}", e),
            Self::ParseInt(e) => write!(f, "{}", e),
            Self::ParseFloat(e) => write!(f, "{}", e),
        }
    }
}

// endregion
// region: trait: Convert From

impl From<String> for MyError {
    fn from(e: String) -> Self {
        MyError::String(e)
    }
}

impl From<std::io::Error> for MyError {
    fn from(e: std::io::Error) -> Self {
        MyError::Io(e)
    }
}

impl From<ParseIntError> for MyError {
    fn from(e: ParseIntError) -> Self {
        MyError::ParseInt(e)
    }
}

impl From<ParseFloatError> for MyError {
    fn from(e: ParseFloatError) -> Self {
        MyError::ParseFloat(e)
    }
}

// endregion

#[macro_export]
macro_rules! ERR {
    ($($arg:tt)*) => {{
        #[allow(unused_mut)]
        let mut s = F!($($arg)*);
        // if cfg!(debug_assertions) && s.lines().nth(1).is_none() { s = F!("{}\n{}\n", s, debug_trace(0,6)); }
        core::result::Result::Err(MyError::String(s))
    }};
}
