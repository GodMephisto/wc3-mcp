---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/main.rs"
ref: HEAD
---

use std::env;
use std::fs;
use std::process::ExitCode;

use jass_rs::{lint, render_dot, render_html, render_tree, Level};

enum Mode {
    Lint,
    Ast,
    Dot,
    Html,
}

fn usage() {
    eprintln!("usage: jass-rs [--ast|--dot|--html] <file.j>");
    eprintln!("  (no flag)  lint the file and print diagnostics");
    eprintln!("  --ast      print the parsed AST as an indented tree");
    eprintln!("  --dot      print the parsed AST as a Graphviz DOT graph");
    eprintln!("  --html     print a self-contained AST viewer page (source + tree)");
    eprintln!();
    eprintln!("  e.g. jass-rs --html file.j > file.ast.html && open file.ast.html");
}

fn main() -> ExitCode {
    let mut mode = Mode::Lint;
    let mut path = None;
    for arg in env::args().skip(1) {
        match arg.as_str() {
            "--ast" => mode = Mode::Ast,
            "--dot" => mode = Mode::Dot,
            "--html" => mode = Mode::Html,
            "-h" | "--help" => {
                usage();
                return ExitCode::SUCCESS;
            }
            other => path = Some(other.to_string()),
        }
    }

    let path = match path {
        Some(path) => path,
        None => {
            usage();
            return ExitCode::FAILURE;
        }
    };

    let src = match fs::read_to_string(&path) {
        Ok(src) => src,
        Err(err) => {
            eprintln!("{path}: {err}");
            return ExitCode::FAILURE;
        }
    };

    let program = match jass_rs::parse(&src) {
        Ok(program) => program,
        Err(err) => {
            eprintln!("{path}:{}:{}: error: {}", err.line, err.col, err.message);
            return ExitCode::FAILURE;
        }
    };

    match mode {
        Mode::Ast => {
            print!("{}", render_tree(&program));
            ExitCode::SUCCESS
        }
        Mode::Dot => {
            print!("{}", render_dot(&program));
            ExitCode::SUCCESS
        }
        Mode::Html => {
            print!("{}", render_html(&program, &src, &path));
            ExitCode::SUCCESS
        }
        Mode::Lint => {
            let diagnostics = lint(&program);
            let mut has_error = false;
            for diag in &diagnostics {
                let level = match diag.level {
                    Level::Error => {
                        has_error = true;
                        "error"
                    }
                    Level::Warning => "warning",
                };
                println!("{path}:{}: {level}: {}", diag.line, diag.message);
            }

            if diagnostics.is_empty() {
                println!("{path}: no issues found");
            }

            if has_error {
                ExitCode::FAILURE
            } else {
                ExitCode::SUCCESS
            }
        }
    }
}
