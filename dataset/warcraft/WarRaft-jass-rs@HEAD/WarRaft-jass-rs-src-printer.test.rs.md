---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/printer.test.rs"
ref: HEAD
---

use super::*;
use crate::parser::Parser;

#[test]
fn renders_empty_function_tree() {
    let program =
        Parser::parse_str("function Foo takes nothing returns nothing\nendfunction").unwrap();
    let tree = render_tree(&program);
    assert_eq!(
        tree,
        "Program\n└─ Function Foo(nothing) -> nothing (line 1)\n"
    );
}

#[test]
fn renders_expression_subtree() {
    let program = Parser::parse_str(
        "function Add takes integer a, integer b returns integer\n    return a + b\nendfunction",
    )
    .unwrap();
    let tree = render_tree(&program);
    assert_eq!(
        tree,
        "Program\n\
         └─ Function Add(integer a, integer b) -> integer (line 1)\n\
         \u{20}  └─ Return (line 2)\n\
         \u{20}     └─ Binary Add\n\
         \u{20}        ├─ Var a\n\
         \u{20}        └─ Var b\n"
    );
}

#[test]
fn renders_globals_and_natives() {
    let program = Parser::parse_str(
        "globals\n    constant integer MAX = 10\nendglobals\nnative Foo takes nothing returns nothing",
    )
    .unwrap();
    let tree = render_tree(&program);
    assert_eq!(
        tree,
        "Program\n\
         ├─ Globals\n\
         │  └─ constant integer MAX (line 2)\n\
         │     └─ Int 10\n\
         └─ Native Foo(nothing) -> nothing (line 4)\n"
    );
}

#[test]
fn dot_output_has_graph_structure() {
    let program =
        Parser::parse_str("function Foo takes nothing returns nothing\nendfunction").unwrap();
    let dot = render_dot(&program);
    assert!(dot.starts_with("digraph AST {"));
    assert!(dot.contains("n0 [label=\"Program\"];"));
    assert!(dot.contains("n0 -> n1;"));
    assert!(dot.trim_end().ends_with('}'));
}

#[test]
fn dot_output_escapes_quotes_and_backslashes() {
    let program = Parser::parse_str(
        "function Foo takes nothing returns nothing\n    call P(\"a\\\"b\")\nendfunction",
    )
    .unwrap();
    let dot = render_dot(&program);
    assert!(dot.contains(r#"label="String \"a\\\"b\"""#));
}

#[test]
fn tags_statement_nodes_with_whole_line_spans() {
    let program = Parser::parse_str(
        "function Add takes integer a, integer b returns integer\n    return a + b\nendfunction",
    )
    .unwrap();
    let root = build_tree(&program);
    assert_eq!(root.kind, "root");
    assert_eq!(root.span, None);

    let function = &root.children[0];
    assert_eq!(function.kind, "sig");
    assert_eq!(function.span, Some(Span::whole_line(1)));

    let ret = &function.children[0];
    assert_eq!(ret.kind, "stmt");
    assert_eq!(ret.span, Some(Span::whole_line(2)));
}

#[test]
fn tags_expression_nodes_with_precise_column_spans() {
    let program = Parser::parse_str(
        "function Add takes integer a, integer b returns integer\n    return a + b\nendfunction",
    )
    .unwrap();
    let root = build_tree(&program);
    let ret = &root.children[0].children[0];
    let binary = &ret.children[0];
    assert_eq!(binary.kind, "expr");
    // "    return a + b" -> `a + b` starts at column 12.
    assert_eq!(binary.span, Some(Span::new(2, 12, 2, 17)));

    let var_a = &binary.children[0];
    assert_eq!(var_a.span, Some(Span::new(2, 12, 2, 13)));
    let var_b = &binary.children[1];
    assert_eq!(var_b.span, Some(Span::new(2, 16, 2, 17)));
}
