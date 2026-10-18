---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/lexer.test.rs"
ref: HEAD
---

use super::*;

fn kinds(src: &str) -> Vec<TokenKind> {
    Lexer::tokenize(src)
        .unwrap()
        .into_iter()
        .map(|t| t.kind)
        .collect()
}

#[test]
fn tokenizes_keywords() {
    assert_eq!(
        kinds("function endfunction takes returns nothing"),
        vec![
            TokenKind::Function,
            TokenKind::EndFunction,
            TokenKind::Takes,
            TokenKind::Returns,
            TokenKind::Nothing,
            TokenKind::Eof,
        ]
    );
}

#[test]
fn tokenizes_identifiers_and_numbers() {
    assert_eq!(
        kinds("foo 123 4.25 0x1F"),
        vec![
            TokenKind::Identifier("foo".to_string()),
            TokenKind::IntLiteral(123),
            TokenKind::RealLiteral(4.25),
            TokenKind::IntLiteral(31),
            TokenKind::Eof,
        ]
    );
}

#[test]
fn tokenizes_string_literal_with_escapes() {
    assert_eq!(
        kinds(r#""hello\nworld""#),
        vec![
            TokenKind::StringLiteral("hello\nworld".to_string()),
            TokenKind::Eof,
        ]
    );
}

#[test]
fn tokenizes_rawcode_literal() {
    assert_eq!(
        kinds("'hfoo'"),
        vec![TokenKind::IntLiteral(0x68666f6f), TokenKind::Eof,]
    );
}

#[test]
fn tokenizes_operators() {
    assert_eq!(
        kinds("== != >= <= = > <"),
        vec![
            TokenKind::EqEq,
            TokenKind::NotEq,
            TokenKind::GtEq,
            TokenKind::LtEq,
            TokenKind::Assign,
            TokenKind::Gt,
            TokenKind::Lt,
            TokenKind::Eof,
        ]
    );
}

#[test]
fn skips_line_comments() {
    assert_eq!(
        kinds("set x = 1 // comment\nset y = 2"),
        vec![
            TokenKind::Set,
            TokenKind::Identifier("x".to_string()),
            TokenKind::Assign,
            TokenKind::IntLiteral(1),
            TokenKind::Set,
            TokenKind::Identifier("y".to_string()),
            TokenKind::Assign,
            TokenKind::IntLiteral(2),
            TokenKind::Eof,
        ]
    );
}

#[test]
fn reports_unterminated_string() {
    let err = Lexer::tokenize("\"abc").unwrap_err();
    assert!(err.message.contains("unterminated string"));
}

#[test]
fn reports_invalid_rawcode_length() {
    let err = Lexer::tokenize("'ab'").unwrap_err();
    assert!(err.message.contains("4 characters"));
}
