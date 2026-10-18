---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/query.test.rs"
ref: HEAD
---

use super::*;
use crate::parser::Parser;

const SRC: &str =
    "function Add takes integer a, integer b returns integer\n    return a + b\nendfunction";

#[test]
fn finds_the_deepest_expr_at_an_exact_position() {
    let program = Parser::parse_str(SRC).unwrap();
    // Column 12 is the 'a' in `return a + b`.
    let hit = find_node_at(&program, 2, 12).unwrap();
    match hit {
        AstRef::Expr(e) => assert_eq!(e.kind, ExprKind::Var("a".to_string())),
        other => panic!("expected an Expr, got {other:?}"),
    }
}

#[test]
fn finds_the_enclosing_binary_for_a_range_spanning_both_operands() {
    let program = Parser::parse_str(SRC).unwrap();
    // Columns 12..17 span `a + b` minus the trailing "b" details — pick
    // a range that only the whole Binary node contains: from "a" (12)
    // through "b" (17).
    let hit = find_node_in(&program, 2, 12, 17).unwrap();
    match hit {
        AstRef::Expr(e) => assert!(matches!(e.kind, ExprKind::Binary(..))),
        other => panic!("expected an Expr, got {other:?}"),
    }
}

#[test]
fn falls_back_to_the_statement_for_columns_outside_any_expression() {
    let program = Parser::parse_str(SRC).unwrap();
    // Column 1 is inside the leading indentation, before `return`.
    let hit = find_node_at(&program, 2, 1).unwrap();
    assert!(matches!(hit, AstRef::Stmt(Stmt::Return { .. })));
}

#[test]
fn returns_none_outside_the_program() {
    let program = Parser::parse_str(SRC).unwrap();
    assert!(find_node_at(&program, 999, 1).is_none());
}

#[test]
fn finds_global_declarations_and_their_initializers() {
    let program = Parser::parse_str("globals\n    constant integer MAX = 10\nendglobals").unwrap();
    // Column 28 is inside `10`.
    let hit = find_node_at(&program, 2, 28).unwrap();
    match hit {
        AstRef::Expr(e) => assert_eq!(e.kind, ExprKind::IntLiteral(10)),
        other => panic!("expected an Expr, got {other:?}"),
    }
    // Column 5 is inside `constant`, not covered by any child.
    let decl_hit = find_node_at(&program, 2, 5).unwrap();
    assert!(matches!(decl_hit, AstRef::GlobalDecl(_)));
}
