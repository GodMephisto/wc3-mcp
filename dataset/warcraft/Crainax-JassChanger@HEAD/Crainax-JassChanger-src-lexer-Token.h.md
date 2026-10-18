---
type: vendor-doc
source_repo: "https://github.com/Crainax/JassChanger"
source_path: "src/lexer/Token.h"
ref: HEAD
---

#pragma once

#include "core/SourceManager.h"
#include "preprocess/TextMacro.h"

#include <string>

namespace vjassc {

enum class TokenKind {
    EndOfFile,
    Identifier,
    IntegerLiteral,
    RealLiteral,
    StringLiteral,
    RawCodeLiteral,
    Keyword,
    LParen,
    RParen,
    LBrace,
    RBrace,
    LBracket,
    RBracket,
    Comma,
    Dot,
    Colon,
    Semicolon,
    Plus,
    Minus,
    Star,
    Slash,
    Percent,
    Assign,
    PlusAssign,
    MinusAssign,
    StarAssign,
    SlashAssign,
    EqualEqual,
    NotEqual,
    Less,
    LessEqual,
    Greater,
    GreaterEqual,
    Arrow,
    And,
    Or,
    Not,
    LineComment,
    Unknown,
};

struct Token {
    TokenKind kind = TokenKind::Unknown;
    SyntaxMode mode = SyntaxMode::JassLike;
    SourceLocation loc;
    std::string text;
};

const char* tokenKindName(TokenKind kind);
bool isKeywordText(std::string_view text);

} // namespace vjassc
