---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/lint.test.rs"
ref: HEAD
---

use super::*;
use crate::parser::Parser;

fn lint_src(src: &str) -> Vec<Diagnostic> {
    let program = Parser::parse_str(src).expect("source should parse");
    lint(&program)
}

#[test]
fn flags_duplicate_global() {
    let diags = lint_src("globals\n    integer x = 1\n    integer x = 2\nendglobals");
    assert!(diags
        .iter()
        .any(|d| d.level == Level::Error && d.message.contains("duplicate global variable 'x'")));
}

#[test]
fn flags_duplicate_function() {
    let src = "function Foo takes nothing returns nothing\nendfunction\nfunction Foo takes nothing returns nothing\nendfunction";
    let diags = lint_src(src);
    assert!(diags
        .iter()
        .any(|d| d.level == Level::Error && d.message.contains("duplicate function 'Foo'")));
}

#[test]
fn flags_duplicate_local() {
    let src = r#"
function Foo takes nothing returns nothing
local integer x = 1
local integer x = 2
endfunction
"#;
    let diags = lint_src(src);
    assert!(diags
        .iter()
        .any(|d| d.level == Level::Error && d.message.contains("duplicate local variable 'x'")));
}

#[test]
fn flags_unused_local() {
    let src = r#"
function Foo takes nothing returns nothing
local integer x = 1
endfunction
"#;
    let diags = lint_src(src);
    assert!(diags
        .iter()
        .any(|d| d.level == Level::Warning && d.message.contains("unused local variable 'x'")));
}

#[test]
fn does_not_flag_used_local() {
    let src = r#"
function Foo takes nothing returns integer
local integer x = 1
return x
endfunction
"#;
    let diags = lint_src(src);
    assert!(!diags.iter().any(|d| d.message.contains("unused local")));
}

#[test]
fn flags_loop_without_exitwhen() {
    let src = r#"
function Foo takes nothing returns nothing
loop
    call DoSomething()
endloop
endfunction
"#;
    let diags = lint_src(src);
    assert!(diags
        .iter()
        .any(|d| d.level == Level::Warning && d.message.contains("no 'exitwhen'")));
}

#[test]
fn does_not_flag_loop_with_exitwhen() {
    let src = r#"
function Foo takes nothing returns nothing
local integer i = 0
loop
    exitwhen i > 10
    set i = i + 1
endloop
endfunction
"#;
    let diags = lint_src(src);
    assert!(!diags.iter().any(|d| d.message.contains("no 'exitwhen'")));
}

#[test]
fn flags_empty_then_branch() {
    let src = r#"
function Foo takes nothing returns nothing
if true then
endif
endfunction
"#;
    let diags = lint_src(src);
    assert!(diags
        .iter()
        .any(|d| d.message.contains("empty 'then' branch")));
}

#[test]
fn flags_missing_return() {
    let src = "function Foo takes nothing returns integer\nendfunction";
    let diags = lint_src(src);
    assert!(diags
        .iter()
        .any(|d| d.level == Level::Warning && d.message.contains("no 'return' statement")));
}

#[test]
fn clean_function_has_no_diagnostics() {
    let src = r#"
globals
integer counter = 0
endglobals

function Increment takes nothing returns integer
local integer i = 0
loop
    exitwhen i >= counter
    set i = i + 1
endloop
return i
endfunction
"#;
    let diags = lint_src(src);
    assert!(diags.is_empty(), "expected no diagnostics, got {diags:?}");
}
