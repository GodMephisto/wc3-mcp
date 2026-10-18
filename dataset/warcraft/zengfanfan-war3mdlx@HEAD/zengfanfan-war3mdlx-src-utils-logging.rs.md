---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/utils/logging.rs"
ref: HEAD
---

use crate::*;

// region: level

#[derive(Debug, Default, Copy, Clone, PartialEq, Eq, PartialOrd, Ord)]
pub enum LogLevel {
    Verbose3,
    Verbose2,
    Verbose,
    #[default]
    Info,
    Warn,
    Error,
}

#[macro_export]
macro_rules! check_log_level {
    (::$item:ident) => {{ *log_level() <= crate::logging::LogLevel::$item }};
}

// endregion
// region: log!

#[macro_export]
macro_rules! _log {
    (::$item:ident, $($arg:tt)*) => {{
        if check_log_level!(::$item) {
            println!($($arg)*);
        }
    }};
}
#[macro_export]
macro_rules! _elog {
    (::$item:ident, $($arg:tt)*) => {{
        if check_log_level!(::$item) {
            eprintln!($($arg)*);
        }
    }};
}

#[macro_export]
macro_rules! elog {
    ($($arg:tt)*) => {{ _elog!(::Error, $($arg)*) }};
}

#[macro_export]
macro_rules! wlog {
    ($($arg:tt)*) => {{ _log!(::Warn, $($arg)*) }};
}

#[macro_export]
macro_rules! log {
    ($($arg:tt)*) => {{ _log!(::Info, $($arg)*) }};
}

#[macro_export]
macro_rules! vlog {
    ($($arg:tt)*) => {{ _log!(::Verbose, $($arg)*) }};
}
#[macro_export]
macro_rules! vvlog {
    ($($arg:tt)*) => {{ _log!(::Verbose2, $($arg)*) }};
}
#[macro_export]
macro_rules! vvvlog {
    ($($arg:tt)*) => {{ _log!(::Verbose3, $($arg)*) }};
}

// endregion
// region: hex dump

#[allow(dead_code)]
pub fn hexdump(data: &Vec<u8>, indent: &str) -> String {
    pretty_hex(data).replace("\n", &F!("\n{indent}"))
}

// endregion
