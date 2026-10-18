---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "src/utils/debug.rs"
ref: HEAD
---

use crate::*;

// region: Backtrace

#[allow(unused)]
pub fn debug_trace(skip: i32, depth: i32) -> String {
    let mut lines: Vec<String> = vec![];
    let mut count = -(skip + 4);
    const INDENT: &str = "\t";
    let indent2 = INDENT.repeat(2);
    let cwd = env::current_dir().unwrap_or_default();
    backtrace::trace(|frame| {
        backtrace::resolve_frame(frame, |symbol| {
            if count >= 0 {
                let s = symbol.name().and_then(|name| Some(name.s())).unwrap_or("<unknown>".s());
                lines.push(F!("{INDENT}{count}: {s}"));
                if let Some(path) = symbol.filename()
                    && let Some(path) = path.relative_to(&cwd).to_str()
                {
                    let line = symbol.lineno().unwrap_or_default();
                    lines.push(F!("{indent2}at {path}:{line}"));
                }
            }
            count += 1;
        });
        return count < depth; // true: continue; false: end;
    });
    return lines.join("\n");
}

// endregion
