---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/parser.test.rs"
ref: HEAD
---

use super::*;

#[test]
fn parses_empty_globals_block() {
    let prog = Parser::parse_str("globals\nendglobals").unwrap();
    assert_eq!(prog.items, vec![TopLevel::Globals(vec![])]);
}

#[test]
fn parses_global_declarations() {
    let prog = Parser::parse_str(
        "globals\n    integer x = 1\n    constant real PI = 3.14\n    unit array units\nendglobals",
    )
    .unwrap();
    match &prog.items[0] {
        TopLevel::Globals(decls) => {
            assert_eq!(decls.len(), 3);
            assert_eq!(decls[0].name, "x");
            assert_eq!(decls[0].type_name, "integer");
            assert!(!decls[0].is_constant);
            assert_eq!(decls[1].name, "PI");
            assert!(decls[1].is_constant);
            assert_eq!(decls[2].name, "units");
            assert!(decls[2].is_array);
        }
        other => panic!("expected globals, got {other:?}"),
    }
}

#[test]
fn parses_native_declaration() {
    let prog = Parser::parse_str("native DoNothing takes nothing returns nothing").unwrap();
    match &prog.items[0] {
        TopLevel::Native(sig) => {
            assert_eq!(sig.name, "DoNothing");
            assert!(sig.params.is_empty());
            assert_eq!(sig.returns, None);
        }
        other => panic!("expected native, got {other:?}"),
    }
}

#[test]
fn parses_simple_function() {
    let src = r#"
function Add takes integer a, integer b returns integer
local integer sum = a + b
return sum
endfunction
"#;
    let prog = Parser::parse_str(src).unwrap();
    match &prog.items[0] {
        TopLevel::Function(f) => {
            assert_eq!(f.sig.name, "Add");
            assert_eq!(f.sig.params.len(), 2);
            assert_eq!(f.sig.returns, Some("integer".to_string()));
            assert_eq!(f.body.len(), 2);
        }
        other => panic!("expected function, got {other:?}"),
    }
}

#[test]
fn parses_if_elseif_else() {
    let src = r#"
function Test takes nothing returns nothing
if 1 > 0 then
    call A()
elseif 1 == 0 then
    call B()
else
    call C()
endif
endfunction
"#;
    let prog = Parser::parse_str(src).unwrap();
    match &prog.items[0] {
        TopLevel::Function(f) => match &f.body[0] {
            Stmt::If {
                branches,
                else_branch,
                ..
            } => {
                assert_eq!(branches.len(), 2);
                assert!(else_branch.is_some());
            }
            other => panic!("expected if, got {other:?}"),
        },
        other => panic!("expected function, got {other:?}"),
    }
}

#[test]
fn parses_loop_and_exitwhen() {
    let src = r#"
function Test takes nothing returns nothing
local integer i = 0
loop
    exitwhen i > 10
    set i = i + 1
endloop
endfunction
"#;
    let prog = Parser::parse_str(src).unwrap();
    match &prog.items[0] {
        TopLevel::Function(f) => match &f.body[1] {
            Stmt::Loop { body, .. } => {
                assert_eq!(body.len(), 2);
                assert!(matches!(body[0], Stmt::ExitWhen { .. }));
            }
            other => panic!("expected loop, got {other:?}"),
        },
        other => panic!("expected function, got {other:?}"),
    }
}

#[test]
fn respects_operator_precedence() {
    let mut parser = Parser::new(Lexer::tokenize("1 + 2 * 3").unwrap());
    let e = parser.expr().unwrap();
    assert_eq!(
        e,
        Expr::dummy(ExprKind::Binary(
            Box::new(Expr::dummy(ExprKind::IntLiteral(1))),
            BinOp::Add,
            Box::new(Expr::dummy(ExprKind::Binary(
                Box::new(Expr::dummy(ExprKind::IntLiteral(2))),
                BinOp::Mul,
                Box::new(Expr::dummy(ExprKind::IntLiteral(3))),
            ))),
        ))
    );
    assert_eq!(e.span, Span::new(1, 1, 1, 10));
}

#[test]
fn parses_array_access_and_call() {
    let mut parser = Parser::new(Lexer::tokenize("arr[GetInt(1)]").unwrap());
    let e = parser.expr().unwrap();
    assert_eq!(
        e,
        Expr::dummy(ExprKind::ArrayAccess(
            "arr".to_string(),
            Box::new(Expr::dummy(ExprKind::Call(
                "GetInt".to_string(),
                vec![Expr::dummy(ExprKind::IntLiteral(1))]
            ))),
        ))
    );
    assert_eq!(e.span, Span::new(1, 1, 1, 15));
}

#[test]
fn reports_error_on_missing_endif() {
    let src = "function T takes nothing returns nothing\n if true then\n call A()\nendfunction";
    let err = Parser::parse_str(src).unwrap_err();
    assert!(err.message.contains("EndFunction"));
}

#[test]
fn reports_error_on_unknown_top_level_token() {
    let err = Parser::parse_str("42").unwrap_err();
    assert!(err.message.contains("unexpected top-level token"));
}
