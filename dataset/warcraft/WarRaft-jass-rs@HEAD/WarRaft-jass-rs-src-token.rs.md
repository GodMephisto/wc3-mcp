---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/token.rs"
ref: HEAD
---

#[derive(Debug, Clone, PartialEq)]
pub enum TokenKind {
    // Keywords
    Function,
    EndFunction,
    Takes,
    Returns,
    Nothing,
    Native,
    Globals,
    EndGlobals,
    Constant,
    Local,
    Set,
    Call,
    If,
    Then,
    ElseIf,
    Else,
    EndIf,
    Loop,
    EndLoop,
    ExitWhen,
    Return,
    Type,
    Extends,
    Array,
    And,
    Or,
    Not,
    True,
    False,
    Null,

    // Literals
    Identifier(String),
    IntLiteral(i64),
    RealLiteral(f64),
    StringLiteral(String),

    // Punctuation / operators
    Plus,
    Minus,
    Star,
    Slash,
    EqEq,
    NotEq,
    Gt,
    Lt,
    GtEq,
    LtEq,
    Assign,
    LParen,
    RParen,
    LBracket,
    RBracket,
    Comma,

    Eof,
}

#[derive(Debug, Clone, PartialEq)]
pub struct Token {
    pub kind: TokenKind,
    pub line: usize,
    /// 1-indexed column of the token's first character.
    pub col: usize,
    /// 1-indexed column just past the token's last character (exclusive).
    /// Tokens never span multiple lines, so this is always on `line`.
    pub end_col: usize,
}

impl Token {
    pub fn new(kind: TokenKind, line: usize, col: usize, end_col: usize) -> Self {
        Token {
            kind,
            line,
            col,
            end_col,
        }
    }
}

pub fn keyword(word: &str) -> Option<TokenKind> {
    Some(match word {
        "function" => TokenKind::Function,
        "endfunction" => TokenKind::EndFunction,
        "takes" => TokenKind::Takes,
        "returns" => TokenKind::Returns,
        "nothing" => TokenKind::Nothing,
        "native" => TokenKind::Native,
        "globals" => TokenKind::Globals,
        "endglobals" => TokenKind::EndGlobals,
        "constant" => TokenKind::Constant,
        "local" => TokenKind::Local,
        "set" => TokenKind::Set,
        "call" => TokenKind::Call,
        "if" => TokenKind::If,
        "then" => TokenKind::Then,
        "elseif" => TokenKind::ElseIf,
        "else" => TokenKind::Else,
        "endif" => TokenKind::EndIf,
        "loop" => TokenKind::Loop,
        "endloop" => TokenKind::EndLoop,
        "exitwhen" => TokenKind::ExitWhen,
        "return" => TokenKind::Return,
        "type" => TokenKind::Type,
        "extends" => TokenKind::Extends,
        "array" => TokenKind::Array,
        "and" => TokenKind::And,
        "or" => TokenKind::Or,
        "not" => TokenKind::Not,
        "true" => TokenKind::True,
        "false" => TokenKind::False,
        "null" => TokenKind::Null,
        _ => return None,
    })
}
